using Aleexnl.Library.Management.Domain.Contracts.Books.Services;
using Aleexnl.Library.Management.Domain.Impl.Books.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aleexnl.Library.Management.Domain.Impl;

/// <summary>
/// Registers domain-layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds domain services for the library management application.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();

        return services;
    }
}
