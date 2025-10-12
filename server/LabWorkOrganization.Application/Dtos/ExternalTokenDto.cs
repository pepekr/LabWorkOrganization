namespace LabWorkOrganization.Application.Dtos
{
    public class ExternalTokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ApiName { get; set; }
    }
}
