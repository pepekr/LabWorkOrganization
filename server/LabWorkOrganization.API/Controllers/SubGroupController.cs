using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabWorkOrganization.API.Controllers
{
    [Route("api/courses/{courseId}/subgroups")]
    public class SubGroupController : ControllerBase
    {
        private readonly SubgroupService _subGroupService;
        public SubGroupController(SubgroupService subGroupService)
        {
            _subGroupService = subGroupService;
        }

        [HttpPost("/create")]
        public async Task<IActionResult> CreateSubgroup([FromBody] SubGroupCreationalDto subgroup)
        {
            var result = await _subGroupService.CreateSubgroup(subgroup);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
        [HttpGet("/getAll")]
        public async Task<IActionResult> GetAllSubgroupsByCourseId([FromRoute] Guid courseId)
        {
            var result = await _subGroupService.GetAllByCourseId(courseId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        [HttpPost("{subGroupId}/queue/add")]
        public async Task<IActionResult> AddToQueue([FromRoute] Guid subGroupId, [FromBody] QueuePlaceCreationalDto queuePlace)
        {
            var result = await _subGroupService.AddToQueue(queuePlace);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        [HttpPost("{subGroupId}/queue/remove")]
        public async Task<IActionResult> RemoveFromQueue([FromRoute] Guid subGroupId, [FromBody] Guid queuePlaceId)
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
