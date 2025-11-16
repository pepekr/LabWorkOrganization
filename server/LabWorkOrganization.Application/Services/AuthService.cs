using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Dtos.UserDtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LabWorkOrganization.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenManager _tokenManager;
        private readonly IUserService _userService;
        private string? _currentUserId;


        public AuthService(IHttpContextAccessor ctx, IUserService userService, IJwtTokenManager jwtTokenManager,
            IPasswordHasher passwordHasher)
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
                Result<User?> userDbResult = await _userService.GetUserByEmail(user.Email);
                if (!userDbResult.IsSuccess)
                {
                    throw new Exception(userDbResult.ErrorMessage);
                }

                if (userDbResult.Data is null)
                {
                    throw new Exception("User not found");
                }

                bool res = _passwordHasher.VerifyPassword(user.Password, userDbResult.Data.HashedPassword);
                if (!res)
                {
                    throw new Exception("Invalid password");
                }

                string access = _tokenManager.GenerateJwtToken(userDbResult.Data.Email, userDbResult.Data.Id,
                    userDbResult.Data.SubGoogleId, 60);
                string refresh = _tokenManager.GenerateJwtToken(userDbResult.Data.Email, userDbResult.Data.Id,
                    userDbResult.Data.SubGoogleId, 1440);
                return Result<JWTTokenDto>.Success(
                    new JWTTokenDto { AccessToken = access, RefreshToken = refresh }
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
                ClaimsPrincipal? principal = _tokenManager.ValidateToken(refreshToken);
                if (principal is null)
                {
                    throw new Exception("Invalid token");
                }

                string? userId = principal.FindFirst("id")?.Value;
                if (userId is null)
                {
                    throw new Exception("Invalid token");
                }

                Result<User?> userDbResult = await _userService.GetUserById(userId);
                if (!userDbResult.IsSuccess)
                {
                    throw new Exception(userDbResult.ErrorMessage);
                }

                if (userDbResult.Data is null)
                {
                    throw new Exception("User not found");
                }

                string access = _tokenManager.GenerateJwtToken(userDbResult.Data.Email, userDbResult.Data.Id,
                    userDbResult.Data.SubGoogleId, 60);
                string refresh = _tokenManager.GenerateJwtToken(userDbResult.Data.Email, userDbResult.Data.Id,
                    userDbResult.Data.SubGoogleId, 1440);
                return Result<JWTTokenDto>.Success(
                    new JWTTokenDto { AccessToken = access, RefreshToken = refresh }
                );
            }
            catch (Exception ex)
            {
                return Result<JWTTokenDto>.Failure($"An error occured during token refresh: {ex.Message}");
            }
        }

        public async Task<Result<JWTTokenDto>> HandleRegistration(UserRegisterDto user)
        {
            Console.WriteLine("Registration working");
            try
            {
                Result<User?> userDbResult = await _userService.GetUserByEmail(user.Email);
                if (userDbResult.IsSuccess)
                {
                    throw new Exception("User with this email exists already");
                }

                Result<User> addUserResult = await _userService.AddUser(user);
                if (!addUserResult.IsSuccess)
                {
                    throw new Exception(addUserResult.ErrorMessage);
                }

                return await HandleLogin(new UserLoginDto { Email = user.Email, Password = user.Password });
            }
            catch (Exception ex)
            {
                return Result<JWTTokenDto>.Failure($"An error occured during registration: {ex.Message}");
            }
        }
    }
}
