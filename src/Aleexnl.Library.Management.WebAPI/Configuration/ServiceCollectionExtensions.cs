using Aleexnl.Library.Management.Data.Impl;
using Aleexnl.Library.Management.Domain.Impl;
using Aleexnl.Library.Management.WebAPI.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;

namespace Aleexnl.Library.Management.WebAPI.Configuration;

/// <summary>
/// Configures application services for the Web API host.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <param name="services">The service collection to configure.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds the services required by the library management API.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public void AddWebApi(IConfiguration configuration)
        {
            services.AddApiDocumentation();
            services.AddApiAuthentication(configuration);
            services.AddApiAuthorization();
            services.AddApiHealthChecks();
            services.AddApiApplicationServices(configuration);
        }

        private void AddApiDocumentation()
        {
            services.AddProblemDetails();
            services.AddOpenApi(options =>
            {
                const string bearerSecuritySchemeId = "Bearer";

                options.AddDocumentTransformer((document, _, _) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes[bearerSecuritySchemeId] = new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Paste a valid Auth0 bearer token.",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT"
                    };

                    return Task.CompletedTask;
                });
                options.AddOperationTransformer((operation, context, _) =>
                {
                    bool requiresAuthorization = context.Description.ActionDescriptor.EndpointMetadata
                        .OfType<IAuthorizeData>()
                        .Any();

                    if (!requiresAuthorization)
                    {
                        return Task.CompletedTask;
                    }

                    operation.Security ??= [];
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(bearerSecuritySchemeId)] = []
                    });

                    return Task.CompletedTask;
                });
            });
        }

        private void AddApiAuthentication(IConfiguration configuration)
        {
            IConfigurationSection authenticationSection = configuration.GetSection("Authentication");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = authenticationSection["Authority"]
                                    ?? throw new InvalidOperationException(
                                        "Authentication:Authority is not configured.");
                options.Audience = authenticationSection["Audience"]
                                   ?? throw new InvalidOperationException("Authentication:Audience is not configured.");
            });
        }

        private void AddApiAuthorization()
        {
            services.AddAuthorization();
        }

        private void AddApiHealthChecks()
        {
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy("Application is running."), ["live"])
                .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);
        }

        private void AddApiApplicationServices(IConfiguration configuration)
        {
            services.AddValidation();
            services.AddData(configuration);
            services.AddDomain();
        }
    }
}
