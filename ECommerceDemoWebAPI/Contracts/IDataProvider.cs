using System.Collections.Generic;

namespace ECommerceDemoWebAPI.Contracts
{
    public interface IDataProvider<T> where T : IEntity
    {
        T Get(string id);

        IEnumerable<T> Get();

        void Add(T entity);

        void Update(T entity);

        void Delete(string id);
    }
}
