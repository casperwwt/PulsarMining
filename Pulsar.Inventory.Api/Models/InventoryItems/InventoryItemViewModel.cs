using System;

namespace Pulsar.Inventory.Api.Models.InventoryItems
{
    public class InventoryItemViewModel
    {
        public Guid Id { get; set; }
        public double CurrentInventoryLevel { get; set; }
    }
}