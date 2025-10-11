using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class TaskService
    {
        private readonly ICrudRepository<Task> _crudRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExternalCrudRepo<Task> _externalCrudRepository;
        public TaskService(IUnitOfWork IUnitOfWork, IExternalCrudRepo<Task> IExternalCrudRepository, ICrudRepository<Task> taskRepo)
        {
            _unitOfWork = IUnitOfWork;
            _externalCrudRepository = IExternalCrudRepository;
            _crudRepository = taskRepo;
        }
        public async Task<Result<Task>> CreateTask(Task course, bool useExternal)
        {
            try
            {
                if (useExternal)
                {
                    await _externalCrudRepository.AddAsync(course);
                }
                await _crudRepository.AddAsync(course);
                await _unitOfWork.SaveChangesAsync();
                return Result<Task>.Success(course);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the task: {ex.Message}");
                return Result<Task>.Failure($"An error occurred while creating the task: {ex.Message}");
            }
        }
        public async Task<Result<Task?>> GetTaskById(Guid id, bool external)
        {
            try
            {
                if (external)
                {
                    return Result<Task?>.Success(await _externalCrudRepository.GetByIdAsync(id));
                }
                return Result<Task?>.Success(await _crudRepository.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                return Result<Task?>.Failure($"An error occurred while getting the task: {ex.Message}");
            }

        }
        public async Task<Result<IEnumerable<Task>>> GetAllTasks(bool isGetExternal)
        {
            try
            {
                var tasks = await _crudRepository.GetAllAsync();
                if (isGetExternal)
                {

                    tasks.Concat(await _externalCrudRepository.GetAllAsync() ?? Enumerable.Empty<Task>());
                }
                return Result<IEnumerable<Task>>.Success(tasks);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Task>>.Failure($"An error occured while getting the task: ${ex.Message}");
            }
        }
        public async Task<Result<Task>> UpdateTask(Task course, bool updateExternal)
        {
            try
            {
                if (updateExternal)
                {
                    await _externalCrudRepository.UpdateAsync(course);
                }
                _crudRepository.Update(course);
                await _unitOfWork.SaveChangesAsync();
                return Result<Task>.Success(course);
            }
            catch (Exception ex)
            {
                return Result<Task>.Failure($"An error occured while updating the task: ${ex.Message}");
            }
        }

        public async Task<Result<Task>> DeleteTask(Guid id, bool deleteExternal)
        {
            try
            {

                var task = await _crudRepository.GetByIdAsync(id);
                if (task is null)
                {
                    throw new Exception("Task not found");
                }
                if (deleteExternal)
                {
                    await _externalCrudRepository.DeleteAsync(task);
                }
                _crudRepository.Delete(task);
                await _unitOfWork.SaveChangesAsync();
                return Result<Task>.Success(task);
            }
            catch (Exception ex)
            {
                return Result<Task>.Failure($"An error occured while deleting the task: ${ex.Message}");
            }

        }
    }
}
