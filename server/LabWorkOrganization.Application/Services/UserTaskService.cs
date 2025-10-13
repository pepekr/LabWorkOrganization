using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class UserTaskService
    {
        private readonly ICrudRepository<UserTask> _userTaskRepository;
        private readonly ICrudRepository<LabTask> _taskRepository;
        private readonly ICrudRepository<SubGroup> _subGroupRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserTaskService(
            ICrudRepository<UserTask> userTaskRepository,
            ICrudRepository<LabTask> taskRepository,
            IUnitOfWork unitOfWork)
        {
            _userTaskRepository = userTaskRepository;
            _taskRepository = taskRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserTask>> AddUserTask(UserTaskCreationalDto userTask)
        {
            try
            {
                var errors = Validation.ValidationHelper.Validate(userTask);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }
                var task = await _taskRepository.GetByIdAsync(userTask.TaskId);
                if (task == null)
                    return Result<UserTask>.Failure("Task not found");
                //var user =  await userRepo() TODO: create user repo and use it here

                await _userTaskRepository.AddAsync(userTask);
                await _unitOfWork.SaveChangesAsync();
                return Result<UserTask>.Success(userTask);
            }
            catch (Exception ex)
            {
                return Result<UserTask>.Failure($"Failed to add UserTask: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<UserTask>>> GetUserTasks(Guid userId)
        {
            try
            {
                var tasks = (await _userTaskRepository.GetAllAsync())
                    .Where(ut => ut.UserId == userId);
                return Result<IEnumerable<UserTask>>.Success(tasks);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<UserTask>>.Failure($"Failed to get tasks: {ex.Message}");
            }
        }

        public async Task<Result<bool>> HasCompletedAllTasks(Guid userId, Guid subGroupId)
        {
            try
            {
                // Load the subgroup
                var subGroup = await _subGroupRepository.GetByIdAsync(subGroupId);
                if (subGroup == null)
                    return Result<bool>.Failure("SubGroup not found");

                // Get all tasks in the course of this subgroup
                var courseTasks = subGroup.Course.Tasks.ToList();

                if (!courseTasks.Any())
                    return Result<bool>.Success(true); // no tasks, consider all completed

                // Get all tasks the user has completed
                var completedTaskIds = (await _userTaskRepository.GetAllAsync())
                    .Where(ut => ut.UserId == userId && ut.IsCompleted)
                    .Select(ut => ut.TaskId)
                    .ToHashSet();

                bool allCompleted = courseTasks.All(t => completedTaskIds.Contains(t.Id));

                return Result<bool>.Success(allCompleted);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error checking tasks: {ex.Message}");
            }
        }

        public async Task<Result<UserTask>> MarkTaskCompleted(Guid userTaskId)
        {
            try
            {
                var userTask = await _userTaskRepository.GetByIdAsync(userTaskId);
                if (userTask == null)
                    return Result<UserTask>.Failure("UserTask not found");

                userTask.IsCompleted = true;
                _userTaskRepository.Update(userTask);
                await _unitOfWork.SaveChangesAsync();

                return Result<UserTask>.Success(userTask);
            }
            catch (Exception ex)
            {
                return Result<UserTask>.Failure($"Error marking task completed: {ex.Message}");
            }
        }
    }
}
