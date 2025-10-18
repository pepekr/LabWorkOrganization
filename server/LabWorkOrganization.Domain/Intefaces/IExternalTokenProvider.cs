namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IExternalTokenProvider
    {
        Task<string> GetAccessTokenAsync();
        Task<string> HandleRefreshAsync(string refreshToken);
    }
}
