using CircuitBreaker.Concrete;
using ECommerceDemoInfrastructure.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceDemoInfrastructure.DataProviders
{
    public class ClientDataServiceCircuitBreakable<T> : IDataProvider<T> where T : IEntity
    {
        private readonly IDataProvider<T> _client;

        private readonly CircuitBreakerState _circuitBreakerState;
        private CircuitBreakerAction _actionBreaker;
        private CircuitBreakerFunc<IEnumerable<T>> _functionBreaker;

        private CircuitBreakerAction ActionBreaker
        {
            get
            {
                _actionBreaker = _actionBreaker ?? new CircuitBreakerAction(_circuitBreakerState);
                return _actionBreaker;
            }
        }

        private CircuitBreakerFunc<IEnumerable<T>> FuncBreaker
        {
            get
            {
                _functionBreaker = _functionBreaker ?? new CircuitBreakerFunc<IEnumerable<T>>(_circuitBreakerState);
                return _functionBreaker;
            }
        }

        public ClientDataServiceCircuitBreakable(
            IDataProvider<T> client,
            int openToHalfOpenWaitTime)
        {
            _client = client;
            _circuitBreakerState = new CircuitBreakerState(openToHalfOpenWaitTime);
        }

        public void Add(T entity)
        {
            ActionBreaker.ExecuteAction(() => _client.Add(entity));
        }

        public void Delete(string id)
        {
            ActionBreaker.ExecuteAction(() => _client.Delete(id));
        }

        public T Get(string id)
        {
            var funcGet = new Func<IEnumerable<T>>(() => new[] { _client.Get(id) });
            return FuncBreaker.ExecuteFunc(() => funcGet()).FirstOrDefault();
        }

        public IEnumerable<T> Get()
        {
            return FuncBreaker.ExecuteFunc(() => _client.Get());
        }

        public void Update(T entity)
        {
            ActionBreaker.ExecuteAction(() => _client.Update(entity));
        }
    }
}
