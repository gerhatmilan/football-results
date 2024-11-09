using FootballResults.DataAccess.Entities;

namespace FootballResults.DataAccess.Repositories
{
    public interface IGenericRepository<T> where T : EntityWithID
    {
        Task<T> GetByIDAsync(int id, bool tracking);
        Task<IEnumerable<T>> GetAllAsync(bool tracking);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
