using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Dtos.UserDtos;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<JWTTokenDto>> HandleLogin(UserLoginDto user);
        Task<Result<JWTTokenDto>> HandleRefresh(string refreshToken);
        Task<Result<JWTTokenDto>> HandleRegistration(UserRegisterDto user);
    }
}
