using ECommerceDemoWebAPI.Contracts;
using ECommerceDemoWebAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceDemoWebAPI.EntityManagers
{
    public class ShoppingCartManager : IEntityManager<ShoppingCart>
    {
        private readonly IDataProvider<ShoppingCart> _shoppingCartDataProvider;
        private readonly IDataProvider<Product> _productDataProvider;

        public ShoppingCartManager(
            IDataProvider<ShoppingCart> shoppingCartDataProvider,
            IDataProvider<Product> productDataProvider)
        {
            _shoppingCartDataProvider = shoppingCartDataProvider;
            _productDataProvider = productDataProvider;
        }
        public async Task AddAsync(ShoppingCart entity)
        {
            ShoppingCart cart = await GetAsync(entity.Id);
            if (cart != null)
            {
                throw new ArgumentException($"Shopping Cart with Id='{entity.Id}' already exists");
            }

            ShoppingCart newShoppingCart = new ShoppingCart()
            {
                Id = entity.Id,
                ClientId = entity.ClientId,
                CreatedDate = entity.CreatedDate ?? DateTime.Now
            };


            if (!entity.Products.Any())
            {
                throw new ArgumentException($"There were specified no products for shopping cart id={entity.Id}");
            }

            await UpdateProducts(newShoppingCart, entity.Products.Select(p => p.Id).ToList());
            await Task.Run(() => _shoppingCartDataProvider.Add(newShoppingCart));
        }

        public async Task DeleteAsync(string id)
        {
            ShoppingCart shoppingCart = await GetAsync(id);
            await RestoreProducts(shoppingCart);
            await Task.Run(() => _shoppingCartDataProvider.Delete(id));
        }

        public async Task<ShoppingCart> GetAsync(string id)
        {
            ShoppingCart shoppingCart = await Task.Run(() => _shoppingCartDataProvider.Get(id));
            return shoppingCart;
        }

        public async Task<List<ShoppingCart>> GetAsync()
        {
            List<ShoppingCart> shoppingCarts = await Task.Run(() => _shoppingCartDataProvider.Get().ToList());
            return shoppingCarts;
        }

        public async Task UpdateAsync(ShoppingCart entity)
        {
            ShoppingCart shoppingCart = await GetAsync(entity.Id);
            if (shoppingCart == null)
            {
                throw new ArgumentException($"Shopping Cart with Id='{entity.Id}' does not exist");
            }

            if (entity.ClientId != shoppingCart.ClientId)
            {
                shoppingCart.ClientId = entity.ClientId;
            }

            if (entity.CreatedDate != shoppingCart.CreatedDate && entity.CreatedDate.HasValue)
            {
                shoppingCart.CreatedDate = entity.CreatedDate;
            }

            if (entity.Products != null && entity.Products.Any())
            {
                await RestoreProducts(shoppingCart);
                await UpdateProducts(shoppingCart, entity.Products.Select(p => p.Id).ToList());
            }

            await Task.Run(() => _shoppingCartDataProvider.Update(shoppingCart));
        }


        private async Task UpdateProducts(ShoppingCart shoppingCart, List<string> productIds)
        {
            shoppingCart.Products = new List<Product>();
            List<Task> removeProductsTasks = new List<Task>();

            foreach (string productId in productIds)
            {
                Product product = await Task.Run(() => _productDataProvider.Get(productId));
                if (product != null)
                {
                    shoppingCart.Products.Add(product);
                    //
                    // Remove products from repository which were selected in shoping cart
                    //
                    removeProductsTasks.Add(Task.Run(() => _productDataProvider.Delete(productId)));
                }
            }

            await Task.WhenAll(removeProductsTasks);
        }

        private async Task RestoreProducts(ShoppingCart shoppingCart)
        {
            List<Task> restoreProductsTasks = shoppingCart.Products
                                                        .Select(p => Task.Run(() => _productDataProvider.Add(p)))
                                                        .ToList();

            await Task.WhenAll(restoreProductsTasks);
        }
    }
}
