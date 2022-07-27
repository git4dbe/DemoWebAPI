using ECommerceDemoWebAPI.Contracts;
using ECommerceDemoWebAPI.DataProviders;
using ECommerceDemoWebAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceDemoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IDataProvider<Product> _productDataProvider;

        public ProductController()
        {
            _productDataProvider = new FileEntityProvider<Product>($"{nameof(Product)}s");
        }

        [HttpGet]
        [Route("Products")]
        public async Task<IEnumerable<Product>> Get()
        {
            List<Product> products = await Task.Run(() => _productDataProvider.Get().ToList());
            return products;
        }

        [HttpGet("{id}")]
        public async Task<Product> Get(string id)
        {
            Product product = await Task.Run(() => _productDataProvider.Get(id));
            return product;
        }

        [HttpPost]
        public async Task Post([FromBody] Product product)
        {
            await Task.Run(() => _productDataProvider.Add(product));
        }

        [HttpPut]
        public async Task Put([FromBody] Product product)
        {
            await Task.Run(() => _productDataProvider.Update(product));
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await Task.Run(() => _productDataProvider.Delete(id));
        }
    }
}
