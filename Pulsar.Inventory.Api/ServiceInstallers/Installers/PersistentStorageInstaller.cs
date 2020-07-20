using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pulsar.Inventory.Api.Data.BaseModels;
using Pulsar.Inventory.Api.Data.Repositories.Generics;
using Pulsar.Inventory.Api.Data.Services;
using Pulsar.Inventory.Api.ServiceInstallers.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulsar.Inventory.Api.Models.InventoryItems;

namespace Pulsar.Inventory.Api.ServiceInstallers.Installers
{
    public class PersistentStorageInstaller : IServiceInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            // Configures services for saving to DB

             services.AddScoped<IRepositoryBase<InventoryItem>, RepositoryBase<InventoryItem>>();
             services.AddScoped<IPersistentStorageService<InventoryItemViewModel, InventoryItem>, PersistentStorageService<InventoryItemViewModel, InventoryItem>>();
        }
    }
}
