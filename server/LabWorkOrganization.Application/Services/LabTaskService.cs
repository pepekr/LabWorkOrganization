using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class LabTaskService
    {
        private readonly ICrudRepository<LabTask> _crudRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExternalCrudRepo<LabTask> _externalCrudRepository;
        public LabTaskService(IUnitOfWork IUnitOfWork, IExternalCrudRepo<LabTask> IExternalCrudRepository, ICrudRepository<LabTask> taskRepo)
        {
            _unitOfWork = IUnitOfWork;
            _externalCrudRepository = IExternalCrudRepository;
            _crudRepository = taskRepo;
        }
        public async Task<Result<LabTask>> CreateTask(LabTaskCreationalDto labTask, bool useExternal)
        {
            try
            {
                var errors = Validation.ValidationHelper.Validate(labTask);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }
                var newTask = new LabTask
                { // NOT ENTERING EXTERNAL ID EXTERNAL API WILL HANDLE IT
                    Id = Guid.NewGuid(),
                    Title = labTask.Title,
                    DueDate = labTask.DueDate,
                    IsSentRequired = labTask.IsSentRequired,
                    TimeLimitPerStudent = labTask.TimeLimitPerStudent,
                    CourseId = labTask.CourseId
                };
                if (useExternal)
                {
                    await _externalCrudRepository.AddAsync(newTask);
                }
                await _crudRepository.AddAsync(newTask);
                await _unitOfWork.SaveChangesAsync();
                return Result<LabTask>.Success(newTask);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the task: {ex.Message}");
                return Result<LabTask>.Failure($"An error occurred while creating the task: {ex.Message}");
            }
        }
        public async Task<Result<LabTask?>> GetTaskById(Guid id, bool external = false)
        {
            try
            {
                if (external)
                {
                    return Result<LabTask?>.Success(await _externalCrudRepository.GetByIdAsync(id));
                }
                return Result<LabTask?>.Success(await _crudRepository.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                return Result<LabTask?>.Failure($"An error occurred while getting the task: {ex.Message}");
            }

        }
        public async Task<Result<IEnumerable<LabTask>>> GetAllTasks(bool isGetExternal = false)
        {
            try
            {
                var tasks = await _crudRepository.GetAllAsync();
                if (isGetExternal)
                {

                    tasks.Concat(await _externalCrudRepository.GetAllAsync() ?? Enumerable.Empty<LabTask>());
                }
                return Result<IEnumerable<LabTask>>.Success(tasks);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<LabTask>>.Failure($"An error occured while getting the task: ${ex.Message}");
            }
        }
        public async Task<Result<LabTask>> UpdateTask(LabTask labTask, bool updateExternal = false)
        {
            try
            {
                var errors = Validation.ValidationHelper.Validate(labTask);
                if (errors.Count > 0)
                {
                    throw new ArgumentException(string.Join("; ", errors));
                }
                if (updateExternal && labTask.ExternalId is not null)
                {
                    await _externalCrudRepository.UpdateAsync(labTask, labTask.ExternalId.Value);
                }
                _crudRepository.Update(labTask);
                await _unitOfWork.SaveChangesAsync();
                return Result<LabTask>.Success(labTask);
            }
            catch (Exception ex)
            {
                return Result<LabTask>.Failure($"An error occured while updating the task: ${ex.Message}");
            }
        }

        public async Task<Result<LabTask>> DeleteTask(Guid id, bool deleteExternal)
        {
            try
            {

                var task = await _crudRepository.GetByIdAsync(id);
                if (task is null)
                {
                    throw new Exception("Task not found");
                }
                if (deleteExternal && task.ExternalId is not null)
                {
                    await _externalCrudRepository.DeleteAsync(task.ExternalId.Value);
                }
                _crudRepository.Delete(task);
                await _unitOfWork.SaveChangesAsync();
                return Result<LabTask>.Success(task);
            }
            catch (Exception ex)
            {
                return Result<LabTask>.Failure($"An error occured while deleting the task: ${ex.Message}");
            }

        }
    }
}
