using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class UserTaskService : IUserTaskService
    {
        private readonly ICrudRepository<UserTask> _userTaskRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public UserTaskService(ICrudRepository<UserTask> userTaskRepository, IUnitOfWork unitOfWork, IUserService userService)
        {
            _userTaskRepository = userTaskRepository;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        private async Task<UserTask> GetOrCreateUserTask(string taskId, string userId)
        {
            var userTasks = await _userTaskRepository.GetAllAsync();
            var userTask = userTasks.FirstOrDefault(ut => ut.TaskId == taskId && ut.UserId == userId);

            if (userTask == null)
            {
                userTask = new UserTask
                {
                    Id = Guid.NewGuid().ToString(),
                    TaskId = taskId,
                    UserId = userId,
                    IsCompleted = false
                };
                await _userTaskRepository.AddAsync(userTask);
            }
            return userTask;
        }

        public async Task<Result<UserTask>> GetUserTaskStatus(string taskId)
        {
            try
            {
                var userId = _userService.GetCurrentUserId();
                var userTask = await GetOrCreateUserTask(taskId, userId);
                await _unitOfWork.SaveChangesAsync(); // Save if created
                return Result<UserTask>.Success(userTask);
            }
            catch (Exception ex)
            {
                return Result<UserTask>.Failure(ex.Message);
            }
        }

        public async Task<Result<UserTask>> MarkAsCompleted(string taskId)
        {
            try
            {
                var userId = _userService.GetCurrentUserId();
                var userTask = await GetOrCreateUserTask(taskId, userId);
                
                userTask.IsCompleted = true;
                _userTaskRepository.Update(userTask);
                await _unitOfWork.SaveChangesAsync();
                
                return Result<UserTask>.Success(userTask);
            }
            catch (Exception ex)
            {
                return Result<UserTask>.Failure(ex.Message);
            }
        }

        public async Task<Result<UserTask>> MarkAsReturned(string taskId)
        {
            try
            {
                var userId = _userService.GetCurrentUserId();
                var userTask = await GetOrCreateUserTask(taskId, userId);

                userTask.IsCompleted = false;
                _userTaskRepository.Update(userTask);
                await _unitOfWork.SaveChangesAsync();

                return Result<UserTask>.Success(userTask);
            }
            catch (Exception ex)
            {
                return Result<UserTask>.Failure(ex.Message);
            }
        }
    }
}
