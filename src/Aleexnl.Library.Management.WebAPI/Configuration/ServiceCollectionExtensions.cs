using Aleexnl.Library.Management.Data.Impl;
using Aleexnl.Library.Management.Domain.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
        /// <returns>The configured service collection.</returns>
        public IServiceCollection AddWebApi(IConfiguration configuration)
        {
            services.AddApiDocumentation();
            services.AddApiAuthentication(configuration);
            services.AddApiAuthorization();
            services.AddApiApplicationServices(configuration);

            return services;
        }

        private IServiceCollection AddApiDocumentation()
        {
            services.AddProblemDetails();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        private IServiceCollection AddApiAuthentication(IConfiguration configuration)
        {
            IConfigurationSection authenticationSection = configuration.GetSection("Authentication");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = authenticationSection["Authority"]
                                    ?? throw new InvalidOperationException("Authentication:Authority is not configured.");
                options.Audience = authenticationSection["Audience"]
                                   ?? throw new InvalidOperationException("Authentication:Audience is not configured.");
            });

            return services;
        }

        private IServiceCollection AddApiAuthorization()
        {
            services.AddAuthorization();

            return services;
        }

        private IServiceCollection AddApiApplicationServices(IConfiguration configuration)
        {
            services.AddValidation();
            services.AddData(configuration);
            services.AddDomain();

            return services;
        }
    }
}
