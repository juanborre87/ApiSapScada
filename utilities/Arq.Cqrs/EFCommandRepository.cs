using Arq.Core;
using Arq.Cqrs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arq.Cqrs;

public class EFCommandRepository<T> : IEFCommandRepository<T> where T : class
{
    private readonly IDbContextProvider _dbContextProvider;
    private readonly DbContext _boundContext;

    /// <summary>
    /// Creates a stateless repository that resolves DbContext by database choice.
    /// Crea un repositorio "stateless" que resuelve el DbContext según la base de datos elegida.
    /// </summary>
    public EFCommandRepository(IDbContextProvider dbContextProvider)
        => _dbContextProvider = dbContextProvider
        ?? throw new ArgumentNullException(nameof(dbContextProvider));

    /// <summary>
    /// Creates a repository bound to a specific DbContext (for UnitOfWork).
    /// Crea un repositorio ligado a un DbContext específico (para UnitOfWork).
    /// </summary>
    public EFCommandRepository(DbContext boundContext)
        => _boundContext = boundContext
        ?? throw new ArgumentNullException(nameof(boundContext));

    /// <summary>
    /// Resolves the appropriate DbContext based on mode (bound/stateless).
    /// Resuelve el DbContext apropiado según el modo (ligado/stateless).
    /// </summary>
    private DbContext ResolveContext(string dbChoice) =>
        _boundContext ?? _dbContextProvider?.GetDbContext(dbChoice)
        ?? throw new InvalidOperationException("No DbContext or IDbContextProvider available.");

    public async Task<int> SaveChangesAsync(string dbChoice)
    {
        var ctx = ResolveContext(dbChoice);
        return await ctx.SaveChangesAsync();
    }


    // ───────────────────────────────────────────────────────────────
    // Immediate Save Methods / Métodos con guardado inmediato
    // ───────────────────────────────────────────────────────────────

    public async Task<T> AddAsync(T entity, string dbChoice)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var ctx = ResolveContext(dbChoice);
        await ctx.Set<T>().AddAsync(entity);
        await ctx.SaveChangesAsync();
        return entity;
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, string dbChoice)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var ctx = ResolveContext(dbChoice);
        await ctx.Set<T>().AddRangeAsync(entities);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity, string dbChoice)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var ctx = ResolveContext(dbChoice);
        var entry = ctx.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            ctx.Set<T>().Attach(entity);
            entry.State = EntityState.Modified;
        }
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity, string dbChoice)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var ctx = ResolveContext(dbChoice);
        var entry = ctx.Entry(entity);
        if (entry.State == EntityState.Detached)
            ctx.Set<T>().Attach(entity);
        ctx.Set<T>().Remove(entity);
        await ctx.SaveChangesAsync();
    }

    // ───────────────────────────────────────────────────────────────
    // Transactional Methods (no SaveChanges) / Métodos transaccionales (sin guardar)
    // ───────────────────────────────────────────────────────────────

    public async Task AddToTransactionAsync(T entity, string dbChoice)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var ctx = ResolveContext(dbChoice);
        await ctx.Set<T>().AddAsync(entity);
    }

    public async Task AddRangeToTransactionAsync(IEnumerable<T> entities, string dbChoice)
    {
        ArgumentNullException.ThrowIfNull(entities);
        var ctx = ResolveContext(dbChoice);
        await ctx.Set<T>().AddRangeAsync(entities);
    }

    public async Task UpdateToTransactionAsync(T entity, string dbChoice)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var ctx = ResolveContext(dbChoice);
        var entry = ctx.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            ctx.Set<T>().Attach(entity);
            entry.State = EntityState.Modified;
        }
    }

    public async Task DeleteToTransactionAsync(T entity, string dbChoice)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var ctx = ResolveContext(dbChoice);
        var entry = ctx.Entry(entity);
        if (entry.State == EntityState.Detached)
            ctx.Set<T>().Attach(entity);
        ctx.Set<T>().Remove(entity);
    }

}
