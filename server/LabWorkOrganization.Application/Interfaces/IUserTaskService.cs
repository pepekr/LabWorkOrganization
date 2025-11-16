using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Interfaces
{
    public interface IUserTaskService
    {
        Task<Result<UserTask>> GetUserTaskStatus(string taskId);
        Task<Result<UserTask>> MarkAsCompleted(string taskId);
        Task<Result<UserTask>> MarkAsReturned(string taskId);
    }
}
