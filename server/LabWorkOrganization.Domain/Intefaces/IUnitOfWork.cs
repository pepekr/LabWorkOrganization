namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
