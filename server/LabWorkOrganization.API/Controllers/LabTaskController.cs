using global::LabWorkOrganization.Application.Dtos.LabTaskDtos;
using global::LabWorkOrganization.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // GET: api/courses/{courseId}/tasks/getAll
        // Retrieves all lab tasks for a given course
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllTaskByCourseId([FromRoute] string courseId)
        {
            var result = await _labTaskService.GetAllTasksByCourseId(courseId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        // GET: api/courses/{courseId}/tasks/getById/{id}
        // Retrieves a specific lab task by its ID
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetTaskById([FromRoute] string id, [FromBody] LabTaskGetDto dto)
        {
            var result = await _labTaskService.GetTaskById(id, dto.CourseId, dto.UseExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        // POST: api/courses/{courseId}/tasks/create
        // Creates a new lab task
        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] LabTaskCreationalDto labTask)
        {
            var result = await _labTaskService.CreateTask(labTask, labTask.UseExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        // DELETE: api/courses/{courseId}/tasks/delete/{id}
        // Deletes an existing lab task
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTask([FromRoute] string id, [FromBody] LabTaskAlterDto task)
        {
            var result = await _labTaskService.DeleteTask(id, task.LabTask.CourseId, task.UseExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        // PATCH: api/courses/{courseId}/tasks/update/{id}
        // Updates the details of an existing lab task
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateTask([FromRoute] string id, [FromBody] LabTaskAlterDto task)
        {
            var result = await _labTaskService.UpdateTask(task.LabTask, task.UseExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
