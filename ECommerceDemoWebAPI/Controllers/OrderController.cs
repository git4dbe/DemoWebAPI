using ECommerceDemoWebAPI.Contracts;
using ECommerceDemoWebAPI.DataProviders;
using ECommerceDemoWebAPI.Entities;
using ECommerceDemoWebAPI.EntityManagers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
            _orderManager = new OrderManager(
                new FileEntityProvider<ShoppingCart>($"{nameof(ShoppingCart)}s"),
                new FileEntityProvider<Order>($"{nameof(Order)}s"),
                new FileEntityProvider<Product>($"{nameof(Product)}s"));
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
