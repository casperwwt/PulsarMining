using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pulsar.CoreElements.Api.Data.BaseModels;
using Pulsar.CoreElements.Api.Data.Repositories.Generics;
using Pulsar.CoreElements.Api.Data.Services;
using Pulsar.CoreElements.Api.ServiceInstallers.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulsar.CoreElements.Api.Models.CoreElements;

namespace Pulsar.CoreElements.Api.ServiceInstallers.Installers
{
    public class PersistentStorageInstaller : IServiceInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            // Configures services for saving to DB

             services.AddScoped<IRepositoryBase<CoreElement>, RepositoryBase<CoreElement>>();
             services.AddScoped<IPersistentStorageService<CoreElementViewModel, CoreElement>, PersistentStorageService<CoreElementViewModel, CoreElement>>();
        }
    }
}
    