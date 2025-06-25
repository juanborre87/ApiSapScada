using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class CommandSqlDB<T> : ICommandSqlDB<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _entity;

        public CommandSqlDB(ApplicationDbContext context)
        {
            _context = context;
            _entity = context.Set<T>();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            try
            {
                await _entity.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return entity;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            try
            {
                var entry = _context.Entry(entity);
                entry.State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public virtual async Task RemoveAsync(T entity)
        {
            try
            {
                var entry = _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            try
            {
                var entry = _context.Entry(entity);
                entry.State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
