using FootballResults.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FootballResults.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : Entity
    {
        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public virtual async Task<T> GetByIDAsync(int id, bool tracking)
        {
            return await _dbSet
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .FirstAsync(e => e.ID == id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(bool tracking)
        {
            return await _dbSet
                .AsTracking(tracking ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking)
                .ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            var entityEntry = _dbSet.Add(entity);
            await _dbContext.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            var existingEntity = await _dbSet.FindAsync(entity.ID);

            if (existingEntity != null)
            {
                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            }

            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
