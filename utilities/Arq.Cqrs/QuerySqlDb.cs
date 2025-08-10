using Arq.Core;
using Arq.Cqrs.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Arq.Cqrs
{
    public class QuerySqlDb<T>(IDbContextProvider dbContextProvider) : IQuerySqlDb<T> where T : class
    {

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filterExpression, string dbChoice, bool tracking = true)
        {
            try
            {
                var _entity = GetDbSet(dbChoice);
                return tracking ? await _entity.FirstOrDefaultAsync(filterExpression) :
                    await _entity.AsNoTracking().FirstOrDefaultAsync(filterExpression);
            }
            catch (DbUpdateException ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }
            catch
            {
                throw;
            }
        }

        public virtual async Task<T> FirstOrDefaultIncludeAsync(string navigationPropertyPath, Expression<Func<T, bool>> filter, string dbChoice, bool tracking = true)
        {
            try
            {
                var _entity = GetDbSet(dbChoice);
                return tracking ? await _entity.Include(navigationPropertyPath).FirstOrDefaultAsync(filter) :
                    await _entity.Include(navigationPropertyPath).AsNoTracking().FirstOrDefaultAsync(filter);
            }
            catch (DbUpdateException ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }
            catch
            {
                throw;
            }
        }

        public virtual async Task<List<T>> WhereAsync(Expression<Func<T, bool>> filterExpression, string dbChoice, bool tracking = true)
        {
            try
            {
                var _entity = GetDbSet(dbChoice);
                return tracking ? await _entity.Where(filterExpression).ToListAsync() :
                    await _entity.AsNoTracking().Where(filterExpression).ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }
            catch
            {
                throw;
            }
        }

        public virtual async Task<List<T>> WhereIncludeAsync(string navigationPropertyPath, Expression<Func<T, bool>> filter, string dbChoice, bool tracking = true)
        {
            try
            {
                var _entity = GetDbSet(dbChoice);
                return tracking ? await _entity.Include(navigationPropertyPath).Where(filter).ToListAsync() :
                    await _entity.Include(navigationPropertyPath).AsNoTracking().Where(filter).ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }
            catch
            {
                throw;
            }
        }

        public virtual async Task<List<T>> IncludeAllAsync(string navigationPropertyPath, string dbChoice, bool tracking = true)
        {
            try
            {
                var _entity = GetDbSet(dbChoice);

                return tracking
                    ? await _entity.Include(navigationPropertyPath).ToListAsync()
                    : await _entity.Include(navigationPropertyPath).AsNoTracking().ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }
            catch
            {
                throw;
            }
        }

        public virtual async Task<List<T>> ListAllAsync(string dbChoice, bool tracking = true)
        {
            try
            {
                var _entity = GetDbSet(dbChoice);

                return tracking
                    ? await _entity.ToListAsync()
                    : await _entity.AsNoTracking().ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }
            catch
            {
                throw;
            }
        }

        private DbSet<T> GetDbSet(string dbChoice) => dbContextProvider.GetDbContext(dbChoice).Set<T>();
    }
}
