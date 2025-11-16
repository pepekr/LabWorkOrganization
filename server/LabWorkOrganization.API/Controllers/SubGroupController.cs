using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Dtos.SubGroupDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    [Route("api/courses/{courseId}/subgroups")]
    [ApiController]
    public class SubGroupController : ControllerBase
    {
        private readonly ISubgroupService _subGroupService;

        public SubGroupController(ISubgroupService subGroupService)
        {
            _subGroupService = subGroupService;
        }

        // POST: api/courses/{courseId}/subgroups/create
        // Creates a new subgroup under a specific course
        [HttpPost("create")]
        public async Task<IActionResult> CreateSubgroup([FromBody] SubGroupCreationalDto subgroup)
        {
            Result<SubGroup> result = await _subGroupService.CreateSubgroup(subgroup);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // GET: api/courses/{courseId}/subgroups/getAll
        // Retrieves all subgroups associated with a given course
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllSubgroupsByCourseId([FromRoute] string courseId)
        {
            Result<IEnumerable<SubGroupDto>> result = await _subGroupService.GetAllSubgroupsByCourseId(courseId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // POST: api/courses/{courseId}/subgroups/{subGroupId}/queue/add
        // Adds a new student/place to a subgroup queue
        [HttpPost("{subGroupId}/queue/add")]
        public async Task<IActionResult> AddToQueue([FromRoute] string subGroupId,
            [FromBody] QueuePlaceCreationalDto queuePlace)
        {
            Result<SubGroupDto> result = await _subGroupService.AddToQueue(queuePlace);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // POST: api/courses/{courseId}/subgroups/{subGroupId}/queue/remove
        // Removes a student/place from a subgroup queue
        [HttpPost("{subGroupId}/queue/remove")]
        public async Task<IActionResult> RemoveFromQueue([FromRoute] string subGroupId, [FromBody] string queuePlaceId)
        {
            Result<SubGroupDto> result = await _subGroupService.RemoveFromQueue(queuePlaceId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // PUT: api/courses/{courseId}/subgroups/{subgroupId}/students
        // Updates the list of students in a subgroup
        [HttpPut("{subgroupId}/students")]
        public async Task<IActionResult> UpdateStudents([FromRoute] string subgroupId,
            [FromBody] SubGroupStudentsDto subGroupStudentsDto)
        {
            // Ensure route ID matches DTO ID
            if (subgroupId != subGroupStudentsDto.SubGroupId)
            {
                return BadRequest("Route ID and DTO ID do not match.");
            }

            Result<SubGroup> result = await _subGroupService.UpdateStudents(subGroupStudentsDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // DELETE: api/courses/{courseId}/subgroups/{subgroupId}
        // Deletes a subgroup by its ID
        [HttpDelete("{subgroupId}")]
        public async Task<IActionResult> DeleteSubgroup([FromRoute] string subgroupId)
        {
            Result<SubGroup> result = await _subGroupService.DeleteSubgroup(subgroupId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
}
