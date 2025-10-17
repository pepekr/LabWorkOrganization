namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IExternalCrudRepoFactory
    {
        IExternalCrudRepo<TEntity> Create<TEntity>(string baseUrl) where TEntity : class;
    }
}
