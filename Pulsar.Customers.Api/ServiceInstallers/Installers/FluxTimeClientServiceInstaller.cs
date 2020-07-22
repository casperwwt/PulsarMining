using System;
using System.Net.Http;
using Pulsar.Customers.Api.Infrastructure.ApplicationConfigurationServices;
using Pulsar.Customers.Api.Infrastructure.FluxTimeServerClientServices;
using Pulsar.Customers.Api.ServiceInstallers.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pulsar.Customers.Api.ServiceInstallers.Installers
{
    /// <summary>
    ///     This installer sets up the FluxTimeClientService. If services is configured, but server is unreachable a critical
    ///     exception should be thrown
    /// </summary>
    public class FluxTimeClientServiceInstaller : IServiceInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            var fluxTimeServerBaseAddress = "";
            // Retrieve required services dependencies
            var serviceProvider = services.BuildServiceProvider();
            var logger =
                (ILogger<FluxTimeClientServiceInstaller>)serviceProvider.GetRequiredService(
                    typeof(ILogger<FluxTimeClientServiceInstaller>));

            var appConfig =
                (ApplicationConfigurationService)serviceProvider.GetRequiredService(
                    typeof(ApplicationConfigurationService));

            // Ensures FluxTimeServer, if configured, is reachable to prevent catastrophic or corrupt data input.

            if (appConfig.IsTimeServerConfigured)
            {
                fluxTimeServerBaseAddress = $"http://{appConfig.TimeServerHostName}";

                logger.LogInformation($"FLUXCLIENT: Found time server {fluxTimeServerBaseAddress} in configuration");

                try
                {
                    var client = new HttpClient();
                    var response = client.GetAsync($"{fluxTimeServerBaseAddress}/api/time").Result;
                }
                catch (Exception e)
                {
                    logger.LogCritical($"CRITICAL ERROR: Time Server configured but not reachable. Startup aborted. {e.Message}");
                    throw new Exception("CRITICAL ERROR: Time Server configured but not reachable. Startup aborted.");
                }
            }
            else
            {
                logger.LogInformation("FLUXCLIENT: No time server has been configured. Will use local system time.");
            }

            services.AddHttpClient("FluxTimeHTTPClient",
                client => { client.BaseAddress = new Uri(fluxTimeServerBaseAddress); });

            services.AddScoped<FluxTimeServerClientService>();
        }
    }
}