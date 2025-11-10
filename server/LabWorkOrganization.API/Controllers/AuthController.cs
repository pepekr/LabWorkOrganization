using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Dtos.UserDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            Result<JWTTokenDto> result = await _authService.HandleLogin(userLoginDto);

            if (result.IsSuccess && result.Data is not null)
            {
                CookieOptions accessCookieOptions = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                };

                CookieOptions refreshCookieOptions = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(10)
                };

                Response.Cookies.Append("access_token", result.Data.AccessToken, accessCookieOptions);
                Response.Cookies.Append("refresh_token", result.Data.RefreshToken, refreshCookieOptions);

                return Ok(new { message = "Login successful" });
            }

            return BadRequest(result.ErrorMessage);
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegistrationDto)
        {
            
            Result<JWTTokenDto> result = await _authService.HandleRegistration(userRegistrationDto);
            if (result.IsSuccess && result.Data is not null)
            {
                CookieOptions accessCookieOptions = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                };

                CookieOptions refreshCookieOptions = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(10)
                };

                Response.Cookies.Append("access_token", result.Data.AccessToken, accessCookieOptions);
                Response.Cookies.Append("refresh_token", result.Data.RefreshToken, refreshCookieOptions);

                return Ok(new { message = "Login successful" });
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpGet("isLoggedIn")]
        public IActionResult IsLoggedIn()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                string? email = User.Claims.FirstOrDefault(c =>
                        c.Type == "email" ||
                        c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                    ?.Value;

                if (!string.IsNullOrEmpty(email))
                {
                    return Ok(new { isLoggedIn = true, email });
                }
            }

            return Unauthorized(new { isLoggedIn = false, message = "User is not logged in" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Console.WriteLine("Logging out user...");
            Response.Cookies.Delete("access_token", new CookieOptions { Path = "/", Secure = true });
            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/", Secure = true });
            return Ok(new { message = "Logout successful" });
        }
    }
}
