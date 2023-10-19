using System;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace JhipsterDotNetMS.Configuration.Consul;

public static class ConsulStartup
{
    public static IServiceCollection AddConsul(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var consulConfigOptions = configuration.GetOptions<ConsulOptions>("Consul");

        serviceCollection.Configure<ConsulOptions>(configuration.GetSection("Consul"));

        return serviceCollection.AddSingleton<IConsulClient>(c => new ConsulClient(cfg =>
        {
            if (!string.IsNullOrEmpty(consulConfigOptions.Host))
            {
                cfg.Address = new Uri(consulConfigOptions.Host);
            }
        }));
    }

    public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var config = scope.ServiceProvider.GetRequiredService<IOptions<ConsulOptions>>().Value;

            if (!config.Enabled)
                return app;

            var serviceId = Guid.NewGuid();
            var consulServiceId = $"{config.Service}:{serviceId}";

            var client = scope.ServiceProvider.GetService<IConsulClient>();

            var consulServiceRistration = new AgentServiceRegistration
            {
                Name = config.Service,
                ID = consulServiceId,
                Address = config.Address,
                Port = config.Port,
            };

            if (config.PingEnabled)
            {
                var healthService = scope.ServiceProvider.GetService<HealthCheckService>();

                if (healthService != null)
                {
                    var scheme = config.Address.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)
                        ? string.Empty
                        : "http://";
                    var check = new AgentServiceCheck
                    {
                        Interval = TimeSpan.FromSeconds(5),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
                        HTTP =
                            $"{scheme}{config.Address}{(config.Port > 0 ? $":{config.Port}" : string.Empty)}/health"
                    };

                    consulServiceRistration.Checks = new[] { check };
                }
                else
                {
                    throw new ConsulConfigurationException("Please ensure that Healthchecks has been added before adding checks to Consul.");
                }
            }

            client.Agent.ServiceRegister(consulServiceRistration);

            return app;
        }
    }
}
