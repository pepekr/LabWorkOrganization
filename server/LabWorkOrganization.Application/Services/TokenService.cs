using LabWorkOrganization.Application.Dtos;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Domain.Utilities;

namespace LabWorkOrganization.Application.Services
{
    public class TokenService
    {
        private readonly ITokenStorage _tokenStorage;
        private readonly IUnitOfWork _unitOfWork;
        public TokenService(ITokenStorage tokenStorage, IUnitOfWork IUnitOfWork)
        {
            _tokenStorage = tokenStorage;
            _unitOfWork = IUnitOfWork;
        }

        public async Task<Result<string>> GetAccessTokenAsync(Guid userId, string apiName)
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
                    return Result<string>.Success(tokenEntity.AccessToken);
                }
                throw new Exception("Access token was not found");
            }
            catch (Exception ex)
            {

                return Result<string>.Failure(ex.Message);
            }


        }

        public async Task<Result<ExternalToken>> SaveTokenAsync(Guid userId, ExternalTokenDto extTokenDto)
        {
            // TODO: parse before saving
            try
            {
                var tokenEntity = await _tokenStorage.GetAccessTokenAsync(userId, extTokenDto.ApiName);
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
            catch (Exception ex)
            {

                return Result<ExternalToken>.Failure(ex.Message);
            }

        }
    }
}
