using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var result = await _authService.HandleLogin(userLoginDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }


        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegistrationDto)
        {
            var result = await _authService.HandleRegistration(userRegistrationDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

    }
}
