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
    public class ShoppingCartController : ControllerBase
    {
        private readonly IEntityManager<ShoppingCart> _shoppingCartManager;

        public ShoppingCartController()
        {
            _shoppingCartManager = new ShoppingCartManager(
                new FileEntityProvider<ShoppingCart>($"{nameof(ShoppingCart)}s"),
                new FileEntityProvider<Product>($"{nameof(Product)}s"));
        }


        [HttpGet]
        [Route("ShoppingCarts")]
        public async Task<IEnumerable<ShoppingCart>> Get()
        {
            List<ShoppingCart> shoppingCarts = await _shoppingCartManager.GetAsync();
            return shoppingCarts;
        }

        [HttpGet("{id}")]
        public async Task<ShoppingCart> Get(string id)
        {
            ShoppingCart shoppingCart = await _shoppingCartManager.GetAsync(id);
            return shoppingCart;
        }

        [HttpPost]
        public async Task Post([FromBody] ShoppingCart shoppingCart)
        {
            await _shoppingCartManager.AddAsync(shoppingCart);
        }

        [HttpPut]
        public async Task Put([FromBody] ShoppingCart shoppingCart)
        {
            await _shoppingCartManager.UpdateAsync(shoppingCart);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _shoppingCartManager.DeleteAsync(id);
        }
    }
}
