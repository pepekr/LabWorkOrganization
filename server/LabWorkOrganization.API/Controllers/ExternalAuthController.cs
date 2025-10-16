using LabWorkOrganization.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    public class ExternalAuthController : ControllerBase
    {
        private readonly ExternalAuthService _extAuthService;
        public ExternalAuthController(ExternalAuthService extAuthService)
        {
            _extAuthService = extAuthService;
        }
        [HttpGet("/external-login")]
        public IActionResult ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl)
        {
            return Redirect(_extAuthService.RedirectUri());
        }
        [HttpGet("/external-callback")]
        public async Task<IActionResult> ExternalCallback([FromQuery] string code)
        {
            var result = await _extAuthService.HandleExternalAuth(code, User);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
