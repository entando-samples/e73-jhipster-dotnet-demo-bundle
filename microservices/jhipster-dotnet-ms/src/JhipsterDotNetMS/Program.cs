using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Security.Authentication;
using System.Security.Claims;
using JhipsterDotNetMS;
using JhipsterDotNetMS.Configuration;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Common;
using Microsoft.AspNetCore.DataProtection;
using IStartup = JhipsterDotNetMS.IStartup;
using static JHipsterNet.Core.Boot.BannerPrinter;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Winton.Extensions.Configuration.Consul;

const string ConsulSection = "Consul";
const string ConsulHost = "Host";

PrintBanner(10 * 1000);

try
{
    var webAppOptions = new WebApplicationOptions
    {
        ContentRootPath = Directory.GetCurrentDirectory(),
        EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
        Args = args
    };
    var builder = WebApplication.CreateBuilder(webAppOptions);
    /*ENTANDO Add Keycloak adapter*/
    string authServerUrl = Environment.GetEnvironmentVariable("KEYCLOAK_AUTH_URL") ?? string.Empty;
    string realm = Environment.GetEnvironmentVariable("KEYCLOAK_REALM") ?? string.Empty;
    string client = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENT_ID") ?? string.Empty;
    string secretEnv = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENT_SECRET") ?? string.Empty;


    var secret = new KeycloakClientInstallationCredentials { Secret = secretEnv };

    var authenticationOptions = new KeycloakAuthenticationOptions
    {
        AuthServerUrl = authServerUrl,
        Realm = realm,
        Resource = client ,
        Credentials = secret,
        SslRequired =  "none",
    };
    
    builder.Services.AddKeycloakAuthentication(authenticationOptions);
    
    /* ENTANDO: Consul is not deployed in the entando instance 
       
    builder.Configuration.AddConsul(
        "config/application/data",
        options =>
        {
            options.ConsulConfigurationOptions =
                cco => { cco.Address = new Uri(builder.Configuration.GetSection(ConsulSection)[ConsulHost]); };
            //options.Optional = true;
            //options.PollWaitTime = TimeSpan.FromSeconds(5);
            //options.ReloadOnChange = true;
        });
    */
    
    builder.Logging.AddSerilog(builder.Configuration);

    IStartup startup = new Startup();

    startup.Configure(builder.Configuration, builder.Services);
    startup.ConfigureServices(builder.Services, builder.Environment);

    WebApplication app = builder.Build();
    
    /* ENTANDO: enable authentication and authorization */
    app.UseAuthentication();
    app.UseAuthorization();
    
    startup.ConfigureEntandoContext(app, app.Environment);
    startup.ConfigureMiddleware(app, app.Environment);
    startup.ConfigureEndpoints(app, app.Environment);

    app
        .MapGet("/",
            () =>
            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

    app.Run();

    return 0;
}
catch (Exception ex)
{
    // Use ForContext to give a context to this static environment (for Serilog LoggerNameEnricher).
    Log.ForContext<Program>().Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
