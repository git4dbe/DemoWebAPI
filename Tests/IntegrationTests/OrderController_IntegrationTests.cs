using ECommerceDemoInfrastructure.Entities;
using ECommerceDemoWebAPI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.IntegrationTests
{
    [TestClass]
    public class OrderController_IntegrationTests
    {
        [TestMethod]
        public async Task CRUDOperations_IsOK()
        {
            ShoppingCartController shoppingCartController = new ShoppingCartController();

            string shoppingCartId = Guid.NewGuid().ToString();
            var shoppingCart = new ShoppingCart() { Id = shoppingCartId };


            string productId = await CreateProduct();
            shoppingCart.Products = new List<Product>(new[] { new Product() { Id = productId } });

            await shoppingCartController.Post(shoppingCart);

            var order = new Order { Id = Guid.NewGuid().ToString() };
            order.ShoppingCart = shoppingCart;

            OrderController orderController = new OrderController();
            await orderController.Post(order);

            var orders = await orderController.Get();
            Assert.IsTrue(orders.Any(o => o.Id == order.Id));

            order.Description = "test desc";
            await orderController.Put(order);

            Order updatedOrder = await orderController.Get(order.Id);
            Assert.AreEqual(updatedOrder.Description, order.Description);


            await orderController.Delete(order.Id);
            Order deletedOrder = await orderController.Get(order.Id);
            Assert.IsNull(deletedOrder);

            var deletedShopingCart = await shoppingCartController.Get(shoppingCart.Id);
            Assert.IsNull(deletedShopingCart);

            await DeleteProduct(productId);

            var deletedProduct = await (new ProductController()).Get(productId);
            Assert.IsNull(deletedProduct);
        }

        private async Task<string> CreateProduct()
        {
            ProductController productController = new ProductController();

            string productId = Guid.NewGuid().ToString();
            var product = new Product() { Id = productId };

            await productController.Post(product);

            return productId;
        }

        private async Task DeleteProduct(string productId)
        {
            ProductController productController = new ProductController();

            await productController.Delete(productId);
        }
    }
}
