using ECommerceDemoWebAPI.Contracts;
using System;

namespace ECommerceDemoWebAPI.Entities
{
    public class Product : IEntity
    {
        public string Id { get; set; }

        public string SerialNumber { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProductType { get; set; }

        public string Manufacturer { get; set; }

        public DateTime ProductionDate { get; set; }

        public Decimal Price { get; set; }

        public string Currency { get; set; }
    }
}
