namespace Application.Interfaces;

public interface ICommandSqlDB<T> where T : class
{
    Task<T> AddAsync(T entity);

    Task DeleteAsync(T entity);

    Task UpdateAsync(T entity);

    Task RemoveAsync(T entity);

}
