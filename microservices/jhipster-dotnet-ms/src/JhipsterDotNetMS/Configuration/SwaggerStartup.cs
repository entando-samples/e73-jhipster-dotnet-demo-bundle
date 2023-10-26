using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using JHipsterNet.Web.Pagination.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JhipsterDotNetMS.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerModule(this IServiceCollection services)
    {
        // ENTANDO Add Keycloak env vars
        string authServerUrl = Environment.GetEnvironmentVariable("KEYCLOAK_AUTH_URL") ?? string.Empty;
        string realm = Environment.GetEnvironmentVariable("KEYCLOAK_REALM") ?? string.Empty;

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v3", new OpenApiInfo { Title = "JhipsterDotNetMS API", Version = "0.0.1" });
            
            c.OperationFilter<PageableModelFilter>();
            // ENTANDO Add Keycloak Security Definition
            c.AddSecurityDefinition( JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authServerUrl+"/realms/"+realm+"/protocol/openid-connect/auth"),
                    }
                },
                In = ParameterLocation.Header,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id =  JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    new string[] {}
                }
            });
        });
       
        
        return services;
    }

    public static IApplicationBuilder UseApplicationSwagger(this IApplicationBuilder app)
    {
        // ENTANDO Add Keycloak env vars
        string realm = Environment.GetEnvironmentVariable("KEYCLOAK_REALM") ?? string.Empty;
        string client = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENT_ID") ?? string.Empty;
        string secretEnv = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENT_SECRET") ?? string.Empty;

        app.UseSwagger(c =>
        {
            c.RouteTemplate = "{documentName}/api-docs";
        });
        app.UseSwaggerUI(c =>
        {
            /* ENTANDO: Add the base path to properly read the /v3/api-docs */
            string pathBase = Environment.GetEnvironmentVariable("SERVER_SERVLET_CONTEXT_PATH") ?? string.Empty; 
            c.SwaggerEndpoint(pathBase +"/v3/api-docs", "JhipsterDotNetMS API");
            /* ENTANDO: Add keycloak auth */
            c.OAuthClientId(client);
            c.OAuthClientSecret(secretEnv);
            c.OAuthRealm(realm);
        });
        return app;
    }
}
