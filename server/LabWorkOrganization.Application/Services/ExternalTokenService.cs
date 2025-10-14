using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace LabWorkOrganization.Application.Services
{
    public class ExternalTokenService
    {
        private readonly IExternalTokenStorage _tokenStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExternalTokenValidation _tokenValidation;
        public ExternalTokenService(IExternalTokenStorage tokenStorage, IUnitOfWork IUnitOfWork, IExternalTokenValidation tokenValidation)
        {
            _tokenStorage = tokenStorage;
            _unitOfWork = IUnitOfWork;
            _tokenValidation = tokenValidation;
        }

        public async Task<Result<string>> GetAccessTokenFromDbAsync(Guid userId, string apiName)
        {
            try
            {
                var tokenEntity = await _tokenStorage.GetAccessTokenAsync(userId, apiName);
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
                if (tokenEntity.AccessToken is not null)
                {   // TODO:
                    // need to check if expiration is okay,
                    // if not issue new one but with class like tokenIssuer
                    await _tokenValidation.ValidateTokenAsync(tokenEntity.AccessToken);
                    return Result<string>.Success(tokenEntity.AccessToken);
                }
                throw new Exception("Access token was not found");
            }
            catch (SecurityTokenException ex)
            {
                // token invalid or expired
                return Result<string>.Failure($"Token invalid: {ex.Message}");
            }
            catch (Exception ex)
            {

                return Result<string>.Failure(ex.Message);
            }


        }

        public async Task<Result<ExternalToken>> SaveTokenAsync(Guid userId, ExternalTokenDto extTokenDto)
        {
            Domain.Entities.ExternalToken? tokenEntity = null; // declare here

            try
            {
                await _tokenValidation.ValidateTokenAsync(extTokenDto.AccessToken);
                await _tokenValidation.ValidateTokenAsync(extTokenDto.RefreshToken);

                tokenEntity = await _tokenStorage.GetAccessTokenAsync(userId, extTokenDto.ApiName);
                if (tokenEntity is null)
                {
                    tokenEntity = new Domain.Entities.ExternalToken
                    {
                        UserId = userId,
                        ApiName = extTokenDto.ApiName,
                        AccessToken = extTokenDto.AccessToken,
                        RefreshToken = extTokenDto.RefreshToken,
                    };
                    _tokenStorage.SaveToken(tokenEntity);
                }
                else
                {
                    tokenEntity.AccessToken = extTokenDto.AccessToken;
                    tokenEntity.RefreshToken = extTokenDto.RefreshToken;
                    _tokenStorage.SaveToken(tokenEntity);
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
                    _tokenStorage.RemoveToken(tokenEntity);

                return Result<ExternalToken>.Failure(ex.Message);
            }
        }

    }
}
