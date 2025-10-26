using LabWorkOrganization.Application.Dtos.UserDtos;
using LabWorkOrganization.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    [Route("api/auth")]
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
            var result = await _authService.HandleLogin(userLoginDto);

            if (result.IsSuccess && result.Data is not null)
            {
                var accessCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                };

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1)
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
            var result = await _authService.HandleRegistration(userRegistrationDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        [HttpGet("isLoggedIn")]
        public async Task<IActionResult> IsLoggedIn()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var email = User.Claims.FirstOrDefault(c =>
     c.Type == "email" || c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

                if (!string.IsNullOrEmpty(email))
                {
                    return Ok(new { isLoggedIn = true, email });
                }
            }
            return Unauthorized(new { isLoggedIn = false, message = "User is not logged in" });
        }
    }
}
