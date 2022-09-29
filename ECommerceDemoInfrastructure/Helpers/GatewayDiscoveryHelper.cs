using ECommerceDemoInfrastructure.DataProviders;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace ECommerceDemoInfrastructure.Helpers
{
    public class GatewayDiscoveryHelper
    {
        private static readonly string _dataProviderService = "DataProviderService";
        private static string _gatewayDiscoveryServiceUrl;

        public static string[] GetServiceUrls(HttpClient httpClient)
        {
            var clientDiscoveryService = new ClientGatewayDiscoveryService(httpClient);
            return clientDiscoveryService.GetServiceUrls(GetGatewayDiscoveryServiceUrl(_dataProviderService));
        }


        public static string GetGatewayDiscoveryServiceUrl(string serviceName)
        {
            string discoveryService = GetGatewayDiscoveryServiceUrl();

            if (!string.IsNullOrEmpty(serviceName))
            {
                string slashExpression = discoveryService.EndsWith("/") ? "" : "/";
                discoveryService = $"{discoveryService}{slashExpression}{serviceName}";
            }

            return discoveryService;
        }

        public static string GetGatewayDiscoveryServiceUrl()
        {
            if (string.IsNullOrEmpty(_gatewayDiscoveryServiceUrl)) _gatewayDiscoveryServiceUrl = GetDefaultGatewayDiscoveryServiceUrl();

            return _gatewayDiscoveryServiceUrl;
        }

        public static void SetGatewayDiscoveryServiceUrl(string serviceUrl)
        {
            _gatewayDiscoveryServiceUrl = serviceUrl;
        }

        private static string GetDefaultGatewayDiscoveryServiceUrl()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            return config.GetValue<string>("GatewayDiscoveryService:Url");
        }
    }
}
