using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceDemoWebAPI.Contracts
{
    public interface IEntityManager<T> where T : IEntity
    {
        Task<T> GetAsync(string id);
        Task<List<T>> GetAsync();

        Task AddAsync(T entity);
        Task UpdateAsync(T entity);

        Task DeleteAsync(string id);
    }
}
