using Arq.Core;
using Arq.Cqrs.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;

namespace Arq.Cqrs
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IDbContextProvider _dbContextProvider;
        private readonly Dictionary<string, IDbContextTransaction> _transactions = new();
        private readonly ConcurrentDictionary<Type, object> _repos = new();

        public UnitOfWork(IDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task BeginTransactionAsync(string databaseChoice)
        {
            var ctx = _dbContextProvider.GetDbContext(databaseChoice);
            if (!_transactions.ContainsKey(databaseChoice))
            {
                var tx = await ctx.Database.BeginTransactionAsync();
                _transactions[databaseChoice] = tx;
            }
        }

        public async Task CommitAsync()
        {
            foreach (var kv in _transactions)
            {
                var dbChoice = kv.Key;
                var tx = kv.Value;
                await _dbContextProvider.GetDbContext(dbChoice).SaveChangesAsync();
                await tx.CommitAsync();
                await tx.DisposeAsync();
            }
            _transactions.Clear();
        }

        public async Task RollbackAsync()
        {
            foreach (var kv in _transactions)
            {
                var tx = kv.Value;
                await tx.RollbackAsync();
                await tx.DisposeAsync();
            }
            _transactions.Clear();
        }

        public ICommandSqlDb<T> Repository<T>() where T : class
        {
            var t = typeof(T);
            if (!_repos.ContainsKey(t))
            {
                var repo = new CommandSqlDb<T>(_dbContextProvider);
                _repos[t] = repo;
            }
            return (ICommandSqlDb<T>)_repos[t];
        }

        public void Dispose()
        {
            foreach (var kv in _transactions)
            {
                try { kv.Value.Dispose(); } catch { }
            }
            _transactions.Clear();

        }
    }
}
