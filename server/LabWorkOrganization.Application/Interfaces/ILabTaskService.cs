using LabWorkOrganization.Application.Dtos.LabTaskDtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Interfaces
{
    public interface ILabTaskService
    {
        Task<Result<LabTask>> CreateTask(LabTaskCreationalDto labTask, bool useExternal);
        Task<Result<LabTask>> DeleteTask(string id, string courseId, bool deleteExternal);
        Task<Result<IEnumerable<LabTask>>> GetAllTasks(string courseId, bool isGetExternal = false);
        Task<Result<IEnumerable<LabTask>>> GetAllTasksByCourseId(string courseId, bool external = false);
        Task<Result<LabTask?>> GetTaskById(string id, string courseId, bool external = false);
        Task<Result<LabTask>> UpdateTask(string id, LabTaskCreationalDto labTask, bool updateExternal = false);
        Task<Result<IEnumerable<LabTask>>> SearchTask(string courseId, string? title, DateTime? dueDate,
            bool useExternal);
    }
}
