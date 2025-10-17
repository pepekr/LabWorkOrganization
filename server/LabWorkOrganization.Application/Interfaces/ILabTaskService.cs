using LabWorkOrganization.Application.Dtos.LabTaskDtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Interfaces
{
    public interface ILabTaskService
    {
        Task<Result<LabTask>> CreateTask(LabTaskCreationalDto labTask, bool useExternal);
        Task<Result<LabTask>> DeleteTask(Guid id, Guid courseId, bool deleteExternal);
        Task<Result<IEnumerable<LabTask>>> GetAllTasks(Guid courseId, bool isGetExternal = false);
        Task<Result<IEnumerable<LabTask>>> GetAllTasksByCourseId(Guid courseId, bool external = false);
        Task<Result<LabTask?>> GetTaskById(Guid id, Guid courseId, bool external = false);
        Task<Result<LabTask>> UpdateTask(LabTask labTask, bool updateExternal = false);
    }
}