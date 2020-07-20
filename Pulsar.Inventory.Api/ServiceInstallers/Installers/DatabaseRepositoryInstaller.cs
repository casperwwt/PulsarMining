using Pulsar.Inventory.Api.Data.DbContexts;
using Pulsar.Inventory.Api.Infrastructure.ApplicationConfigurationServices;
using Pulsar.Inventory.Api.ServiceInstallers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using Pulsar.Inventory.Api.Models.InventoryItems;

namespace Pulsar.Inventory.Api.ServiceInstallers.Installers
{
    public class DatabaseRepositoryInstaller : IServiceInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var appConfiguration =
                (ApplicationConfigurationService)serviceProvider.GetService(typeof(ApplicationConfigurationService));

            var logger =
                (ILogger<DatabaseRepositoryInstaller>)serviceProvider.GetRequiredService(
                    typeof(ILogger<DatabaseRepositoryInstaller>));

            if (appConfiguration.UseInMemorySql)
            {
                logger.LogInformation("DatabaseContext: InMemory Database has been configured. Any persistent memory store settings will be ignored.");
                services.AddDbContext<DatabaseContext>(options => { options.UseInMemoryDatabase("InMemory"); });
            }
            else
            {
                if (appConfiguration.IsSqlConfigured)
                {
                    logger.LogInformation("DatabaseContext: SQL Persistence database will be used");
                    services.AddDbContext<DatabaseContext>(options =>
                    {
                        options.UseSqlServer(appConfiguration.SqlConnectionString);
                    });

                    // Connection to SQL server is not tested. Future health check and connection errors should be handled in service.
                }
                else
                {
                    logger.LogCritical("CRITICAL ERROR: Required Database persistence settings not found.");
                    throw new Exception("CRITICAL ERROR: Required Database persistence settings not found.");
                }
            }

            if (appConfiguration.IsDemo)
            {
                try
                {

                    logger.LogInformation("#### !!! Server operating in Demo Mode !!! ####");
                    serviceProvider = services.BuildServiceProvider();
                    var context = (DatabaseContext)serviceProvider.GetService(typeof(DatabaseContext));



                    DemoSeedDatabase(context);
                }
                catch (Exception e)
                {
                    logger.LogCritical("CRITICAL ERROR: Application is set to Demo Mode but cannot connect to database.");
                }
            }



        }

        /// <summary>
        /// Seeds database with demo data. Should be invoked when IsDemo == True;
        /// </summary>

        private static void DemoSeedDatabase(DatabaseContext context)
        {
            // Seeding function should only run on empty DBs to prevent duplicates.

            context.Database.EnsureCreated();

            if (context.InventoryItems.FirstOrDefault() == null)
            {
                var IntentoryList = new List<InventoryItem>()
                {
                    new InventoryItem()
                    {
                        Id = new Guid("0e152be9-4502-4264-b42e-9cab6e5fb42e"),
                        CurrentInventoryLevel = 50000
                    },
                    new InventoryItem()
                    {
                        Id = new Guid("c6804f17-bf93-4038-a2c7-587e47797d47"),
                        CurrentInventoryLevel = 900000
                    },
                    new InventoryItem()
                    {
                        Id = new Guid("c1f5b753-d111-4ca1-a8c8-a4c32942a2f6"),
                        CurrentInventoryLevel = 212233
                    },
                    new InventoryItem()
                    {
                        Id = new Guid("a63a4015-679f-4953-8ed1-a53792510017"),
                        CurrentInventoryLevel = 0
                    }
                };

                context.InventoryItems.AddRange(IntentoryList);
                context.SaveChanges();
            }
        }
    }
}