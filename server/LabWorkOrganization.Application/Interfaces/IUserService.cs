using LabWorkOrganization.Application.Dtos.UserDtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<User>> AddUser(UserRegisterDto user);
        Task<Result<User>> DeleteUser(string id);
        Task<Result<IEnumerable<User>>> GetAllUsers();
        Task<Result<IEnumerable<User>>> GetAllUsersBySubGroupId(string subGroupId);

        Task<IEnumerable<User>> GetAllUsersByCourseId(string courseId, bool external = false);
        string GetCurrentUserId();
        string GetCurrentUserExternalId();
        Task<Result<User?>> GetUserByEmail(string email);
        Task<Result<User?>> GetUserById(string id);
        Task<Result<User>> UpdateUser(string id, User updatedUser);


    }
}
