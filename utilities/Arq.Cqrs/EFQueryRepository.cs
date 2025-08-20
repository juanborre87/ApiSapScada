using Arq.Core;
using Arq.Cqrs.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Arq.Cqrs;

public class EFQueryRepository<T> : IEFQueryRepository<T> where T : class
{
    private readonly IDbContextProvider _dbContextProvider;
    private readonly DbContext _boundContext;

    /// <summary>
    /// Creates a stateless repository that resolves DbContext by database choice.
    /// Crea un repositorio "stateless" que resuelve el DbContext según la base de datos elegida.
    /// </summary>
    public EFQueryRepository(IDbContextProvider dbContextProvider)
        => _dbContextProvider = dbContextProvider
        ?? throw new ArgumentNullException(nameof(dbContextProvider));

    /// <summary>
    /// Creates a repository bound to a specific DbContext (for UnitOfWork).
    /// Crea un repositorio ligado a un DbContext específico (para UnitOfWork).
    /// </summary>
    public EFQueryRepository(DbContext boundContext)
        => _boundContext = boundContext
        ?? throw new ArgumentNullException(nameof(boundContext));

    // ───────────────────────────────────────────────────────────────
    // Non-paging methods / Métodos sin paginación
    // ───────────────────────────────────────────────────────────────

    public async Task<T?> FirstOrDefaultAsync(
        string dbChoice,
        Expression<Func<T, bool>> expr,
        bool tracking = true)
    {
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().AsQueryable();
        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(expr);
    }

    public async Task<T?> FirstOrDefaultIncludeMultipleAsync(
        string dbChoice,
        Expression<Func<T, bool>> expr,
        bool tracking = true,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        if (include != null)
            query = ApplyIncludes(query, include);

        return await query.FirstOrDefaultAsync(expr);
    }

    public async Task<List<T>> WhereAsync(
        string dbChoice,
        Expression<Func<T, bool>> expr,
        bool tracking = true)
    {
        var context = _dbContextProvider.GetDbContext(dbChoice);

        var query = context.Set<T>().AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.Where(expr).ToListAsync();
    }

    public async Task<List<T>> WhereIncludeMultipleAsync(
        string dbChoice,
        Expression<Func<T, bool>> expr,
        bool tracking = true,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        if (include != null)
            query = ApplyIncludes(query, include);

        return await query.Where(expr).ToListAsync();
    }

    public IAsyncEnumerable<T> StreamWhereAsync(
        string dbChoice,
        Expression<Func<T, bool>> expr,
        bool tracking = true)
    {
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        return query.Where(expr).AsAsyncEnumerable();
    }

    public async Task<List<T>> ListAllAsync(
        string dbChoice,
        bool tracking = true)
    {
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    public async Task<List<T>> IncludeMultipleAsync(
        string dbChoice,
        bool tracking = true,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        if (include != null)
            query = ApplyIncludes(query, include);

        return await query.ToListAsync();
    }

    // ───────────────────────────────────────────────────────────────
    // Paging methods / Métodos con paginación
    // ───────────────────────────────────────────────────────────────

    public async Task<PagedResult<T>> ListAllPageAsync(
        string dbChoice,
        int pageNumber,
        int pageSize,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool tracking = true)
    {
        ValidatePagingParameters(pageNumber, pageSize);
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        var totalRecords = await query.CountAsync();

        if (orderBy != null)
            query = orderBy(query);

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            MetaData = BuildMetaData(pageNumber, pageSize, totalRecords),
            Data = data
        };
    }

    public async Task<PagedResult<T>> WherePagedAsync(
        string dbChoice,
        Expression<Func<T, bool>> expr,
        int pageNumber,
        int pageSize,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool tracking = true)
    {
        ValidatePagingParameters(pageNumber, pageSize);
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().AsQueryable();

        if (expr != null)
            query = query.Where(expr);

        if (!tracking)
            query = query.AsNoTracking();

        var totalRecords = await query.CountAsync();

        if (orderBy != null)
            query = orderBy(query);

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            MetaData = BuildMetaData(pageNumber, pageSize, totalRecords),
            Data = data
        };
    }

    public async Task<PagedResult<T>> WhereIncludeMultiplePagedAsync(
        string dbChoice,
        Expression<Func<T, bool>> expr,
        int pageNumber,
        int pageSize,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool tracking = true,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        ValidatePagingParameters(pageNumber, pageSize);
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().AsQueryable();

        if (expr != null)
            query = query.Where(expr);

        if (!tracking)
            query = query.AsNoTracking();

        if (include != null)
            query = ApplyIncludes(query, include);

        var totalRecords = await query.CountAsync();

        if (orderBy != null)
            query = orderBy(query);

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            MetaData = BuildMetaData(pageNumber, pageSize, totalRecords),
            Data = data
        };
    }

    public async Task<PagedResult<T>> FirstOrDefaultPageAsync(
        string dbChoice,
        Expression<Func<T, bool>> expr,
        bool tracking = true)
    {
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().AsQueryable();

        if (expr != null)
            query = query.Where(expr);

        if (!tracking)
            query = query.AsNoTracking();

        var totalRecords = await query.CountAsync();
        var data = new List<T>();

        var entity = await query.FirstOrDefaultAsync();
        if (entity != null)
            data.Add(entity);

        return new PagedResult<T>
        {
            MetaData = BuildMetaData(1, 1, totalRecords),
            Data = data
        };
    }

    public async Task<PagedResult<T>> FirstOrDefaultIncludeMultiplePagedAsync(
        string dbChoice,
        Expression<Func<T, bool>> expr,
        bool tracking = true,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().Where(expr);

        if (expr != null)
            query = query.Where(expr);

        if (!tracking)
            query = query.AsNoTracking();

        if (include != null)
            query = ApplyIncludes(query, include);

        var totalRecords = await query.CountAsync();
        var data = new List<T>();

        var entity = await query.FirstOrDefaultAsync();
        if (entity != null)
            data.Add(entity);

        return new PagedResult<T>
        {
            MetaData = BuildMetaData(1, 1, totalRecords),
            Data = data
        };
    }

    public async Task<PagedResult<T>> StreamWherePageAsync(
        string dbChoice,
        Expression<Func<T, bool>> expr,
        int pageNumber,
        int pageSize,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool tracking = true)
    {
        ValidatePagingParameters(pageNumber, pageSize);
        var context = _dbContextProvider.GetDbContext(dbChoice);
        var query = context.Set<T>().Where(expr);

        if (!tracking)
            query = query.AsNoTracking();

        var totalRecords = await query.CountAsync();

        if (orderBy != null)
            query = orderBy(query);

        var data = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            MetaData = BuildMetaData(pageNumber, pageSize, totalRecords),
            Data = data
        };
    }

    // ───────────────────────────────────────────────────────────────
    // Helpers
    // ───────────────────────────────────────────────────────────────

    private static IQueryable<T> ApplyIncludes(
        IQueryable<T> query,
        Func<IQueryable<T>, 
        IIncludableQueryable<T, object>>? include)
    {
        return include != null ? include(query) : query;
    }

    private static MetaData BuildMetaData(int pageNumber, int pageSize, int totalRecords)
    {
        return new MetaData
        {
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            HasNext = pageNumber * pageSize < totalRecords,
            HasPrevious = pageNumber > 1
        };
    }

    private static void ValidatePagingParameters(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "El número de página debe ser mayor o igual a 1.");

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "El tamaño de página debe ser mayor a 0.");
    }

}
