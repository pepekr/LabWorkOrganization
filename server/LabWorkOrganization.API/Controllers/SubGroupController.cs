using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Dtos.SubGroupDtos;
using LabWorkOrganization.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    [Route("api/courses/{courseId}/subgroups")]
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
            var result = await _subGroupService.CreateSubgroup(subgroup);
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
            var result = await _subGroupService.GetAllSubgroupsByCourseId(courseId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        // POST: api/courses/{courseId}/subgroups/{subGroupId}/queue/add
        // Adds a new student/place to a subgroup queue
        [HttpPost("{subGroupId}/queue/add")]
        public async Task<IActionResult> AddToQueue([FromRoute] string subGroupId, [FromBody] QueuePlaceCreationalDto queuePlace)
        {
            var result = await _subGroupService.AddToQueue(queuePlace);
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
            var result = await _subGroupService.RemoveFromQueue(queuePlaceId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
