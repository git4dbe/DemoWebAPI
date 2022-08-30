using System.Collections.Generic;

namespace ECommerceDemoCommon.Contracts
{
    public interface IDataProvider<T> where T : IEntity
    {
        T Get(string id);

        IEnumerable<T> Get();

        void AddAsync(T entity);

        void Update(T entity);

        void Delete(string id);
    }


}
