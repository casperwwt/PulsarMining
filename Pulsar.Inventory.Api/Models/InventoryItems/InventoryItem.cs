using System;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Pulsar.Inventory.Api.Data.BaseModels;

namespace Pulsar.Inventory.Api.Models.InventoryItems
{
    public class InventoryItem : BaseDbEntity
    {
        public double CurrentInventoryLevel { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}