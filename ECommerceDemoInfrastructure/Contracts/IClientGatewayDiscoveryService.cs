
namespace ECommerceDemoInfrastructure.Contracts
{
    public interface IClientGatewayDiscoveryService
    {
        string[] GetServiceUrls(string discoveryUrl);
    }
}
