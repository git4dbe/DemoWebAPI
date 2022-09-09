using ECommerceDemoInfrastructure.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceDemoInfrastructure.DataProviders
{
    public class IntegratedDataServiceProvider<T> : IDataProvider<T> where T : IEntity
    {
        private readonly IEnumerable<IDataProvider<T>> _clients;
        private readonly int _retryNumber;
        private readonly int _delayPeriodInMicroSeconds;
        private readonly ILogger _logger;

        public IntegratedDataServiceProvider(
            IEnumerable<IDataProvider<T>> clients,
            int retryNumber,
            int delayPeriodInMicroSeconds,
            ILogger logger = null
            )
        {
            _clients = clients;
            _retryNumber = retryNumber;
            _delayPeriodInMicroSeconds = delayPeriodInMicroSeconds;
            _logger = logger;
        }
        public void Add(T entity)
        {
            List<Action> clientAddActions = _clients.Select(c => new Action(() => c.Add(entity)))
                                                 .ToList();

            ProcessActions(clientAddActions, "Add");
        }

        public void Delete(string id)
        {
            List<Action> clientDeleteActions = _clients.Select(c => new Action(() => c.Delete(id)))
                                                 .ToList();

            ProcessActions(clientDeleteActions, $"Delete {typeof(T).Name} by id={id}");
        }

        public T Get(string id)
        {
            List<Func<IEnumerable<T>>> clientGetFuncs = _clients.Select(c => new Func<IEnumerable<T>>(() => new[] { c.Get(id) }))
                                                                       .ToList();

            T result = ProcessFuncs(clientGetFuncs, $"Get {typeof(T).Name} by id={id}").FirstOrDefault();

            return result;
        }

        public IEnumerable<T> Get()
        {
            List<Func<IEnumerable<T>>> clientGetListFuncs = _clients.Select(c => new Func<IEnumerable<T>>(() => c.Get()))
                                                                       .ToList();

            return ProcessFuncs(clientGetListFuncs, $"Get {typeof(T).Name}s list");
        }

        public void Update(T entity)
        {
            List<Action> clientUpdateActions = _clients.Select(c => new Action(() => c.Update(entity)))
                                                 .ToList();

            ProcessActions(clientUpdateActions, $"Update {typeof(T).Name} with id={entity.Id}");
        }

        private void ProcessActions(List<Action> clientActions, string actionName)
        {

            for (int i = 1; i <= _retryNumber; i++)
            {
                for (int j = 0; j < clientActions.Count; j++)
                {
                    try
                    {
                        clientActions[j]();
                        return;
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError($"Execution of {actionName} is failed, client{j}, attempt {i}", ex);
                    }
                }

                Task.Delay(_delayPeriodInMicroSeconds).Wait();
            }

            throw new Exception($"Execution of {actionName} is failed for {_retryNumber} retry attempts on {clientActions.Count} clients");
        }


        private IEnumerable<T> ProcessFuncs(List<Func<IEnumerable<T>>> clientFuncs, string funcName)
        {
            for (int i = 1; i <= _retryNumber; i++)
            {
                for (int j = 0; j < clientFuncs.Count; j++)
                {
                    try
                    {
                        return clientFuncs[j]();
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError($"Execution of {funcName} is failed, client{j}, attempt {i}", ex);
                    }
                }

                Task.Delay(_delayPeriodInMicroSeconds).Wait();
            }

            throw new Exception($"Execution of {funcName} is failed for {_retryNumber} retry attempts on {clientFuncs.Count} clients");
        }
    }
}
