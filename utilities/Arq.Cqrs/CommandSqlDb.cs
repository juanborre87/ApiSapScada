using Arq.Core;
using Arq.Cqrs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Arq.Cqrs
{
    public class CommandSqlDb<T>(IDbContextProvider dbContextProvider) : ICommandSqlDb<T> where T : class
    {

        public async Task<T> AddAsync(T entity, string dbChoice)
        {
            try
            {
                var ctx = dbContextProvider.GetDbContext(dbChoice);
                await ctx.Set<T>().AddAsync(entity);
                await ctx.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }

        }

        public async Task AddRangeAsync(IEnumerable<T> entities, string dbChoice)
        {
            try
            {
                var ctx = dbContextProvider.GetDbContext(dbChoice);
                await ctx.Set<T>().AddRangeAsync(entities);
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }

        }

        public async Task UpdateAsync(T entity, string dbChoice)
        {
            try
            {
                var ctx = dbContextProvider.GetDbContext(dbChoice);
                ctx.Set<T>().Update(entity);
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }

        }

        public async Task DeleteAsync(T entity, string dbChoice)
        {
            try
            {
                var ctx = dbContextProvider.GetDbContext(dbChoice);
                ctx.Set<T>().Remove(entity);
                await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }

        }

        public async Task AddToTransactionAsync(T entity, string dbChoice)
        {
            try
            {
                var ctx = dbContextProvider.GetDbContext(dbChoice);
                await ctx.Set<T>().AddAsync(entity);
            }
            catch (Exception ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }

        }

        public async Task UpdateToTransactionAsync(T entity, string dbChoice)
        {
            try
            {
                var ctx = dbContextProvider.GetDbContext(dbChoice);
                ctx.Set<T>().Update(entity);
            }
            catch (Exception ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }

        }

        public async Task<int> SaveChangesAsync(string dbChoice)
        {
            try
            {
                var ctx = dbContextProvider.GetDbContext(dbChoice);
                return await ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var sqlMessage = ex.InnerException?.Message ?? ex.Message;
                throw new Exception(sqlMessage);
            }

        }

    }
}
