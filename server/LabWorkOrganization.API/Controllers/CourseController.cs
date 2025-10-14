using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Services;
using LabWorkOrganization.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    [Authorize]
    [Route("api/courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;
        public CourseController(CourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet("/getAll")]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await _courseService.GetAllCourses();
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
        [HttpGet("/getById/{id}")]
        public async Task<IActionResult> GetCourseById([FromRoute] Guid id, [FromBody] bool isExternalCourse)
        {
            var result = await _courseService.GetCourseById(id, isExternalCourse);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
        [HttpPost("/create")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreationalDto course, [FromBody] bool useExternal)
        {
            var result = await _courseService.CreateCourse(course, useExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        [HttpDelete("/delete/{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] Guid id, [FromBody] bool isExternalCourse)
        {
            var result = await _courseService.DeleteCourse(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        [HttpPatch("/update/{id}")]
        public async Task<IActionResult> UpdateCourse([FromRoute] Guid id, [FromBody] Course course, [FromBody] bool isExternalCourse)
        {
            var result = await _courseService.UpdateCourse(course, isExternalCourse);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
