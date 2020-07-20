using Microsoft.EntityFrameworkCore;
using Pulsar.Inventory.Api.Models.InventoryItems;

namespace Pulsar.Inventory.Api.Data.DbContexts
{
    public class DatabaseContext : DbContext
    {

        public DbSet<InventoryItem> InventoryItems { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
    }
}