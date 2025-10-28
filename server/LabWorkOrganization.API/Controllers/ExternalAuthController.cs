using LabWorkOrganization.Application.Interfaces;
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
        [HttpGet("external-login")]
        public IActionResult ExternalLogin([FromQuery] string returnUrl)
        {
            return Redirect(_extAuthService.RedirectUri());
        }
        [HttpGet("external-callback")]
        public async Task<IActionResult> ExternalCallback([FromQuery] string code)
        {
            var result = await _extAuthService.HandleExternalAuth(code, User);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Redirect($"http://localhost:4200?linked=true"); ;
        }
        [HttpPost("external-logout")]
        public async Task<IActionResult> ExternalLogout()
        {
            var result = await _extAuthService.HandleExternalLogout();
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
        [Authorize]
        [HttpGet("isLoggedIn")]
        public async Task<IActionResult> IsLoggedIn()
        {
            var result = await _extAuthService.IsLoggedIn();
            if (!result.IsSuccess)
            {
                return Unauthorized(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
