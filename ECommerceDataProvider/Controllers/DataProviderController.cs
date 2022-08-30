using ECommerceDemoCommon.DataProviders;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceDataProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataProviderController : ControllerBase
    {
        [HttpGet]
        [Route("Entities")]
        public async Task<IEnumerable<object>> Get(string entityType)
        {
            var dataProvider = new FileStringProvider(entityType);
            var entities = await Task.Run(() => dataProvider.Get().ToList());
            return entities;
        }

        [HttpGet]
        public async Task<string> Get(string entityType, string id)
        {
            var dataProvider = new FileStringProvider(entityType);
            string entity = await Task.Run(() => dataProvider.Get(id));
            return entity;
        }

        [HttpPost]
        public async Task Post(string entityType, string id, [FromBody] object entity)
        {
            var dataProvider = new FileStringProvider(entityType);
            await Task.Run(() => dataProvider.Add(id, entity.ToString()));
        }

        [HttpPut]
        public async Task Put(string entityType, string id, [FromBody] object entity)
        {
            var dataProvider = new FileStringProvider(entityType);
            await Task.Run(() => dataProvider.Update(id, entity.ToString()));
        }

        [HttpDelete]
        public async Task Delete(string entityType, string id)
        {
            var dataProvider = new FileStringProvider(entityType);
            await Task.Run(() => dataProvider.Delete(id));
        }
    }
}
