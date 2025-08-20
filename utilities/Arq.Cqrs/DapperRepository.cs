using Arq.Core;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Arq.Cqrs;

public class DapperRepository : IDapperRepository
{
    private readonly DbContext _context;

    public DapperRepository(DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IDbConnection GetOpenConnection()
    {
        var conn = _context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            conn.Open();
        return conn;
    }

    private IDbTransaction? GetCurrentTransaction()
    {
        return _context.Database.CurrentTransaction?.GetDbTransaction();
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
    {
        using var conn = GetOpenConnection();
        var tx = GetCurrentTransaction();
        return await conn.QueryAsync<T>(sql, param, transaction: tx, commandType: commandType);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
    {
        using var conn = GetOpenConnection();
        var tx = GetCurrentTransaction();
        return await conn.QuerySingleAsync<T>(sql, param, transaction: tx, commandType: commandType);
    }

    public async Task<int> ExecuteAsync(string sql, object? param = null, CommandType commandType = CommandType.Text)
    {
        using var conn = GetOpenConnection();
        var tx = GetCurrentTransaction();
        return await conn.ExecuteAsync(sql, param, transaction: tx, commandType: commandType);
    }
}
