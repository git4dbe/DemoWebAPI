using ECommerceDemoInfrastructure.DataProviders;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace ECommerceDemoInfrastructure.Helpers
{
    public class GatewayDiscoveryHelper
    {
        private static readonly string _dataProviderService = "DataProviderService";

        public static string[] GetServiceUrls(HttpClient httpClient)
        {
            var clientDiscoveryService = new ClientGatewayDiscoveryService(httpClient);
            return clientDiscoveryService.GetServiceUrls(GetGatewayDiscoveryServiceUrl());
        }


        public static string GetGatewayDiscoveryServiceUrl(string serviceName)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string discoveryService = config.GetValue<string>("GatewayDiscoveryService:Url");

            if (!string.IsNullOrEmpty(serviceName))
            {
                string slashExpression = discoveryService.EndsWith("/") ? "" : "/";
                discoveryService = $"{discoveryService}{slashExpression}{serviceName}";
            }

            return discoveryService;
        }

        public static string GetGatewayDiscoveryServiceUrl()
        {
            return GetGatewayDiscoveryServiceUrl(_dataProviderService);
        }

    }
}
