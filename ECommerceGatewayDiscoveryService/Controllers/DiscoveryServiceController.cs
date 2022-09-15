using ECommerceDemoInfrastructure.Models;
using ECommerceGatewayDiscoveryService.Contracts;
using ECommerceGatewayDiscoveryService.DataProviders;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ECommerceGatewayDiscoveryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscoveryServiceController : ControllerBase
    {
        private readonly IEndpointsProvider _endpointsProvider;

        public DiscoveryServiceController()
        {
            _endpointsProvider = new ServiceEndpointsProvider();
        }


        [HttpGet]
        [Route("Services")]
        public IEnumerable<ServiceEndpoint> Get()
        {
            return _endpointsProvider.GetEndpoints();
        }

        [HttpGet]
        [Route("{serviceName}")]
        public IEnumerable<string> Get(string serviceName)
        {
            return _endpointsProvider.GetServiceEndpoints(serviceName);
        }

        [HttpPost]
        public void Post([FromBody] ServiceEndpoint[] serviceEndpoints)
        {
            _endpointsProvider.AddEndpoints(serviceEndpoints);
        }

        [HttpDelete]
        [Route("{serviceName}")]
        public void Delete(string serviceName)
        {
            _endpointsProvider.DeleteServiceEndpoints(serviceName);
        }

        [HttpDelete]
        [Route("Clear")]
        public void Clear()
        {
            _endpointsProvider.Clear();
        }
    }
}
