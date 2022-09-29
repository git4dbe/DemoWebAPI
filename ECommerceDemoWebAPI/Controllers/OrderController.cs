using ECommerceDemoInfrastructure.Contracts;
using ECommerceDemoInfrastructure.Entities;
using ECommerceDemoInfrastructure.Factories;
using ECommerceDemoWebAPI.EntityManagers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ECommerceDemoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IEntityManager<Order> _orderManager;

        public OrderController()
        {
            var httpClient = new HttpClient(); //TODO use IHttpClientFactory for httpClient creation

            _orderManager = new OrderManager(
                DataProviderFactory<ShoppingCart>.CreateIntegratedDataProvider(httpClient),
                DataProviderFactory<Order>.CreateIntegratedDataProvider(httpClient),
                DataProviderFactory<Product>.CreateIntegratedDataProvider(httpClient));
        }

        [HttpGet]
        [Route("Orders")]
        public async Task<IEnumerable<Order>> Get()
        {
            List<Order> orders = await _orderManager.GetAsync();
            return orders;
        }

        [HttpGet("{id}")]
        public async Task<Order> Get(string id)
        {
            Order order = await _orderManager.GetAsync(id);
            return order;
        }

        [HttpPost]
        public async Task Post([FromBody] Order order)
        {
            await _orderManager.AddAsync(order);
        }

        [HttpPut]
        public async Task Put([FromBody] Order order)
        {
            await _orderManager.UpdateAsync(order);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _orderManager.DeleteAsync(id);
        }

    }
}
