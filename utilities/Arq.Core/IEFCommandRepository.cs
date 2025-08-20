namespace Arq.Core;

public interface IEFCommandRepository<T> where T : class
{
    /// <summary>
    /// Inserts a new entity into the database and saves the changes immediately.
    /// Inserta una nueva entidad en la base de datos y guarda los cambios inmediatamente.
    /// </summary>
    Task<T> AddAsync(T entity, string dbChoice);

    /// <summary>
    /// Deletes an existing entity from the database and saves the changes immediately.
    /// Elimina una entidad existente de la base de datos y guarda los cambios inmediatamente.
    /// </summary>
    Task DeleteAsync(T entity, string dbChoice);

    /// <summary>
    /// Updates an existing entity in the database and saves the changes immediately.
    /// Actualiza una entidad existente en la base de datos y guarda los cambios inmediatamente.
    /// </summary>
    Task UpdateAsync(T entity, string dbChoice);

    /// <summary>
    /// Inserts multiple entities into the database and saves the changes immediately.
    /// Inserta múltiples entidades en la base de datos y guarda los cambios inmediatamente.
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, string dbChoice);

    /// <summary>
    /// Inserts a new entity into an active transaction without saving immediately.
    /// Inserta una nueva entidad en una transacción activa sin guardar inmediatamente.
    /// </summary>
    Task AddToTransactionAsync(T entity, string dbChoice);

    /// <summary>
    /// Updates an entity within an active transaction without saving immediately.
    /// Actualiza una entidad dentro de una transacción activa sin guardar inmediatamente.
    /// </summary>
    Task UpdateToTransactionAsync(T entity, string dbChoice);

    /// <summary>
    /// Inserts multiple entities into an active transaction without saving immediately.
    /// Inserta múltiples entidades en una transacción activa sin guardar inmediatamente.
    /// </summary>
    Task AddRangeToTransactionAsync(IEnumerable<T> entities, string dbChoice);

    /// <summary>
    /// Saves all pending changes in the current context to the database.
    /// Guarda todos los cambios pendientes en el contexto actual en la base de datos.
    /// </summary>
    Task<int> SaveChangesAsync(string dbChoice);
}
