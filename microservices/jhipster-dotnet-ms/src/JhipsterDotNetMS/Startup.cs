using System;
using JhipsterDotNetMS.Infrastructure.Data;
using JhipsterDotNetMS.Configuration;
using JhipsterDotNetMS.Infrastructure.Configuration;
using JhipsterDotNetMS.Configuration.Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

[assembly: ApiController]

namespace JhipsterDotNetMS;

public class Startup : IStartup
{
    public virtual void Configure(IConfiguration configuration, IServiceCollection services)
    {
        services
            .AddAppSettingsModule(configuration);
            /*ENTANDO
            .AddConsul(configuration);
            */

            AddDatabase(configuration, services);
}

    public virtual void ConfigureServices(IServiceCollection services, IHostEnvironment environment)
    {
        services
            .AddProblemDetailsModule(environment)
            .AddAutoMapperModule()
            .AddSwaggerModule()
            .AddWebModule()
            .AddRepositoryModule()
            .AddServiceModule();
    }
    
    public virtual void ConfigureMiddleware(IApplicationBuilder app, IHostEnvironment environment)
    {
        IServiceProvider serviceProvider = app.ApplicationServices;
            var securitySettingsOptions = serviceProvider.GetRequiredService<IOptions<SecuritySettings>>();
            var securitySettings = securitySettingsOptions.Value;
           
            app
                .UseApplicationSecurity(securitySettings)
                .UseApplicationProblemDetails()
                /* ENTANDO
                 .UseConsul() */
                .UseApplicationDatabase(environment);

    }

    public virtual void ConfigureEndpoints(IApplicationBuilder app, IHostEnvironment environment)
    {
        app
            .UseApplicationSwagger()
            .UseApplicationWeb(environment);
    }

    public void ConfigureEntandoContext(WebApplication app, IWebHostEnvironment appEnvironment) 
    {
        // Set the path base using an environment variable. SERVER_SERVLET_CONTEXT_PATH is provided by Entando and used in the auto-wiring of the ready/live checks
        var pathBase = Environment.GetEnvironmentVariable("SERVER_SERVLET_CONTEXT_PATH") ?? string.Empty;            
        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }
    }
    
    protected virtual void AddDatabase(IConfiguration configuration, IServiceCollection services)
    {
        services.AddDatabaseModule(configuration);
    }
}
