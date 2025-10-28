using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LabWorkOrganization.Application.Services
{
    public class ExternalAuthService : IExternalAuthService
    {
        private readonly IExternalTokenService _externalTokenService;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _currentUserId;
        private readonly string _currentUserEmail;
        private readonly IExternalTokenValidation _tokenValidator;
        public ExternalAuthService(IExternalTokenService externalTokenService, IUserService userService, IUnitOfWork unitOfWork, IHttpContextAccessor ctx, IExternalTokenValidation tokenValidator)
        {
            _tokenValidator = tokenValidator;
            _externalTokenService = externalTokenService;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _currentUserId = ctx.HttpContext?.User?.FindFirst("id")?.Value ?? "";
            _currentUserEmail = ctx.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? "";
            if (_currentUserEmail.Length == 0 && _currentUserId.Length == 0) { throw new Exception("User not authenticated"); }
        }
        public async Task<Result<JWTTokenDto>> HandleExternalAuth(string code, ClaimsPrincipal? claimsPrincipal)
        {
            try
            {
                TokenResponse tokenR = await GetGoogleTokensAsync(code);
                ClaimsPrincipal? googleUserIdentityPrincipal = await _tokenValidator.ValidateJwtTokenAsync(tokenR.IdToken);
                var googleEmail = googleUserIdentityPrincipal?.FindFirst(ClaimTypes.Email)?.Value;
                if (googleEmail != _currentUserEmail) throw new Exception("Email mismatch, use the same email you used in main application registration");
                var userDbResult = await _userService.GetUserByEmail(googleEmail);
                if (!userDbResult.IsSuccess) throw new Exception(userDbResult.ErrorMessage);
                if (userDbResult.Data is null) throw new Exception("User not found");

                var extTokenDto = new ExternalTokenDto
                {
                    AccessToken = tokenR.AccessToken,
                    RefreshToken = tokenR.RefreshToken,
                    ExpiresIn = DateTime.UtcNow.AddSeconds(tokenR.ExpiresInSeconds ?? 3600),
                    ApiName = "Google",

                };

                userDbResult.Data.SubGoogleId = googleUserIdentityPrincipal?
                                                .FindFirst("sub")?.Value
                                                ?? googleUserIdentityPrincipal?
                                                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                await _userService.UpdateUser(userDbResult.Data.Id, userDbResult.Data);
                await _externalTokenService.SaveTokenAsync(_currentUserId, extTokenDto);
                await _unitOfWork.SaveChangesAsync();
                return Result<JWTTokenDto>.Success(
                    new JWTTokenDto
                    {
                        AccessToken = tokenR.AccessToken,
                        RefreshToken = tokenR.RefreshToken ?? ""
                    }
                    );
            }
            catch (Exception ex)
            {
                return Result<JWTTokenDto>.Failure($"An error occured during login: {ex.Message}");
            }

        }
        public async Task<TokenResponse> GetGoogleTokensAsync(string code)
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID"),
                    ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET")
                },
                Scopes = new[]
                {
            "openid",
            "email",
            "profile",
            "https://www.googleapis.com/auth/userinfo.email",
            "https://www.googleapis.com/auth/userinfo.profile"
        }
            };

            var flow = new GoogleAuthorizationCodeFlow(initializer);

            var token = await flow.ExchangeCodeForTokenAsync(
                userId: "",
                code: code,
                redirectUri: Environment.GetEnvironmentVariable("REDIRECT_URI")!,
                taskCancellationToken: CancellationToken.None
            );

            return token;
        }

        public string RedirectUri()
        {
            var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
            var redirectUri = Environment.GetEnvironmentVariable("REDIRECT_URI");

            string[] scopes = new[]
            {
                "https://www.googleapis.com/auth/userinfo.email",
                "https://www.googleapis.com/auth/userinfo.profile",
                "openid",
                "https://www.googleapis.com/auth/classroom.courses.readonly",
                "https://www.googleapis.com/auth/classroom.coursework.students",
                "https://www.googleapis.com/auth/classroom.courseworkmaterials",
                "https://www.googleapis.com/auth/classroom.topics",
                "https://www.googleapis.com/auth/classroom.rosters.readonly",
                "https://www.googleapis.com/auth/classroom.coursework.me",
                "https://www.googleapis.com/auth/classroom.courses",
                "https://www.googleapis.com/auth/classroom.rosters",
                "https://www.googleapis.com/auth/classroom.profile.emails"
            };


            var scopeParam = Uri.EscapeDataString(string.Join(" ", scopes));

            var authorizationUrl =
                $"https://accounts.google.com/o/oauth2/v2/auth?" +
                $"response_type=code&" +
                $"client_id={Uri.EscapeDataString(clientId)}&" +
                $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                $"scope={scopeParam}&" +
                $"access_type=offline&" +
                $"prompt=consent";

            return authorizationUrl;

        }
        public async Task<Result<string>> HandleExternalLogout()
        {
            try
            {
                await _externalTokenService.RemoveTokenAsync(_currentUserId, "Google");
                await _unitOfWork.SaveChangesAsync();
                return Result<string>.Success("External logout successful");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"An error occured during external logout: {ex.Message}");
            }
        }
        public async Task<Result<bool>> IsLoggedIn()
        {
            try
            {
                var token = await _externalTokenService.GetAccessTokenFromDbAsync(_currentUserId, "Google");
                if (token is null) throw new Exception("No external token found");
                if (!token.IsSuccess) throw new Exception(token.ErrorMessage);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"An error occured during checking external login status: {ex.Message}");
            }
        }
    }

}
