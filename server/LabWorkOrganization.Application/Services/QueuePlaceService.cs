using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class QueuePlaceService
    {
        private readonly ICrudRepository<QueuePlace> _crudRepository;
        private readonly ICrudRepository<User> _userRepository;
        private readonly ICrudRepository<SubGroup> _subGroupRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QueuePlaceService(
            ICrudRepository<QueuePlace> crudRepository,
            ICrudRepository<User> userRepository,
            ICrudRepository<SubGroup> subGroupRepository,
            IUnitOfWork unitOfWork)
        {
            _crudRepository = crudRepository;
            _userRepository = userRepository;
            _subGroupRepository = subGroupRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<QueuePlace>> CreateQueuePlace(QueuePlaceCreationalDto dto)
        {
            try
            {
                // Load user
                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user == null)
                    return Result<QueuePlace>.Failure("User not found");

                // Load subgroup
                var subGroup = await _subGroupRepository.GetByIdAsync(dto.SubGroupId);
                if (subGroup == null)
                    return Result<QueuePlace>.Failure("SubGroup not found");

                var queuePlace = new QueuePlace
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    User = user,
                    SubGroupId = dto.SubGroupId,
                    SubGroup = subGroup,
                    SpecifiedTime = dto.SpecifiedTime
                };

                await _crudRepository.AddAsync(queuePlace);
                await _unitOfWork.SaveChangesAsync();

                return Result<QueuePlace>.Success(queuePlace);
            }
            catch (Exception ex)
            {
                return Result<QueuePlace>.Failure($"An error occurred while creating the queue place: {ex.Message}");
            }
        }

        public async Task<Result<QueuePlace?>> GetById(Guid id)
        {
            try
            {
                var queuePlace = await _crudRepository.GetByIdAsync(id);
                return queuePlace != null
                    ? Result<QueuePlace?>.Success(queuePlace)
                    : Result<QueuePlace?>.Failure("QueuePlace not found");
            }
            catch (Exception ex)
            {
                return Result<QueuePlace?>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<QueuePlace>>> GetAll()
        {
            try
            {
                var queuePlaces = await _crudRepository.GetAllAsync() ?? Enumerable.Empty<QueuePlace>();
                return Result<IEnumerable<QueuePlace>>.Success(queuePlaces);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<QueuePlace>>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Result<QueuePlace>> UpdateQueuePlace(QueuePlace queuePlace)
        {
            try
            {
                _crudRepository.Update(queuePlace);
                await _unitOfWork.SaveChangesAsync();

                return Result<QueuePlace>.Success(queuePlace);
            }
            catch (Exception ex)
            {
                return Result<QueuePlace>.Failure($"An error occurred while updating the queue place: {ex.Message}");
            }
        }

        public async Task<Result<QueuePlace>> DeleteQueuePlace(Guid id)
        {
            try
            {
                var queuePlace = await _crudRepository.GetByIdAsync(id);
                if (queuePlace == null)
                    return Result<QueuePlace>.Failure("QueuePlace not found");

                _crudRepository.Delete(queuePlace);
                await _unitOfWork.SaveChangesAsync();

                return Result<QueuePlace>.Success(queuePlace);
            }
            catch (Exception ex)
            {
                return Result<QueuePlace>.Failure($"An error occurred while deleting the queue place: {ex.Message}");
            }
        }
    }
}
