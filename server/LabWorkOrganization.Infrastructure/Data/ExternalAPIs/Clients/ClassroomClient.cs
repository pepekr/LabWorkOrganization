using AutoMapper;
using LabWorkOrganization.Domain.Intefaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients
{
    /// IMPORTANT: Maybe in adding method, id is not expected so it will give an error -> check and change mapping algorithm if needed
    public class ClassroomClient<TEntity, TResponse> : IExternalCrudRepo<TEntity>
        where TEntity : class
    {
        protected readonly string _baseUrl;
        protected readonly HttpClient _httpClient;
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
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                WriteIndented = false
            };
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await EnsureAuthorizationHeader();
            TResponse requestDto = _mapper.Map<TResponse>(entity);
            HttpResponseMessage result = await _httpClient.PostAsJsonAsync(_baseUrl, requestDto, _jsonOptions);
            if (!result.IsSuccessStatusCode)
            {
                string content = await result.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Request failed with status code {(int)result.StatusCode} ({result.ReasonPhrase}). " +
                    $"Response content: {content}"
                );
            }

            result.EnsureSuccessStatusCode();
            string json = await result.Content.ReadAsStringAsync();
            TResponse createdDto = JsonSerializer.Deserialize<TResponse>(json, _jsonOptions)!;
            return _mapper.Map<TEntity>(createdDto);
        }

        public async Task DeleteAsync(string externalId)
        {
            await EnsureAuthorizationHeader();
            HttpResponseMessage result = await _httpClient.DeleteAsync($"{_baseUrl}/{externalId}");
            result.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            await EnsureAuthorizationHeader();
            HttpResponseMessage res = await _httpClient.GetAsync(_baseUrl);
            res.EnsureSuccessStatusCode();
            string json = await res.Content.ReadAsStringAsync();
            IEnumerable<TResponse>? temp = JsonSerializer.Deserialize<IEnumerable<TResponse>>(json, _jsonOptions)!;
            return _mapper.Map<IEnumerable<TEntity>>(temp);
        }

        public async Task<TEntity?> GetByIdAsync(string externalId)
        {
            await EnsureAuthorizationHeader();
            HttpResponseMessage res = await _httpClient.GetAsync($"{_baseUrl}/{externalId}");
            res.EnsureSuccessStatusCode();
            string json = await res.Content.ReadAsStringAsync();
            TResponse temp = JsonSerializer.Deserialize<TResponse>(json, _jsonOptions)!;
            return _mapper.Map<TEntity>(temp);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, string externalId)
        {
            await EnsureAuthorizationHeader();
            TResponse? requestDto = _mapper.Map<TResponse>(entity);
            HttpResponseMessage result =
                await _httpClient.PutAsJsonAsync($"{_baseUrl}/{externalId}", requestDto, _jsonOptions);
            result.EnsureSuccessStatusCode();
            return entity;
        }

        protected async Task EnsureAuthorizationHeader()
        {
            string token = await _tokenProvider.GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
