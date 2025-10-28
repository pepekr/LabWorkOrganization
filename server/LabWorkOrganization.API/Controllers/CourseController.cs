using LabWorkOrganization.Application.Dtos.CourseDtos;
using LabWorkOrganization.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    [Authorize]
    [Route("api/courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // GET: api/courses/getAllByUserId
        // Retrieves all courses associated with the current user
        [HttpGet("getAllByUserId")]
        public async Task<IActionResult> GetAllCourses([FromQuery] bool isGetExternal = false)
        {
            var result = await _courseService.GetAllCoursesByUserAsync(isGetExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        // GET: api/courses/getById/{id}
        // Retrieves a specific course by its ID
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetCourseById([FromRoute] string id, [FromQuery] bool isExternalCourse)
        {
            var result = await _courseService.GetCourseById(id, isExternalCourse);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        // POST: api/courses/create
        // Creates a new course
        [HttpPost("create")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreationalDto course)
        {
            var result = await _courseService.CreateCourse(course, course.UseExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        // DELETE: api/courses/delete/{id}
        // Deletes a specific course by ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] string id, [FromBody] bool isExternalCourse)
        {
            var result = await _courseService.DeleteCourse(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        // PATCH: api/courses/update/{id}
        // Updates the details of an existing course
        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateCourse([FromRoute] string id, [FromBody] CourseAlterDto course)
        {
            var result = await _courseService.UpdateCourse(course.course, course.useExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
