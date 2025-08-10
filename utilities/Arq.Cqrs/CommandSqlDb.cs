using Arq.Core;
using Arq.Cqrs.Interfaces;

namespace Arq.Cqrs
{
    public class CommandSqlDb<T>(IDbContextProvider dbContextProvider) : ICommandSqlDb<T> where T : class
    {

        public async Task<T> AddAsync(T entity, string dbChoice)
        {
            var ctx = dbContextProvider.GetDbContext(dbChoice);
            await ctx.Set<T>().AddAsync(entity);
            await ctx.SaveChangesAsync();
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, string dbChoice)
        {
            var ctx = dbContextProvider.GetDbContext(dbChoice);
            await ctx.Set<T>().AddRangeAsync(entities);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity, string dbChoice)
        {
            var ctx = dbContextProvider.GetDbContext(dbChoice);
            ctx.Set<T>().Update(entity);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity, string dbChoice)
        {
            var ctx = dbContextProvider.GetDbContext(dbChoice);
            ctx.Set<T>().Remove(entity);
            await ctx.SaveChangesAsync();
        }

        public async Task AddToTransactionAsync(T entity, string dbChoice)
        {
            var ctx = dbContextProvider.GetDbContext(dbChoice);
            await ctx.Set<T>().AddAsync(entity);
        }

        public async Task UpdateToTransactionAsync(T entity, string dbChoice)
        {
            var ctx = dbContextProvider.GetDbContext(dbChoice);
            ctx.Set<T>().Update(entity);
        }

        public async Task<int> SaveChangesAsync(string dbChoice)
        {
            var ctx = dbContextProvider.GetDbContext(dbChoice);
            return await ctx.SaveChangesAsync();
        }

    }
}
