using Arq.Core;
using Microsoft.EntityFrameworkCore;

namespace Arq.Cqrs;

public class EFCommandRepository<T> : IEFCommandRepository<T> where T : class
{
    private readonly DbContext _dbContext;

    /// <summary>
    /// Creates a repository bound to a specific DbContext (for UnitOfWork).
    /// Crea un repositorio ligado a un DbContext específico (para UnitOfWork).
    /// </summary>
    public EFCommandRepository(DbContext dbContext)
        => _dbContext = dbContext
        ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<int> SaveChangesAsync()
        => await _dbContext.SaveChangesAsync();


    public async Task AddAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _dbContext.Set<T>().AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        await _dbContext.Set<T>().AddRangeAsync(entities);
    }

    public async Task UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = _dbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            _dbContext.Set<T>().Attach(entity);
            entry.State = EntityState.Modified;
        }
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);

        foreach (var entity in entities)
        {
            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Set<T>().Attach(entity);
                entry.State = EntityState.Modified;
            }
        }
    }

    public async Task DeleteAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var entry = _dbContext.Entry(entity);
        if (entry.State == EntityState.Detached)
            _dbContext.Set<T>().Attach(entity);
        _dbContext.Set<T>().Remove(entity);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        _dbContext.Set<T>().RemoveRange(entities);
    }
}
