using LabWorkOrganization.Application.Dtos.UserDtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<User>> AddUser(UserRegisterDto user);
        Task<Result<User>> DeleteUser(Guid id);
        Task<Result<IEnumerable<User>>> GetAllUsers();
        string GetCurrentUserId();
        Task<Result<User?>> GetUserByEmail(string email);
        Task<Result<User?>> GetUserById(Guid id);
        Task<Result<User>> UpdateUser(Guid id, User updatedUser);
    }
}