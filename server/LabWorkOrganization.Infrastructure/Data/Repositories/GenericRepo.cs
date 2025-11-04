using LabWorkOrganization.Domain.Intefaces;
using LabWorkOrganization.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class GenericRepo<TEntity> : ICrudRepository<TEntity>
    where TEntity : class
{
    private readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public GenericRepo(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<TEntity?> GetByIdAsync(
        string id,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        foreach (Expression<Func<TEntity, object>> include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(e => EF.Property<string>(e, "Id") == id);
    }


    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
}
