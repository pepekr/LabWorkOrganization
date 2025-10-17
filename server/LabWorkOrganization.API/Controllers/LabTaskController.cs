namespace LabWorkOrganization.API.Controllers
{
    using global::LabWorkOrganization.Application.Dtos;
    using global::LabWorkOrganization.Application.Services;
    using global::LabWorkOrganization.Domain.Entities;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace LabWorkOrganization.API.Controllers
    {
        [Authorize]
        [Route("api/courses/{courseid}/tasks")]
        [ApiController]
        public class LabTaskController : ControllerBase
        {
            private readonly LabTaskService _labTaskService;
            public LabTaskController(LabTaskService labTaskService)
            {
                _labTaskService = labTaskService;
            }

            [HttpGet("/getAll")]
            public async Task<IActionResult> GetAllTaskByCourseId([FromRoute] Guid courseId)
            {
                var result = await _labTaskService.GetAllTasksByCourseId(courseId);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }
                return Ok(result.Data);
            }
            [HttpGet("/getById/{id}")]
            public async Task<IActionResult> GetTaskById([FromRoute] Guid id, [FromBody] Guid courseId, [FromBody] bool isExternalCourse)
            {
                var result = await _labTaskService.GetTaskById(id, courseId, isExternalCourse);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }
                return Ok(result.Data);
            }
            [HttpPost("/create")]
            public async Task<IActionResult> CreateTask([FromBody] LabTaskCreationalDto labTask, [FromBody] bool useExternal)
            {
                var result = await _labTaskService.CreateTask(labTask, useExternal);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }
                return Ok(result.Data);
            }

            [HttpDelete("/delete/{id}")]
            public async Task<IActionResult> DeleteTask([FromRoute] Guid id, [FromBody] Guid courseId, [FromBody] bool isExternalTask)
            {
                var result = await _labTaskService.DeleteTask(id, courseId, isExternalTask);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }
                return Ok(result.Data);
            }

            [HttpPatch("/update/{id}")]
            public async Task<IActionResult> UpdateTask([FromRoute] Guid id, [FromBody] LabTask task, [FromBody] bool isExternalTask)
            {
                var result = await _labTaskService.UpdateTask(task, isExternalTask);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.ErrorMessage);
                }
                return Ok(result.Data);
            }
        }
    }

}
