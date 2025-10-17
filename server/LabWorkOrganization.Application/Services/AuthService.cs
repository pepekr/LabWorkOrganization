using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Dtos.UserDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Http;

namespace LabWorkOrganization.Application.Services
{
    public class AuthService : IAuthService
    {
        private string? _currentUserId;
        private readonly IUserService _userService;
        private readonly IJwtTokenManager _tokenManager;
        private readonly IPasswordHasher _passwordHasher;


        public AuthService(IHttpContextAccessor ctx, IUserService userService, IJwtTokenManager jwtTokenManager, IPasswordHasher passwordHasher)
        {
            _currentUserId = ctx.HttpContext?.User?.FindFirst("id")?.Value;
            _userService = userService;
            _tokenManager = jwtTokenManager;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<JWTTokenDto>> HandleLogin(UserLoginDto user)
        {
            try
            {
                var userDbResult = await _userService.GetUserByEmail(user.Email);
                if (!userDbResult.IsSuccess) throw new Exception(userDbResult.ErrorMessage);
                if (userDbResult.Data is null) throw new Exception("User not found");
                var res = _passwordHasher.VerifyPassword(user.Password, userDbResult.Data.HashedPassword.ToString());
                if (!res) throw new Exception("Invalid password");
                var access = _tokenManager.GenerateJwtToken(userDbResult.Data.Email, userDbResult.Data.Id.ToString(), userDbResult.Data.SubGoogleId?.ToString(), 60);
                var refresh = _tokenManager.GenerateJwtToken(userDbResult.Data.Email, userDbResult.Data.Id.ToString(), userDbResult.Data.SubGoogleId?.ToString(), 1440);
                return Result<JWTTokenDto>.Success(

                    new JWTTokenDto
                    {
                        AccessToken = access,
                        RefreshToken = refresh
                    }
                    );
            }
            catch (Exception ex)
            {
                return Result<JWTTokenDto>.Failure($"An error occured during login: {ex.Message}");
            }
        }

        public async Task<Result<JWTTokenDto>> HandleRefresh(string refreshToken)
        {
            try
            {
                var principal = _tokenManager.ValidateToken(refreshToken);
                if (principal is null) throw new Exception("Invalid token");
                var userId = principal.FindFirst("id")?.Value;
                if (userId is null) throw new Exception("Invalid token");
                var userDbResult = await _userService.GetUserById(Guid.Parse(userId));
                if (!userDbResult.IsSuccess) throw new Exception(userDbResult.ErrorMessage);
                if (userDbResult.Data is null) throw new Exception("User not found");
                var access = _tokenManager.GenerateJwtToken(userDbResult.Data.Email, userDbResult.Data.Id.ToString(), userDbResult.Data.SubGoogleId.ToString(), 60);
                var refresh = _tokenManager.GenerateJwtToken(userDbResult.Data.Email, userDbResult.Data.Id.ToString(), userDbResult.Data.SubGoogleId.ToString(), 1440);
                return Result<JWTTokenDto>.Success(

                    new JWTTokenDto
                    {
                        AccessToken = access,
                        RefreshToken = refresh
                    }
                    );
            }
            catch (Exception ex)
            {
                return Result<JWTTokenDto>.Failure($"An error occured during token refresh: {ex.Message}");
            }
        }

        public async Task<Result<JWTTokenDto>> HandleRegistration(UserRegisterDto user)
        {
            try
            {
                var userDbResult = await _userService.GetUserByEmail(user.Email);
                if (userDbResult.IsSuccess) throw new Exception("User with this email exists already");
                var addUserResult = await _userService.AddUser(user);
                if (!addUserResult.IsSuccess) throw new Exception(addUserResult.ErrorMessage);
                return await HandleLogin(new UserLoginDto { Email = user.Email, Password = user.Password });
            }
            catch (Exception ex)
            {
                return Result<JWTTokenDto>.Failure($"An error occured during registration: {ex.Message}");
            }
        }
    }
}
