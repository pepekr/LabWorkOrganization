using AutoMapper;
using LabWorkOrganization.Domain.Intefaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients
{


    /// IMPORTANT: Maybe in adding method, id is not expected so it will give an error -> check and change mapping algorithm if needed
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
            TResponse requestDto = _mapper.Map<TResponse>(entity);
            var result = await _httpClient.PostAsJsonAsync(_baseUrl, requestDto, _jsonOptions);
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            var createdDto = JsonSerializer.Deserialize<TResponse>(json, _jsonOptions)!;
            return _mapper.Map<TEntity>(createdDto);
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
            return _mapper.Map<IEnumerable<TEntity>>(temp);
        }

        public async Task<TEntity?> GetByIdAsync(Guid externalId)
        {
            var res = await _httpClient.GetAsync($"{_baseUrl}/{externalId}");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var temp = JsonSerializer.Deserialize<TResponse>(json)!;
            return _mapper.Map<TEntity>(temp);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, Guid externalId)
        {
            var requestDto = _mapper.Map<TResponse>(entity);
            var result = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{externalId}", requestDto, _jsonOptions);
            result.EnsureSuccessStatusCode();
            return entity;
        }
    }
}
