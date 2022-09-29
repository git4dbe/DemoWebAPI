using ECommerceDemoInfrastructure.Contracts;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ECommerceDemoInfrastructure.DataProviders
{
    public class ClientGatewayDiscoveryService : IClientGatewayDiscoveryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        public ClientGatewayDiscoveryService(HttpClient httpClient, ILogger logger = null)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public string[] GetServiceUrls(string discoveryUrl)
        {
            var result = _httpClient.GetAsync(discoveryUrl).Result;

            if (!result.IsSuccessStatusCode)
            {
                _logger?.Error($"Discovery Service '{discoveryUrl}' failed with status code - {result.StatusCode}");
                return Array.Empty<string>();
            }

            string json = result.Content.ReadAsStringAsync().Result;
            string[] serviceUrls = JsonConvert.DeserializeObject<List<string>>(json).ToArray();

            return serviceUrls;
        }
    }
}
