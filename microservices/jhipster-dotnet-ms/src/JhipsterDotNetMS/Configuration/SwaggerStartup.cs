using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using JHipsterNet.Web.Pagination.Swagger;

namespace JhipsterDotNetMS.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerModule(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v3", new OpenApiInfo { Title = "JhipsterDotNetMS API", Version = "0.0.1" });
            c.OperationFilter<PageableModelFilter>();
        });
       
        
        return services;
    }

    public static IApplicationBuilder UseApplicationSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "{documentName}/api-docs";
        });
        app.UseSwaggerUI(c =>
        {
            /* ENTANDO: add the base path to properly read the /v3/api-docs */
            string pathBase = Environment.GetEnvironmentVariable("SERVER_SERVLET_CONTEXT_PATH") ?? string.Empty; 
            c.SwaggerEndpoint(pathBase +"/v3/api-docs", "JhipsterDotNetMS API");
        });
        return app;
    }
}
