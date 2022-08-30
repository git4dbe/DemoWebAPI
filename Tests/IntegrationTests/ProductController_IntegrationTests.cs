using ECommerceDemoWebAPI.Controllers;
using ECommerceDemoCommon.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.IntegrationTests
{
    [TestClass]
    public class ProductController_IntegrationTests
    {

        [TestMethod]
        public async Task CRUDOperations_IsOK()
        {
            ProductController productController = new ProductController();

            string productId = Guid.NewGuid().ToString();
            var product = new Product() { Id = productId };

            await productController.Post(product);

            var products = await productController.Get();
            Assert.IsTrue(products.Any(p => p.Id == productId));

            product.Name = "name1";
            await productController.Put(product);

            Product updatedProduct = await productController.Get(productId);
            Assert.AreEqual(product.Name, updatedProduct.Name);

            await productController.Delete(productId);
            Product deletedProduct = await productController.Get(productId);
            Assert.IsNull(deletedProduct);
        }
    }
}
