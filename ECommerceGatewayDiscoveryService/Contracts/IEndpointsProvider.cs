using ECommerceDemoInfrastructure.Models;
using System.Collections.Generic;

namespace ECommerceGatewayDiscoveryService.Contracts
{
    public interface IEndpointsProvider
    {
        IEnumerable<ServiceEndpoint> GetEndpoints();
        IEnumerable<string> GetServiceEndpoints(string serviceName);
        void AddEndpoints(ServiceEndpoint[] serviceEndpoints);
        void DeleteServiceEndpoints(string serviceName);
        void Clear();
    }
}
