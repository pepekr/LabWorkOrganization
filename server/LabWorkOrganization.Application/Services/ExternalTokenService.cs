using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Application.Interfaces;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace LabWorkOrganization.Application.Services
{
    public class ExternalTokenService : IExternalTokenService
    {
        private readonly IExternalTokenProvider _tokenProvider;
        private readonly IExternalTokenStorage _tokenStorage;
        private readonly IExternalTokenValidation _tokenValidation;
        private readonly IUnitOfWork _unitOfWork;

        public ExternalTokenService(IExternalTokenStorage tokenStorage, IUnitOfWork IUnitOfWork,
            IExternalTokenValidation tokenValidation, IExternalTokenProvider ITokenProvider)
        {
            _tokenStorage = tokenStorage;
            _unitOfWork = IUnitOfWork;
            _tokenValidation = tokenValidation;
            _tokenProvider = ITokenProvider;
        }

        public async Task<Result<string>> GetAccessTokenFromDbAsync(string userId, string apiName)
        {
            try
            {
                ExternalToken? tokenEntity = await _tokenStorage.GetAccessTokenAsync(userId, apiName);
                if (tokenEntity is null)
                {
                    throw new Exception("Token not found");
                }

                if (tokenEntity.RefreshToken is null)
                {
                    _tokenStorage.RemoveToken(tokenEntity);
                    await _unitOfWork.SaveChangesAsync();
                    throw new Exception("Refresh token was not found, token removed");
                }

                if (!string.IsNullOrEmpty(tokenEntity.AccessToken))
                {
                    try
                    {
                        await _tokenValidation.ValidateOpaqueTokenAsync(tokenEntity.AccessToken, "access_token");
                        return Result<string>.Success(tokenEntity.AccessToken);
                    }
                    catch (SecurityTokenException)
                    {
                        Result<ExternalToken> refreshedResult =
                            await GetRefreshedToken(tokenEntity.RefreshToken, userId, apiName);
                        if (!refreshedResult.IsSuccess || refreshedResult.Data is null)
                        {
                            throw new Exception(refreshedResult.ErrorMessage ?? "Failed to refresh token");
                        }

                        return Result<string>.Success(refreshedResult.Data.AccessToken);
                    }
                }

                Result<ExternalToken> refreshResult =
                    await GetRefreshedToken(tokenEntity.RefreshToken, userId, apiName);
                if (!refreshResult.IsSuccess || refreshResult.Data is null)
                {
                    throw new Exception(refreshResult.ErrorMessage ?? "Failed to refresh token");
                }

                return Result<string>.Success(refreshResult.Data.AccessToken);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(ex.Message);
            }
        }

        public async Task<Result<ExternalToken>> SaveTokenAsync(string userId, ExternalTokenDto extTokenDto)
        {
            ExternalToken? tokenEntity = null;

            try
            {
                //need an opaque validation method this is for jwt only
                await _tokenValidation.ValidateOpaqueTokenAsync(extTokenDto.AccessToken, "access_token");

                tokenEntity = await _tokenStorage.GetAccessTokenAsync(userId, extTokenDto.ApiName);
                if (tokenEntity is null)
                {
                    tokenEntity = new ExternalToken
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId,
                        ApiName = extTokenDto.ApiName,
                        AccessToken = extTokenDto.AccessToken,
                        RefreshToken = extTokenDto.RefreshToken
                    };
                    _tokenStorage.SaveToken(tokenEntity);
                }
                else
                {
                    tokenEntity.AccessToken = extTokenDto.AccessToken;
                    tokenEntity.RefreshToken = extTokenDto.RefreshToken;
                    _tokenStorage.UpdateToken(tokenEntity);
                }

                await _unitOfWork.SaveChangesAsync();
                return Result<ExternalToken>.Success(tokenEntity);
            }
            catch (SecurityTokenException ex)
            {
                // token invalid or expired

                return Result<ExternalToken>.Failure($"Token invalid: {ex.Message}");
            }
            catch (Exception ex)
            {
                if (tokenEntity != null)
                {
                    _tokenStorage.RemoveToken(tokenEntity);
                }

                return Result<ExternalToken>.Failure(ex.Message);
            }
        }

        public async Task<Result<ExternalToken>> GetRefreshedToken(string refreshToken, string userId, string apiName)
        {
            try
            {
                string access_token = await _tokenProvider.HandleRefreshAsync(refreshToken);
                ExternalToken? tokenFromDb = await _tokenStorage.GetAccessTokenAsync(userId, apiName);
                ExternalTokenDto tokenObj = new()
                {
                    AccessToken = access_token, RefreshToken = refreshToken, ApiName = apiName
                };
                Result<ExternalToken> tokenResult = await SaveTokenAsync(userId, tokenObj);
                if (!tokenResult.IsSuccess || tokenResult.Data is null)
                {
                    throw new Exception(tokenResult.ErrorMessage);
                }

                return Result<ExternalToken>.Success(tokenResult.Data);
            }
            catch (Exception ex)
            {
                return Result<ExternalToken>.Failure($"An error occurred while refreshing token: {ex.Message}");
            }
        }

        public async Task<Result<bool>> RemoveTokenAsync(string userId, string apiName)
        {
            try
            {
                ExternalToken? tokenEntity = await _tokenStorage.GetAccessTokenAsync(userId, apiName);
                if (tokenEntity is null)
                {
                    throw new Exception("Token not found");
                }

                _tokenStorage.RemoveToken(tokenEntity);
                await _unitOfWork.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }
    }
}
