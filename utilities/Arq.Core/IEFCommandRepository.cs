namespace Arq.Core;

public interface IEFCommandRepository<T> where T : class
{
    /// <summary>
    /// Inserts a new entity into an active transaction without saving immediately.
    /// Inserta una nueva entidad en una transacción activa sin guardar inmediatamente.
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    /// Updates an entity within an active transaction without saving immediately.
    /// Actualiza una entidad dentro de una transacción activa sin guardar inmediatamente.
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Inserts multiple entities into an active transaction without saving immediately.
    /// Inserta múltiples entidades en una transacción activa sin guardar inmediatamente.
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Delete an entity.
    /// Elimina una entidad.
    /// </summary>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Saves all pending changes in the current context to the database.
    /// Guarda todos los cambios pendientes en el contexto actual en la base de datos.
    /// </summary>
    Task<int> SaveChangesAsync();
}
