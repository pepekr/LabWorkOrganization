using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks/{taskId}")]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskService _userTaskService;

        public UserTaskController(IUserTaskService userTaskService)
        {
            _userTaskService = userTaskService;
        }

        [HttpGet("my-status")]
        public async Task<IActionResult> GetUserTaskStatus([FromRoute] string taskId)
        {
            Result<UserTask> result = await _userTaskService.GetUserTaskStatus(taskId);
            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpPost("complete")]
        public async Task<IActionResult> MarkAsCompleted([FromRoute] string taskId)
        {
            Result<UserTask> result = await _userTaskService.MarkAsCompleted(taskId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpPost("return")]
        public async Task<IActionResult> MarkAsReturned([FromRoute] string taskId)
        {
            Result<UserTask> result = await _userTaskService.MarkAsReturned(taskId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
}
