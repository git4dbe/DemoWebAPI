using ECommerceDemoInfrastructure.DataProviders;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ECommerceDataProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataProviderController : ControllerBase
    {
        [HttpGet]
        [Route("Entities")]
        public IEnumerable<object> Get(string entityType)
        {
            var dataProvider = new FileStringProvider(entityType);
            var entities = dataProvider.Get().ToList();
            return entities;
        }

        [HttpGet]
        public string Get(string entityType, string id)
        {
            var dataProvider = new FileStringProvider(entityType);
            string entity = dataProvider.Get(id);
            return entity;
        }

        [HttpPost]
        public void Post(string entityType, string id)
        {
            var dataProvider = new FileStringProvider(entityType);

            string entity = GetRequestBodyStringContent();

            dataProvider.Add(id, entity);
        }

        [HttpPut]
        public void Put(string entityType, string id)
        {
            var dataProvider = new FileStringProvider(entityType);

            string entity = GetRequestBodyStringContent();

            dataProvider.Update(id, entity);
        }

        [HttpDelete]
        public void Delete(string entityType, string id)
        {
            var dataProvider = new FileStringProvider(entityType);
            dataProvider.Delete(id);
        }

        private string GetRequestBodyStringContent()
        {
            using (StreamReader streamReader = new StreamReader(Request.Body))
            {
                return streamReader.ReadToEndAsync().Result;
            }
        }
    }
}
