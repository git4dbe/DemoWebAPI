using System.Collections.Generic;

namespace ECommerceDemoInfrastructure.Contracts
{
    public interface IStringDataProvider
    {
        string Get(string id);

        IEnumerable<string> Get();

        void Add(string id, string entity);

        void Update(string id, string entity);

        void Delete(string id);

        bool IsNew(string id);
    }
}
