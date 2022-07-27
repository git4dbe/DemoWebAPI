using ECommerceDemoWebAPI.Contracts;
using System;

namespace ECommerceDemoWebAPI.Entities
{
    public class Order : IEntity
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string DeliveryAddress { get; set; }

        public string ContactInfo { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public ShoppingCart ShoppingCart { get; set; }
    }
}
