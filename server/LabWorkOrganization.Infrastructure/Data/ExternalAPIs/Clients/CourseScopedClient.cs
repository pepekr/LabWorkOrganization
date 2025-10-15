using AutoMapper;
using LabWorkOrganization.Domain.Intefaces;
using System.Text.Json;

namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients
{
    public class CourseScopedClient<TEntity, TResponse> : ClassroomClient<TEntity, TResponse>, ICourseScopedExternalRepository<TEntity>
         where TEntity : class, IHasCourseId
    {
        public CourseScopedClient(string accessToken, HttpClient client, string baseUrl, IMapper mapper)
            : base(accessToken, client, baseUrl, mapper)
        {
        }

        public async Task<IEnumerable<TEntity>> GetAllByCourseIdAsync(Guid courseId)
        {
            // here might be errors dont know how url based in google api will watch later
            await EnsureAuthorizationHeader();
            var res = await _httpClient.GetAsync($"{_baseUrl}?courseId={courseId}");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var temp = JsonSerializer.Deserialize<IEnumerable<TResponse>>(json, _jsonOptions)!;
            return _mapper.Map<IEnumerable<TEntity>>(temp);
        }
    }
}
