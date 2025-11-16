using Asp.Versioning;
using LabWorkOrganization.Application.Dtos.CourseDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    #region v1
    // - - - - - - - V1 - - - - - - - 
    
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/courses")]
    [ApiController]
    public class CourseControllerV1 : ControllerBase
    {
        private readonly ICourseService _courseService;


        public CourseControllerV1(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // GET: api/courses/getAllByUserId
        // Retrieves all courses associated with the current user
        [HttpGet("getAllByUserId")]
        public async Task<IActionResult> GetAllCourses([FromQuery] bool isGetExternal = false)
        {
            Result<IEnumerable<Course>> result = await _courseService.GetAllCoursesByUserAsync(isGetExternal);
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
            Result<Course?> result = await _courseService.GetCourseById(id, isExternalCourse);
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
            Result<Course> result = await _courseService.CreateCourse(course, course.UseExternal);
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
            Result<Course> result = await _courseService.DeleteCourse(id);
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
            Result<Course> result = await _courseService.UpdateCourse(course.course, course.useExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
    #endregion
    #region v2
    // - - - - - - - V2 - - - - - - - 
    
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/courses")]
    [ApiController]
    public class CourseControllerV2 : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseControllerV2(ICourseService courseService)
        {
            _courseService = courseService;
        }
        
        [HttpGet("getAllByUserId")]
        public async Task<IActionResult> GetAllCourses([FromQuery] bool isGetExternal = false)
        {
            Result<IEnumerable<Course>> result = await _courseService.GetAllCoursesByUserAsyncV2(isGetExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }


        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetCourseById([FromRoute] string id, [FromQuery] bool isExternalCourse)
        {
            Result<Course?> result = await _courseService.GetCourseByIdV2(id, isExternalCourse);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreationalDtoV2 course)
        {
            Result<Course> result = await _courseService.CreateCourseV2(course, course.UseExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] string id, [FromBody] bool isExternalCourse)
        {
            Result<Course> result = await _courseService.DeleteCourseV2(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateCourse([FromRoute] string id, [FromBody] CourseAlterDtoV2 course)
        {
            Result<Course> result = await _courseService.UpdateCourseV2(course.course, course.useExternal);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
    #endregion
}
