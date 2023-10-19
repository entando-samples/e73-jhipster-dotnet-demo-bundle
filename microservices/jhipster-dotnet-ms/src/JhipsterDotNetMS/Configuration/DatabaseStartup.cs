using System;
using JhipsterDotNetMS.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace JhipsterDotNetMS.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseModule(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = null;
        string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
        string databaseUrl = configuration.GetValue<string>("DATABASE_URL") ?? string.Empty;
        string dbUrlEnv  = Environment.GetEnvironmentVariable("SPRING_DATASOURCE_URL") ?? string.Empty;
        string dbUserEnv  = Environment.GetEnvironmentVariable("SPRING_DATASOURCE_USERNAME") ?? string.Empty;
        string dbPassEnv  = Environment.GetEnvironmentVariable("SPRING_DATASOURCE_PASSWORD") ?? string.Empty;
        
        dbUrlEnv = dbUrlEnv.Substring(dbUrlEnv.LastIndexOf("//") + 2);
        
        string dbHostEnv = dbUrlEnv.Substring(0, dbUrlEnv.LastIndexOf(":"));        
        string dbPortEnv = dbUrlEnv.Substring(dbUrlEnv.LastIndexOf(":") + 1, dbUrlEnv.LastIndexOf("/")-dbUrlEnv.LastIndexOf(":") - 1);
        string dbNameEnv = dbUrlEnv.Substring(dbUrlEnv.LastIndexOf("/") + 1);
        
        if (!String.IsNullOrEmpty(databaseUrl) && Uri.IsWellFormedUriString(databaseUrl, UriKind.RelativeOrAbsolute))
        {
            Console.WriteLine("DATABASE_URL will be used to create the connection string.");
            //  Parse the connection string
            var databaseUri = new Uri(databaseUrl);
            string db = databaseUri.LocalPath.TrimStart('/');
            string[] userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);

            switch (databaseUri.Scheme)
            {
                case "postgres":
                    connectionString = $"Server={databaseUri.Host};Port={databaseUri.Port};Database={db};User Id={userInfo[0]};Password={userInfo[1]};Integrated Security=true;Pooling=true;MinPoolSize=0;MaxPoolSize=20;";
                    break;
                case "mysql":
                    connectionString = $"Server={databaseUri.Host};Port={databaseUri.Port};Database={db};User={userInfo[0]};Password={userInfo[1]};Pooling=true;MinimumPoolSize=0;MaximumPoolsize=10;";
                    break;
                case "mssql":
                    connectionString = $"Server={databaseUri.Host};Port={databaseUri.Port};Database={db};User={userInfo[0]};Password={userInfo[1]};Trusted_Connection=False;Pooling=true;";
                    break;
                case "mongodb":
                    connectionString = $"Server={databaseUri.Host};Port={databaseUri.Port};Database={db};User={userInfo[0]};Password={userInfo[1]};Pooling=true;MinimumPoolSize=0;MaximumPoolsize=10;";
                    break;
                default:
                    Console.WriteLine("It was not possible to determine the database type provided by DATABASE_URL");
                    break;
            }
        }
        else
        {
            if (environmentName.Equals("Production") || environmentName.Equals("Development")) {
                connectionString = "Host=" + dbHostEnv + ";Database=" + dbNameEnv + ";Port=" + dbPortEnv +
                                   ";User Id=" + dbUserEnv + ";Password=" + dbPassEnv +
                                   ";Integrated Security=true;Pooling=true;";
            }
            else {
                connectionString = configuration.GetConnectionString("AppDbContext");
            }

        }
        // Opt out of the new timestamp mapping logic for postgres (https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic)
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.AddDbContext<ApplicationDatabaseContext>(context => { context.UseNpgsql(connectionString); });
        services.AddScoped<DbContext>(provider => provider.GetService<ApplicationDatabaseContext>());
        return services;
    }

    public static IApplicationBuilder UseApplicationDatabase(this IApplicationBuilder app, IHostEnvironment environment)
    {
        if (environment.IsDevelopment() || environment.IsProduction())
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDatabaseContext>();
                context.Database.OpenConnection();
                /*
                 *  ENTANDO : Change the table creation strategy from the original jhipster.net generated code 
                 *            to create tables also in a not empty database for the installation of the bundle in the
                 *            entando instance using the production environment 
                 */
                // context.Database.EnsureCreated();
                // Initialize the schema for this DbContext
                var databaseCreator = context.GetService<IRelationalDatabaseCreator>();
                try {
                    databaseCreator.CreateTables();
                } catch (Npgsql.PostgresException) {
                    //A Npgsql.PostgresException will be thrown if tables already exist. So simply ignore it.
                }
            }
        }
        return app;
    }
}
