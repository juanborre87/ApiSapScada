using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IQuerySqlDB<T> where T : class
    {
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filterExpression, bool tracking = true);

        Task<T> FirstOrDefaultIncludeAsync(string navigationPropertyPath, Expression<Func<T, bool>> filter, bool tracking = true);

        Task<List<T>> WhereAsync(Expression<Func<T, bool>> filterExpression, bool tracking = true);

        Task<List<T>> WhereIncludeAsync(string navigationPropertyPath, Expression<Func<T, bool>> filter, bool tracking = true);

        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);

        IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath);
    }
}
