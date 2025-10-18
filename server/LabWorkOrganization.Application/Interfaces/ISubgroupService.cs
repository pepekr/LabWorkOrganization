using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Dtos.SubGroupDtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Interfaces
{
    public interface ISubgroupService
    {
        Task<Result<SubGroup>> AddToQueue(QueuePlaceCreationalDto queuePlace);
        Task<Result<SubGroup>> CreateSubgroup(SubGroupCreationalDto subgroup);
        Task<Result<SubGroup>> DeleteSubgroup(SubGroup subgroup);
        Task<Result<SubGroup>> GetAllByCourseId(string courseId);
        Task<Result<SubGroup>> RemoveFromQueue(string queuePlaceId);
        Task<Result<SubGroup>> UpdateStudents(SubGroupStudentsDto subGroupStudents);
    }
}
