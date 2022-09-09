using ECommerceDemoInfrastructure.Contracts;
using ECommerceDemoInfrastructure.DataProviders;

namespace ECommerceDemoInfrastructure.Factories
{
    public class DataProviderFactory<T> where T : IEntity
    {
        public static IDataProvider<T> CreateIntegratedDataProvider(string dataServiceUrl)
        {
            var clientDataServiceProvider = new ClientDataServiceProvider<T>(dataServiceUrl);

            var clientDataServiceCircuitBreakable = new ClientDataServiceCircuitBreakable<T>(clientDataServiceProvider, openToHalfOpenWaitTime: 100);

            var fileEntityProvider = new FileEntityProvider<T>($"{typeof(T).Name}s");

            var integratedDataProvider = new IntegratedDataServiceProvider<T>(
                                            clients: new IDataProvider<T>[] {
                                                            clientDataServiceCircuitBreakable,
                                                            fileEntityProvider // is obsolete - used as a backup monolyth client
                                            },
                                            retryNumber: 3,
                                            delayPeriodInMicroSeconds: 100);

            return integratedDataProvider;
        }
    }
}
