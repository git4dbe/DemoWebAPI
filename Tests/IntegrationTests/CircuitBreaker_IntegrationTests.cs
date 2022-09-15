using CircuitBreaker.Concrete;
using ECommerceDemoInfrastructure.DataProviders;
using ECommerceDemoInfrastructure.Entities;
using ECommerceDemoInfrastructure.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tests.IntegrationTests
{
    [TestClass]
    public class CircuitBreaker_IntegrationTests
    {
        [TestMethod]
        public void CircuitBreakerClosed_IsOK()
        {
            var breaker = new CircuitBreakerFunc<IEnumerable<Product>>(new CircuitBreakerState(1000));

            var httpClient = new HttpClient();
            var client = new ClientDataServiceProvider<Product>(httpClient, GetDataServiceUrl(httpClient));

            IEnumerable<Product> result = breaker.ExecuteFunc(() => client.Get());

            Assert.IsTrue(breaker.BreakerState.IsClosed);
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void CircuitBreakerOpen_IsOK()
        {
            var breaker = new CircuitBreakerFunc<IEnumerable<Product>>(new CircuitBreakerState(5000));
            var httpClient = new HttpClient();

            Exception dummyUrlException = GetBreakerException(httpClient, breaker, "http://localhost:13560/DummyAPI");
            Assert.IsNotNull(dummyUrlException, "It is expected that request with invalid url will cause exception");
            Assert.IsTrue(breaker.BreakerState.IsOpen);

            Task.Delay(2000).Wait();
            
            string dataServiceUrl = GetDataServiceUrl(httpClient);

            Exception openWaitTimeException = GetBreakerException(httpClient, breaker, dataServiceUrl);
            Assert.IsNotNull(openWaitTimeException, "It is expected that in active open time period the request will be rejected");
            Assert.IsTrue(breaker.BreakerState.IsOpen);

            Task.Delay(4000).Wait();
            Exception thereIsNoException = GetBreakerException(httpClient, breaker, dataServiceUrl);
            Assert.IsTrue(breaker.BreakerState.IsClosed, "It is expected that after open time period the correct url request should be Ok");
            Assert.IsNull(thereIsNoException);
        }

        private Exception GetBreakerException(HttpClient httpClient, CircuitBreakerFunc<IEnumerable<Product>> breaker, string url)
        {
            IEnumerable<Product> result = null;
            var client = new ClientDataServiceProvider<Product>(httpClient, url);

            try
            {
                result = breaker.ExecuteFunc(() => client.Get());
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        private string GetDataServiceUrl(HttpClient httpClient)
        {
            return GatewayDiscoveryHelper.GetServiceUrls(httpClient).FirstOrDefault();
        }
    }
}
