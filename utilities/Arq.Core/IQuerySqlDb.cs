using System.Linq.Expressions;

namespace Arq.Core
{
    public interface IQuerySqlDb<T> where T : class
    {
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filterExpression, string dbChoice, bool tracking = true);
        Task<T> FirstOrDefaultIncludeAsync(string navigationPropertyPath, Expression<Func<T, bool>> filter, string dbChoice, bool tracking = true);
        Task<List<T>> WhereAsync(Expression<Func<T, bool>> filterExpression, string dbChoice, bool tracking = true);
        Task<List<T>> WhereIncludeAsync(string navigationPropertyPath, Expression<Func<T, bool>> filter, string dbChoice, bool tracking = true);
        Task<List<T>> IncludeAllAsync(string navigationPropertyPath, string dbChoice, bool tracking = true);
        Task<List<T>> ListAllAsync(string dbChoice, bool tracking = true);
    }
}
