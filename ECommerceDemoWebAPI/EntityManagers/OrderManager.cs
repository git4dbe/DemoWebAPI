using ECommerceDemoCommon.Contracts;
using ECommerceDemoCommon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceDemoWebAPI.EntityManagers
{
    public class OrderManager : IEntityManager<Order>
    {
        private readonly IDataProvider<ShoppingCart> _shoppingCartDataProvider;
        private readonly IDataProvider<Order> _orderDataProvider;
        private readonly IDataProvider<Product> _productDataProvider;

        public OrderManager(
            IDataProvider<ShoppingCart> shoppingCartDataProvider,
            IDataProvider<Order> orderDataProvider,
            IDataProvider<Product> productDataProvider)
        {
            _shoppingCartDataProvider = shoppingCartDataProvider;
            _orderDataProvider = orderDataProvider;
            _productDataProvider = productDataProvider;
        }

        public async Task AddAsync(Order entity)
        {
            Order order = await GetAsync(entity.Id);
            if (order != null)
            {
                throw new ArgumentException($"Order with Id='{entity.Id}' already exists");
            }

            Order newOrder = new Order()
            {
                Id = entity.Id,
                Description = entity.Description,
                DeliveryDate = entity.DeliveryDate,
                DeliveryAddress = entity.DeliveryAddress,
                ContactInfo = entity.ContactInfo,
                Status = entity.Status,
                CreatedDate = entity.CreatedDate ?? DateTime.Now,
                CompletedDate = entity.CompletedDate
            };


            newOrder.ShoppingCart = await Task.Run(() => _shoppingCartDataProvider.Get(entity.ShoppingCart.Id));
            await Task.Run(() => _shoppingCartDataProvider.Delete(entity.ShoppingCart.Id));
            await Task.Run(() => _orderDataProvider.AddAsync(newOrder));
        }

        public async Task DeleteAsync(string id)
        {
            Order order = await GetAsync(id);

            List<Task> restoreProductsTasks = order.ShoppingCart.Products
                                                        .Select(p => Task.Run(() => _productDataProvider.AddAsync(p)))
                                                        .ToList();

            await Task.WhenAll(restoreProductsTasks);
            await Task.Run(() => _orderDataProvider.Delete(id));
        }

        public async Task<Order> GetAsync(string id)
        {
            Order order = await Task.Run(() => _orderDataProvider.Get(id));
            return order;
        }

        public async Task<List<Order>> GetAsync()
        {
            List<Order> orders = await Task.Run(() => _orderDataProvider.Get().ToList());
            return orders;
        }

        public async Task UpdateAsync(Order entity)
        {
            Order order = await GetAsync(entity.Id);
            if (order == null)
            {
                throw new ArgumentException($"Order with Id='{entity.Id}' does not exist");
            }

            if (entity.Description != order.Description && !string.IsNullOrEmpty(entity.Description))
            {
                order.Description = entity.Description;
            }

            if (entity.DeliveryDate != order.DeliveryDate && entity.DeliveryDate.HasValue)
            {
                order.DeliveryDate = entity.DeliveryDate;
            }

            if (entity.DeliveryAddress != order.DeliveryAddress && !string.IsNullOrEmpty(entity.DeliveryAddress))
            {
                order.DeliveryAddress = entity.DeliveryAddress;
            }

            if (entity.ContactInfo != order.ContactInfo && !string.IsNullOrEmpty(entity.ContactInfo))
            {
                order.ContactInfo = entity.ContactInfo;
            }

            if (entity.Status != order.Status && !string.IsNullOrEmpty(entity.Status))
            {
                order.Status = entity.Status;
            }

            if (entity.CompletedDate != order.CompletedDate && entity.CompletedDate.HasValue)
            {
                order.CompletedDate = entity.CompletedDate;
            }

            await Task.Run(() => _orderDataProvider.Update(order));
        }

    }
}
