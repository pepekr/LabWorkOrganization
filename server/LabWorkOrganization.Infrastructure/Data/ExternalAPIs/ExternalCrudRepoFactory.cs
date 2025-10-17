using AutoMapper;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.dtos;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.dtos.LabWorkOrganization.Infrastructure.ExternalClients.Google.Dtos;

namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs
{
    internal class ExternalCrudRepoFactory : IExternalCrudRepoFactory
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly IExternalTokenProvider _tokenProvider;

        public ExternalCrudRepoFactory(HttpClient httpClient, IMapper mapper, IExternalTokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            _mapper = mapper;
            _tokenProvider = tokenProvider;
        }

        public IExternalCrudRepo<TEntity> Create<TEntity>(string baseUrl)
            where TEntity : class
        {
            var dtoType = GetDtoType(typeof(TEntity));
            Type clientType;
            if (typeof(IHasCourseId).IsAssignableFrom(typeof(TEntity)))
            {
                clientType = typeof(CourseScopedClient<,>).MakeGenericType(typeof(TEntity), dtoType);
            }
            else
            {
                clientType = typeof(ClassroomClient<,>).MakeGenericType(typeof(TEntity), dtoType);
            }

            var instance = Activator.CreateInstance(
                clientType,
                _httpClient,
                baseUrl,
                _mapper,
                _tokenProvider
            );

            if (instance is not IExternalCrudRepo<TEntity> repo)
                throw new InvalidOperationException($"Created type {clientType.Name} is not a valid IExternalCrudRepo<{typeof(TEntity).Name}>.");

            return repo;
        }

        private static Type GetDtoType(Type entityType)
        {
            if (entityType == typeof(Course)) return typeof(CourseClassroomDto);
            if (entityType == typeof(LabTask)) return typeof(LabWorkClassroomDto);

            throw new NotSupportedException($"No DTO mapping registered for entity: {entityType.Name}");
        }
    }
}
