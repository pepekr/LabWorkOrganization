using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LabWorkOrganization.Application.Services
{
    public class ExternalAuthService
    {
        private readonly IExternalTokenStorage _externalTokenStorage;
        private readonly UserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _currentUserId;
        private readonly string _currentUserEmail;
        public ExternalAuthService(IExternalTokenStorage externalTokenStorage, UserService userService, IUnitOfWork unitOfWork, IHttpContextAccessor ctx)
        {
            _externalTokenStorage = externalTokenStorage;
            _userService = userService;
            _unitOfWork = unitOfWork;
            _currentUserId = ctx.HttpContext?.User?.FindFirst("id")?.Value ?? "";
            _currentUserEmail = ctx.HttpContext?.User?.FindFirst("email")?.Value ?? "";
            if (_currentUserEmail.Length == 0 || _currentUserId.Length == 0) throw new Exception("User not authenticated");
        }
        public async Task<Result<JWTTokenDto>> HandleExternalAuth(string code, ClaimsPrincipal? claimsPrincipal)
        {
            try
            {
                var googleEmail = claimsPrincipal?.FindFirst(ClaimTypes.Email)?.Value;
                if (googleEmail != _currentUserEmail) throw new Exception("Email mismatch, use the same email you used in main application registration");
                var userDbResult = await _userService.GetUserByEmail(googleEmail);
                if (!userDbResult.IsSuccess) throw new Exception(userDbResult.ErrorMessage);
                if (userDbResult.Data is null) throw new Exception("User not found");
                TokenResponse tokenR = await GetGoogleTokensAsync(code);
                var extTokenDto = new ExternalToken
                {
                    AccessToken = tokenR.AccessToken,
                    RefreshToken = tokenR.RefreshToken,
                    ExpiresIn = DateTime.UtcNow.AddSeconds(tokenR.ExpiresInSeconds ?? 3600),
                    ApiName = "Google",
                    UserId = Guid.Parse(_currentUserId)
                };
                _externalTokenStorage.SaveToken(extTokenDto);
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
                    ClientId = "<YOUR_CLIENT_ID>",
                    ClientSecret = "<YOUR_CLIENT_SECRET>"
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
    }

}
