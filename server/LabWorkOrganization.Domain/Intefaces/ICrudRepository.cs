namespace LabWorkOrganization.Domain.Intefaces
{
    public interface ICrudRepository<TCreational, T>
        where TCreational : ICreationalDto
        where T : IFullDto
    {
        public Task<T> CreateAsync(TCreational dto);
        public Task<T> GetByIdAsync(Guid id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> UpdateAsync(T dto);
        public Task DeleteAsync(Guid id);
    }
}
