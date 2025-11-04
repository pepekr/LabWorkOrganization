using LabWorkOrganization.Application.Dtos.LabTaskDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    namespace LabWorkOrganization.API.Controllers
    {
        [Authorize]
        [Route("api/courses/{courseId}/tasks")]
        [ApiController]
        public class LabTaskController : ControllerBase
        {
            private readonly ILabTaskService _labTaskService;

            public LabTaskController(ILabTaskService labTaskService)
            {
                _labTaskService = labTaskService;
            }

            [HttpGet("getAll")]
            public async Task<IActionResult> GetAllTaskByCourseId([FromRoute] string courseId)
            {
                Result<IEnumerable<LabTask>> result = await _labTaskService.GetAllTasksByCourseId(courseId);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }

            [HttpGet("getById/{id}")]
            public async Task<IActionResult> GetTaskById([FromRoute] string id, [FromRoute] string courseId, [FromQuery] bool useExternal)
            {
                Result<LabTask?> result = await _labTaskService.GetTaskById(id, courseId, useExternal);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }

            [HttpPost("create")]
            public async Task<IActionResult> CreateTask([FromBody] LabTaskCreationalDto labTask)
            {
                Result<LabTask> result = await _labTaskService.CreateTask(labTask, labTask.UseExternal);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }

            [HttpDelete("delete/{id}")]
            public async Task<IActionResult> DeleteTask([FromRoute] string id, [FromBody] LabTaskAlterDto task)
            {
                Result<LabTask> result = await _labTaskService.DeleteTask(id, task.CourseId, task.UseExternal);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }

            [HttpPatch("update/{id}")]
            public async Task<IActionResult> UpdateTask([FromRoute] string id, [FromBody] LabTaskCreationalDto task)
            {
                Result<LabTask> result = await _labTaskService.UpdateTask(id, task, task.UseExternal);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }

                return Ok(result.Data);
            }
        }
    }
}
