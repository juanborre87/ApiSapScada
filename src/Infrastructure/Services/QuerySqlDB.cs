using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Services
{
    public class QuerySqlDB<T> : IQuerySqlDB<T> where T : class
    {
        protected readonly DbSet<T> _entity;

        public QuerySqlDB(ApplicationDbContext context)
        {
            _entity = context.Set<T>();
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filterExpression, bool tracking = true)
        {
            try
            {
                return tracking ? await _entity.FirstOrDefaultAsync(filterExpression) :
                    await _entity.AsNoTracking().FirstOrDefaultAsync(filterExpression);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public virtual async Task<T> FirstOrDefaultIncludeAsync(string navigationPropertyPath, Expression<Func<T, bool>> filter, bool tracking = true)
        {
            try
            {
                return tracking ? await _entity.Include(navigationPropertyPath).FirstOrDefaultAsync(filter) : await _entity.Include(navigationPropertyPath).AsNoTracking().FirstOrDefaultAsync(filter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public virtual async Task<List<T>> WhereAsync(Expression<Func<T, bool>> filterExpression, bool tracking = true)
        {
            try
            {
                return tracking ? await _entity.Where(filterExpression).ToListAsync() :
                    await _entity.AsNoTracking().Where(filterExpression).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public virtual async Task<List<T>> WhereIncludeAsync(string navigationPropertyPath, Expression<Func<T, bool>> filter, bool tracking = true)
        {
            try
            {
                return tracking ? await _entity.Include(navigationPropertyPath).Where(filter).ToListAsync() : await _entity.Include(navigationPropertyPath).AsNoTracking().Where(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            return await _entity.AnyAsync(filter);
        }

        public virtual IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
        => _entity.Include(navigationPropertyPath);
    }
}
