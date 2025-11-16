using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    [Authorize]
    [Route("api/external-auth")]
    [ApiController]
    public class ExternalAuthController : ControllerBase
    {
        private readonly IExternalAuthService _extAuthService;

        public ExternalAuthController(IExternalAuthService extAuthService)
        {
            _extAuthService = extAuthService;
        }

        // Redirects user to the external authentication provider (e.g., Google)

        [HttpGet("external-login")]
        public IActionResult ExternalLogin([FromQuery] string returnUrl)
        {
            return Redirect(_extAuthService.RedirectUri());
        }

        // Handles the callback after external authentication
        [HttpGet("external-callback")]
        public async Task<IActionResult> ExternalCallback([FromQuery] string code)
        {
            Result<JWTTokenDto> result = await _extAuthService.HandleExternalAuth(code, User);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            // Redirects back to frontend after successful login
            return Redirect($"http://localhost:4200?linked=true");
        }

        // Logs the user out from the external authentication provider
        [HttpPost("external-logout")]
        public async Task<IActionResult> ExternalLogout()
        {
            Result<string> result = await _extAuthService.HandleExternalLogout();
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // Checks whether the user is currently logged in with external authentication
        [Authorize]
        [HttpGet("isLoggedIn")]
        public async Task<IActionResult> IsLoggedIn()
        {
            Result<bool> result = await _extAuthService.IsLoggedIn();
            if (!result.IsSuccess)
            {
                return Unauthorized(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
}
