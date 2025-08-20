namespace Arq.Core;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Starts a new database transaction for the given database choice.
    /// Inicia una nueva transacción en la base de datos indicada.
    /// </summary>
    Task BeginTransactionAsync(string dbChoice);

    /// <summary>
    /// Commits changes to the database for the given context without necessarily committing the global transaction.
    /// Confirma los cambios realizados en la base de datos indicada sin necesidad de confirmar la transacción global.
    /// </summary>
    Task CommitAsync(string dbChoice);

    /// <summary>
    /// Commits the active transaction, persisting all changes across databases involved.
    /// Confirma la transacción activa, persistiendo todos los cambios en las bases de datos involucradas.
    /// </summary>
    Task CommitAllAsync();

    /// <summary>
    /// Rolls back all pending changes for the given database.
    /// Revierte los cambios pendientes en la base de datos indicada.
    /// </summary>
    Task RollbackAsync(string dbChoice);

    /// <summary>
    /// Rolls back all pending changes for all databases.
    /// Revierte los cambios pendientes en todas las bases de datos.
    /// </summary>
    Task RollbackAllAsync();

    /// <summary>
    /// Provides access to the EF Command Repository for performing write operations on the specified database.
    /// Provee acceso al Repositorio de Comandos EF para operaciones de escritura sobre la base de datos indicada.
    /// </summary>
    IEFCommandRepository<T> CommandRepository<T>(string dbChoice) where T : class;

    /// <summary>
    /// Provides access to the EF Query Repository for performing read operations on the specified database.
    /// Provee acceso al Repositorio de Consultas EF para operaciones de lectura sobre la base de datos indicada.
    /// </summary>
    IEFQueryRepository<T> QueryRepository<T>(string dbChoice) where T : class;

    /// <summary>
    /// Provides access to the Dapper repository for executing raw SQL or lightweight queries.
    /// Provee acceso al repositorio Dapper para ejecutar SQL crudo o consultas ligeras.
    /// </summary>
    IDapperRepository DapperRepository(string dbChoice);
}
