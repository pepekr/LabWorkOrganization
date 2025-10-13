using AutoMapper;
using LabWorkOrganization.Domain.Intefaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients
{
    public class ClassroomClient<TEntity, TResponse> : IExternalCrudRepo<TEntity>
        where TEntity : class
    {
        private readonly string _accessToken;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IMapper _mapper;
        public ClassroomClient(string accessToken, HttpClient client, string baseUrl, IMapper mapper)
        {
            _httpClient = client;
            _accessToken = accessToken;
            _baseUrl = baseUrl;
            _mapper = mapper;
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            //map to request dto
            TResponse requestDto = (TResponse)(object)entity;
            var result = await _httpClient.PostAsJsonAsync(_baseUrl, requestDto, _jsonOptions);
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            var createdDto = JsonSerializer.Deserialize<TResponse>(json, _jsonOptions)!;
            return (TEntity)(object)createdDto;
        }

        public async Task DeleteAsync(Guid externalId)
        {
            var result = await _httpClient.DeleteAsync($"{_baseUrl}/{externalId}");
            result.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var res = await _httpClient.GetAsync(_baseUrl);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var temp = JsonSerializer.Deserialize<IEnumerable<TResponse>>(json)!;
            return (IEnumerable<TEntity>)(object)temp;
        }

        public async Task<TEntity?> GetByIdAsync(Guid externalId)
        {
            var res = await _httpClient.GetAsync($"{_baseUrl}/{externalId}");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var temp = JsonSerializer.Deserialize<TResponse>(json)!;
            return (TEntity)(object)temp;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, Guid externalId)
        {
            var requestDto = (TResponse)(object)entity;
            var result = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{externalId}", requestDto, _jsonOptions);
            result.EnsureSuccessStatusCode();
            return entity;
        }
    }
}
