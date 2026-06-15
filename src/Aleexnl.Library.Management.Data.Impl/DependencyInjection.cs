using Aleexnl.Library.Management.Data.Contracts.Repositories;
using Aleexnl.Library.Management.Data.Impl.Persistence;
using Aleexnl.Library.Management.Data.Impl.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aleexnl.Library.Management.Data.Impl;

/// <summary>
/// Registers data-layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds EF Core persistence services for the library management application.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("LibraryManagement");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection string is empty");
        }

        services.AddDbContext<LibraryManagementDbContext>(options => options
            .UseSqlite(connectionString)
            .UseSeeding((dbContext, _) => LibraryManagementDbSeeder.Seed(dbContext, configuration))
            .UseAsyncSeeding((dbContext, _, cancellationToken) =>
                LibraryManagementDbSeeder.SeedAsync(dbContext, configuration, cancellationToken)));
        services.AddScoped<IBookRepository, BookRepository>();

        return services;
    }
}
