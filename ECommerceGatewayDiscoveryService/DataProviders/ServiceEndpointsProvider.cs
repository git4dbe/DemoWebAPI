using ECommerceDemoInfrastructure.Contracts;
using ECommerceDemoInfrastructure.DataProviders;
using ECommerceDemoInfrastructure.Models;
using ECommerceGatewayDiscoveryService.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommerceGatewayDiscoveryService.DataProviders
{
    public class ServiceEndpointsProvider : IEndpointsProvider
    {
        private readonly IStringDataProvider _stringDataProvider;
        private readonly string _fileIdName = $"{nameof(ServiceEndpoint)}s";
        public ServiceEndpointsProvider()
        {
            _stringDataProvider = new FileStringProvider(nameof(ServiceEndpoint));
        }

        public void AddEndpoints(ServiceEndpoint[] serviceEndpoints)
        {
            List<ServiceEndpoint> endpoints = GetEndpoints().ToList();

            List<ServiceEndpoint> newEndpoints = serviceEndpoints.Where(
                                s => !endpoints.Any(e => e.ServiceName == s.ServiceName && e.ServiceUrl == s.ServiceUrl))
                                .ToList();

            if (!newEndpoints.Any()) return;

            endpoints.AddRange(newEndpoints);

            endpoints = endpoints.OrderBy(e => e.ServiceName)
                                 .ThenBy(e => e.ServiceUrl)
                                 .ToList();

            SaveEndpoints(endpoints);
        }

        public void Clear()
        {
            if (_stringDataProvider.IsNew(_fileIdName)) return;
            _stringDataProvider.Delete(_fileIdName);
        }

        public void DeleteServiceEndpoints(string serviceName)
        {
            List<ServiceEndpoint> endpoints = GetEndpoints().ToList();
            if (!endpoints.Any(e => e.ServiceName == serviceName)) return;

            endpoints = endpoints.Where(e => e.ServiceName != serviceName).ToList();

            SaveEndpoints(endpoints);
        }

        public IEnumerable<ServiceEndpoint> GetEndpoints()
        {
            string json = _stringDataProvider.Get(_fileIdName);
            if (string.IsNullOrEmpty(json))
            {
                return Array.Empty<ServiceEndpoint>();
            }

            List<ServiceEndpoint> endpoints = JsonConvert.DeserializeObject<List<ServiceEndpoint>>(json);
            return endpoints;
        }

        public IEnumerable<string> GetServiceEndpoints(string serviceName)
        {
            List<ServiceEndpoint> endpoints = GetEndpoints().ToList();

            string[] serviceEndpoints = endpoints.Where(e => e.ServiceName == serviceName)
                                                 .Select(e => e.ServiceUrl)
                                                 .ToArray();

            return serviceEndpoints;
        }

        private void SaveEndpoints(List<ServiceEndpoint> endpoints)
        {
            string json = JsonConvert.SerializeObject(endpoints, Formatting.Indented);

            if (_stringDataProvider.IsNew(_fileIdName))
            {
                _stringDataProvider.Add(_fileIdName, json);
            }
            else
            {
                _stringDataProvider.Update(_fileIdName, json);
            }
        }
    }
}
