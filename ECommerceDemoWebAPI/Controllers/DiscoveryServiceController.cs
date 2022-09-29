using ECommerceDemoInfrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ECommerceDemoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscoveryServiceController : ControllerBase
    {
        [HttpGet]
        public string GetDiscoveryServiceUrl()
        {
            return GatewayDiscoveryHelper.GetGatewayDiscoveryServiceUrl();
        }

        [HttpPost]
        public void SetDiscoveryServiceUrl(string serviceUrl)
        {
            GatewayDiscoveryHelper.SetGatewayDiscoveryServiceUrl(serviceUrl);
        }

    }
}
