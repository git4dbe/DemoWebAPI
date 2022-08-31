using ECommerceDemoInfrastructure.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ECommerceDemoInfrastructure.DataProviders
{
    public class ClientDataServiceProvider<T> : IDataProvider<T> where T : IEntity
    {
        private readonly string _dataServiceUrl;
        private readonly HttpClient _dataServiceClient;

        public ClientDataServiceProvider(string dataServiceUrl)
        {
            _dataServiceUrl = dataServiceUrl;
            _dataServiceClient = new HttpClient();
        }

        public void Add(T entity)
        {
            //http://localhost:13560/api/DataProvider?entityType=Product&id=product11
            string url = GetEntityUrl(typeof(T).Name, entity.Id);

            var content = new StringContent(JsonConvert.SerializeObject(entity, Formatting.Indented), Encoding.UTF8);

            var result = _dataServiceClient.PostAsync(url, content).Result;
            

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception($"Data Service {typeof(T).Name} Add request failed with status code - {result.StatusCode}");
            }
        }

        public void Delete(string id)
        {
            //http://localhost:13560/api/DataProvider?entityType=Product&id=product11
            string url = GetEntityUrl(typeof(T).Name, id);

            var result = _dataServiceClient.DeleteAsync(url).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception($"Data Service {typeof(T).Name} id={id} Delete request failed with status code - {result.StatusCode}");
            }
        }

        public T Get(string id)
        {
            //http://localhost:13560/api/DataProvider?entityType=Product&id=product4
            string url = GetEntityUrl(typeof(T).Name, id);

            var result = _dataServiceClient.GetAsync(url).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception($"Data Service {typeof(T).Name} id={id} Get request failed with status code - {result.StatusCode}");
            }

            string json = result.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(json);
        }

        public IEnumerable<T> Get()
        {
            //http://localhost:13560/api/DataProvider/Entities?entityType=Product
            string url = $"{_dataServiceUrl}/Entities?entityType={typeof(T).Name}";

            var result = _dataServiceClient.GetAsync(url).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception($"Data Service {typeof(T).Name}s Get request failed with status code - {result.StatusCode}");
            }

            string json = result.Content.ReadAsStringAsync().Result;
            var list = JsonConvert.DeserializeObject<List<string>>(json);
            return list.Select(l => JsonConvert.DeserializeObject<T>(l));
        }

        public void Update(T entity)
        {
            string url = GetEntityUrl(typeof(T).Name, entity.Id);

            var content = new StringContent(JsonConvert.SerializeObject(entity, Formatting.Indented), Encoding.UTF8);

            var result = _dataServiceClient.PutAsync(url, content).Result;

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception($"Data Service {typeof(T).Name} id={entity.Id} Put request failed with status code - {result.StatusCode}");
            }
        }

        private string GetEntityUrl(string entityType, string id)
        {
            return $"{_dataServiceUrl}?entityType={entityType}&id={id}";
        }
    }
}
