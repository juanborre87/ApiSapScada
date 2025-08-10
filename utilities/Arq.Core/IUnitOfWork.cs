namespace Arq.Core
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync(string dbChoice);
        Task CommitAsync();
        Task RollbackAsync();
        ICommandSqlDb<T> Repository<T>() where T : class;
    }
}
