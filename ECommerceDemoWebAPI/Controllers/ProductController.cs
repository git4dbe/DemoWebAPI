using ECommerceDemoInfrastructure.Contracts;
using ECommerceDemoInfrastructure.Entities;
using ECommerceDemoInfrastructure.Factories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ECommerceDemoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IDataProvider<Product> _productDataProvider;

        [ActivatorUtilitiesConstructor]
        public ProductController()
        {
            var httpClient = new HttpClient(); //TODO use IHttpClientFactory for httpClient creation
            _productDataProvider = DataProviderFactory<Product>.CreateIntegratedDataProvider(httpClient);
        }

        public ProductController(IDataProvider<Product> productDataProvider)
        {
            _productDataProvider = productDataProvider;
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
