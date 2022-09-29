using ECommerceDemoInfrastructure.Contracts;
using ECommerceDemoInfrastructure.DataProviders;
using ECommerceDemoInfrastructure.Helpers;
using System.Collections.Generic;
using System.Net.Http;

namespace ECommerceDemoInfrastructure.Factories
{
    public class DataProviderFactory<T> where T : IEntity
    {

        public static IDataProvider<T> CreateIntegratedDataProvider(HttpClient httpClient)
        {
            string[] serviceUrls = GatewayDiscoveryHelper.GetServiceUrls(httpClient);
            List<IDataProvider<T>> serviceClientsList = new List<IDataProvider<T>>();

            foreach (string url in serviceUrls)
            {
                var clientDataServiceProvider = new ClientDataServiceProvider<T>(httpClient, url);
                var clientDataServiceCircuitBreakable = new ClientDataServiceCircuitBreakable<T>(clientDataServiceProvider, openToHalfOpenWaitTime: 100);
                serviceClientsList.Add(clientDataServiceCircuitBreakable);
            }

            serviceClientsList.Add(new FileEntityProvider<T>($"{typeof(T).Name}s")); //// is obsolete - used as a backup monolyth client


            var integratedDataProvider = new IntegratedDataServiceProvider<T>(
                                            clients: serviceClientsList.ToArray(),
                                            retryNumber: 3,
                                            delayPeriodInMicroSeconds: 100);

            return integratedDataProvider;
        }

    }
}
