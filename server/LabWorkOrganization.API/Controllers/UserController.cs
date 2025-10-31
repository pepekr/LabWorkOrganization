using LabWorkOrganization.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("subgroup/{subgroupId}")]
        public async Task<IActionResult> GetUsersBySubGroupId(string subgroupId)
        {
            var a = await _userService.GetAllUsersBySubGroupId(subgroupId);
            if (!a.IsSuccess)
            {

                return BadRequest(a.ErrorMessage);
            }
            return Ok(a.Data);
        }
    }
}
