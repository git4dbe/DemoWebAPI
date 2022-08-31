using ECommerceDemoInfrastructure.Contracts;
using System;
using System.Collections.Generic;

namespace ECommerceDemoInfrastructure.Entities
{
    public class ShoppingCart : IEntity
    {
        public string Id { get; set; }

        public string ClientId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public List<Product> Products { get; set; }
    }
}
