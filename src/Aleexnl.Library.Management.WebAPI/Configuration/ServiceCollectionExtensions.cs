using Aleexnl.Library.Management.Data.Impl;
using Aleexnl.Library.Management.Domain.Impl;

namespace Aleexnl.Library.Management.WebAPI.Configuration;

/// <summary>
/// Configures application services for the Web API host.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the services required by the library management API.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProblemDetails();
        services.AddOpenApi();
        services.AddValidation();
        services.AddData(configuration);
        services.AddDomain();

        return services;
    }
}
