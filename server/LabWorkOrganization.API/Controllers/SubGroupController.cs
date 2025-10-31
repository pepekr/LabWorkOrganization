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
        
        /**
         * Оновлює список студентів у підгрупі.
         * Очікує DTO, що містить ID підгрупи та новий список email-адрес.
         */
        [HttpPut("{subgroupId}/students")]
        public async Task<IActionResult> UpdateStudents([FromRoute] string subgroupId, [FromBody] SubGroupStudentsDto subGroupStudentsDto)
        {
            // Перевірка, що ID в DTO та ID в маршруті збігаються
            if (subgroupId != subGroupStudentsDto.SubGroupId)
            {
                return BadRequest("Route ID and DTO ID do not match.");
            }

            var result = await _subGroupService.UpdateStudents(subGroupStudentsDto);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
        
        /**
         * Видаляє підгрупу за її ID.
         */
        [HttpDelete("{subgroupId}")]
        public async Task<IActionResult> DeleteSubgroup([FromRoute] string subgroupId)
        {
            var result = await _subGroupService.DeleteSubgroup(subgroupId); // [cite: 1291, 1443]
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
