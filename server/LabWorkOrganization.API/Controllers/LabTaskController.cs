using LabWorkOrganization.Application.Dtos.LabTaskDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;
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
            Result<IEnumerable<LabTask>> result = await _labTaskService.GetAllTasksByCourseId(courseId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        // GET: api/courses/{courseId}/tasks/getById/{id}
        // Retrieves a specific lab task by its ID
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

        // POST: api/courses/{courseId}/tasks/create
        // Creates a new lab task
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

        // DELETE: api/courses/{courseId}/tasks/delete/{id}
        // Deletes an existing lab task
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

        // PATCH: api/courses/{courseId}/tasks/update/{id}
        // Updates the details of an existing lab task
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

        // GET: api/courses/{courseId}/tasks/search
        // Searches for lab tasks in a course by optional title and due date
        [HttpGet("search")]
        public async Task<IActionResult> SearchTasks([FromRoute] string courseId,
            [FromQuery] string? title,
            [FromQuery] DateTime? dueDate,
            [FromQuery] bool useExternal)
        {
            Result<IEnumerable<LabTask>> result = await _labTaskService.SearchTask(courseId, title, dueDate, useExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
