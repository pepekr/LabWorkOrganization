namespace LabWorkOrganization.Domain.Intefaces
{
    public interface IUnitOfWork
    {
        public Task<int> SaveChangesAsync();
    }
}
