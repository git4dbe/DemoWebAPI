using CircuitBreaker.Concrete;
using ECommerceDemoInfrastructure.DataProviders;
using ECommerceDemoInfrastructure.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var client = new ClientDataServiceProvider<Product>(GetDataServiceUrl());

            IEnumerable<Product> result = breaker.ExecuteFunc(() => client.Get());

            Assert.IsTrue(breaker.BreakerState.IsClosed);
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void CircuitBreakerOpen_IsOK()
        {
            var breaker = new CircuitBreakerFunc<IEnumerable<Product>>(new CircuitBreakerState(5000));

            Exception dummyUrlException = GetBreakerException(breaker, "http://localhost:13560/DummyAPI");
            Assert.IsNotNull(dummyUrlException, "It is expected that request with invalid url will cause exception");
            Assert.IsTrue(breaker.BreakerState.IsOpen);

            Task.Delay(2000).Wait();

            Exception openWaitTimeException = GetBreakerException(breaker, GetDataServiceUrl());
            Assert.IsNotNull(openWaitTimeException, "It is expected that in active open time period the request will be rejected");
            Assert.IsTrue(breaker.BreakerState.IsOpen);

            Task.Delay(4000).Wait();
            Exception thereIsNoException = GetBreakerException(breaker, GetDataServiceUrl());
            Assert.IsTrue(breaker.BreakerState.IsClosed, "It is expected that after open time period the correct url request should be Ok");
            Assert.IsNull(thereIsNoException);
        }

        private Exception GetBreakerException(CircuitBreakerFunc<IEnumerable<Product>> breaker, string url)
        {
            IEnumerable<Product> result = null;
            var client = new ClientDataServiceProvider<Product>(url);

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

        private string GetDataServiceUrl()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            return config.GetValue<string>("DataProviderService:Url");
        }
    }
}
