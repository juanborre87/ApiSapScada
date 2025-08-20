using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Arq.Core;

public interface IEFQueryRepository<T> where T : class
{
    // ───────────────────────────────────────────────────────────────
    // Non-paging methods / Métodos sin paginación
    // ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the first entity that matches the built filter or null if none is found.
    /// Devuelve la primera entidad que coincide con el filtro construido o null si no se encuentra ninguna.
    /// </summary>
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> expr,
        bool tracking = true);

    /// <summary>
    /// Returns the first entity that matches the built filter, including related entities via Include/ThenInclude chain.
    /// Devuelve la primera entidad que coincide con el filtro, incluyendo entidades relacionadas mediante Include/ThenInclude.
    /// </summary>
    Task<T?> FirstOrDefaultIncludeMultipleAsync(
        Expression<Func<T, bool>> expr,
        bool tracking = true,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    /// <summary>
    /// Returns a list of entities matching the built filter.
    /// Devuelve una lista de entidades que coinciden con el filtro construido.
    /// </summary>
    Task<List<T>> WhereAsync(
        Expression<Func<T, bool>> expr,
        bool tracking = true);

    /// <summary>
    /// Returns a list of entities matching the built filter, including related entities via Include/ThenInclude chain.
    /// Devuelve una lista de entidades que coinciden con el filtro construido, incluyendo entidades relacionadas con Include/ThenInclude.
    /// </summary>
    Task<List<T>> WhereIncludeMultipleAsync(
        Expression<Func<T, bool>> expr,
        bool tracking = true,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    /// <summary>
    /// Streams entities matching the built filter without loading all into memory.
    /// Transmite entidades que coinciden con el filtro construido sin cargarlas todas en memoria.
    /// </summary>
    IAsyncEnumerable<T> StreamWhereAsync(
        Expression<Func<T, bool>> expr,
        bool tracking = true);

    /// <summary>
    /// Returns all entities.
    /// Devuelve todas las entidades.
    /// </summary>
    Task<List<T>> ListAllAsync(
        bool tracking = true);

    /// <summary>
    /// Returns all entities including related entities via Include/ThenInclude chain.
    /// Devuelve todas las entidades incluyendo relacionadas con Include/ThenInclude.
    /// </summary>
    Task<List<T>> IncludeMultipleAsync(
        bool tracking = true,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    // ───────────────────────────────────────────────────────────────
    // Paging methods / Métodos con paginación
    // ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns a paged list of entities matching the built filter, with optional ordering.
    /// Devuelve una lista paginada de entidades que coinciden con el filtro construido, con ordenamiento opcional.
    /// </summary>
    Task<PagedResult<T>> WherePagedAsync(
        Expression<Func<T, bool>> expr,
        int pageNumber,
        int pageSize,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool tracking = true);

    /// <summary>
    /// Returns a paged list matching the built filter, including related entities via Include/ThenInclude chain and optional ordering.
    /// Devuelve una lista paginada que coincide con el filtro, incluyendo relacionadas con Include/ThenInclude y orden opcional.
    /// </summary>
    Task<PagedResult<T>> WhereIncludeMultiplePagedAsync(
        Expression<Func<T, bool>> expr,
        int pageNumber,
        int pageSize,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool tracking = true,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    /// <summary>
    /// Returns the first entity in a paged shape (metadata included), without includes.
    /// Devuelve la primera entidad en forma paginada (con metadata), sin includes.
    /// </summary>
    Task<PagedResult<T>> FirstOrDefaultPageAsync(
        Expression<Func<T, bool>> expr,
        bool tracking = true);

    /// <summary>
    /// Returns one entity (with includes) wrapped in a PagedResult for frontend metadata needs.
    /// Devuelve una entidad (con includes) envuelta en PagedResult para necesidades de metadata del frontend.
    /// </summary>
    Task<PagedResult<T>> FirstOrDefaultIncludeMultiplePagedAsync(
        Expression<Func<T, bool>> expr,
        bool tracking = true,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

    /// <summary>
    /// Returns all entities in a paged result with optional ordering.
    /// Devuelve todas las entidades en un resultado paginado con orden opcional.
    /// </summary>
    Task<PagedResult<T>> ListAllPageAsync(
        int pageNumber,
        int pageSize,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool tracking = true);

    /// <summary>
    /// Returns a paged result for a filtered query. Named 'Stream' to mirror the non-paged method, but returns a single page (not a live stream) to include metadata.
    /// Devuelve un resultado paginado para una consulta filtrada. Se llama 'Stream' para reflejar el método sin paginación, pero retorna una página (no un stream) para incluir metadata.
    /// </summary>
    Task<PagedResult<T>> StreamWherePageAsync(
        Expression<Func<T, bool>> expr,
        int pageNumber,
        int pageSize,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool tracking = true);
}
