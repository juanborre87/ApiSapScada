using System.Data;

namespace Arq.Core;

public interface IDapperRepository
{
    /// <summary>
    /// Executes a query and returns a sequence of elements of type <typeparamref name="T"/>.
    /// Ejecuta una consulta y devuelve una secuencia de elementos del tipo <typeparamref name="T"/>.
    /// </summary>
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);

    /// <summary>
    /// Executes a query and returns a single element of type <typeparamref name="T"/>.
    /// Throws an exception if no record or more than one record is returned.
    /// Ejecuta una consulta y devuelve un único elemento del tipo <typeparamref name="T"/>.
    /// Lanza una excepción si no se devuelve ningún registro o más de un registro.
    /// </summary>
    Task<T> QuerySingleAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);

    /// <summary>
    /// Executes a command (such as INSERT, UPDATE, DELETE) and returns the number of affected rows.
    /// Ejecuta un comando (como INSERT, UPDATE, DELETE) y devuelve el número de filas afectadas.
    /// </summary>
    Task<int> ExecuteAsync(string sql, object? param = null, CommandType commandType = CommandType.Text);

    /// <summary>
    /// Returns the underlying (open) connection for advanced operations.
    /// Devuelve la conexión subyacente (abierta) para operaciones avanzadas.
    /// </summary>
    IDbConnection GetOpenConnection();
}
