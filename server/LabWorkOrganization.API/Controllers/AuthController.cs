using LabWorkOrganization.Application.Dtos.UserDtos;
using LabWorkOrganization.Application.Interfaces;
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

        // POST: api/auth/login
        // Authenticates user and issues JWT tokens stored in cookies
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var result = await _authService.HandleLogin(userLoginDto);

            if (result.IsSuccess && result.Data is not null)
            {
                // Create secure cookies for access and refresh tokens
                var accessCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                };

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(10)
                };

                // Store tokens in cookies
                Response.Cookies.Append("access_token", result.Data.AccessToken, accessCookieOptions);
                Response.Cookies.Append("refresh_token", result.Data.RefreshToken, refreshCookieOptions);

                return Ok(new { message = "Login successful" });
            }
            return BadRequest(result.ErrorMessage);
        }

        // POST: api/auth/register
        // Registers a new user and issues authentication tokens
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegistrationDto)
        {
            var result = await _authService.HandleRegistration(userRegistrationDto);
            if (result.IsSuccess && result.Data is not null)
            {
                var accessCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                };

                var refreshCookieOptions = new CookieOptions
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

        // GET: api/auth/isLoggedIn
        // Checks if the current user is authenticated
        [HttpGet("isLoggedIn")]
        public IActionResult IsLoggedIn()
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

        // POST: api/auth/logout
        // Logs the user out by removing authentication cookies
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
