using LabWorkOrganization.Domain.Intefaces;

namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs.Clients
{
    public class ClassroomClient<TEntity> : IExternalCrudRepo<TEntity>
    {
        // need to implement call to the external API (classroom) and map to entity objects
        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
