using ECommerceDemoCommon.Contracts;
using ECommerceDemoCommon.DataProviders;
using ECommerceDemoCommon.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        [ActivatorUtilitiesConstructor]
        public ProductController()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string dataProviderServiceUrl = config.GetValue<string>("DataProviderService:Url");

            _productDataProvider = new ClientDataServiceProvider<Product>(dataProviderServiceUrl);
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
            await Task.Run(() => _productDataProvider.AddAsync(product));
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
