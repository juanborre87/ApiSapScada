namespace Arq.Core
{
    public interface ICommandSqlDb<T> where T : class
    {
        Task<T> AddAsync(T entity, string dbChoice);
        Task DeleteAsync(T entity, string dbChoice);
        Task UpdateAsync(T entity, string dbChoice);
        Task AddRangeAsync(IEnumerable<T> entities, string dbChoice);
        Task AddToTransactionAsync(T entity, string dbChoice);
        Task UpdateToTransactionAsync(T entity, string dbChoice);
        Task AddRangeToTransactionAsync(IEnumerable<T> entities, string dbChoice);
        Task<int> SaveChangesAsync(string dbChoice);
    }
}
