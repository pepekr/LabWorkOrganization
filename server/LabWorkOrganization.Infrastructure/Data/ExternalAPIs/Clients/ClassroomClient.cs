using AutoMapper;
using LabWorkOrganization.Domain.Intefaces;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients
{


    /// IMPORTANT: Maybe in adding method, id is not expected so it will give an error -> check and change mapping algorithm if needed
    public class ClassroomClient<TEntity, TResponse> : IExternalCrudRepo<TEntity>
        where TEntity : class
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;
        protected readonly JsonSerializerOptions _jsonOptions;
        protected readonly IMapper _mapper;
        protected readonly IExternalTokenProvider _tokenProvider;
        public ClassroomClient(HttpClient client, string baseUrl, IMapper mapper, IExternalTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
            _httpClient = client;
            _baseUrl = baseUrl;
            _mapper = mapper;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
    {
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
    },
                WriteIndented = false
            };
        }
        protected async Task EnsureAuthorizationHeader()
        {
            var token = await _tokenProvider.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await EnsureAuthorizationHeader();
            TResponse requestDto = _mapper.Map<TResponse>(entity);
            var result = await _httpClient.PostAsJsonAsync(_baseUrl, requestDto, _jsonOptions);
            if (!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Request failed with status code {(int)result.StatusCode} ({result.ReasonPhrase}). " +
                    $"Response content: {content}"
                );
            }
            result.EnsureSuccessStatusCode();
            var json = await result.Content.ReadAsStringAsync();
            var createdDto = JsonSerializer.Deserialize<TResponse>(json, _jsonOptions)!;
            return _mapper.Map<TEntity>(createdDto);
        }

        public async Task DeleteAsync(Guid externalId)
        {
            await EnsureAuthorizationHeader();
            var result = await _httpClient.DeleteAsync($"{_baseUrl}/{externalId}");
            result.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            await EnsureAuthorizationHeader();
            var res = await _httpClient.GetAsync(_baseUrl);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var temp = JsonSerializer.Deserialize<IEnumerable<TResponse>>(json)!;
            return _mapper.Map<IEnumerable<TEntity>>(temp);
        }

        public async Task<TEntity?> GetByIdAsync(Guid externalId)
        {
            await EnsureAuthorizationHeader();
            var res = await _httpClient.GetAsync($"{_baseUrl}/{externalId}");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var temp = JsonSerializer.Deserialize<TResponse>(json)!;
            return _mapper.Map<TEntity>(temp);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, Guid externalId)
        {
            await EnsureAuthorizationHeader();
            var requestDto = _mapper.Map<TResponse>(entity);
            var result = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{externalId}", requestDto, _jsonOptions);
            result.EnsureSuccessStatusCode();
            return entity;
        }
    }
}
