using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;
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
            Result<IEnumerable<User>> a = await _userService.GetAllUsersBySubGroupId(subgroupId);
            if (!a.IsSuccess)
            {
                return BadRequest(a.ErrorMessage);
            }

            return Ok(a.Data);
        }
        
        [HttpGet("student/courses")]
        public async Task<IActionResult> GetStudentCourses()
        {
            Result<IEnumerable<Course>> result = await _userService.GetStudentCoursesAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
