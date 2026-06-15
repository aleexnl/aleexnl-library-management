using Aleexnl.Library.Management.Data.Impl.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aleexnl.Library.Management.WebAPI.HealthChecks;

/// <summary>
///     Verifies that the application can reach the configured database.
/// </summary>
/// <param name="dbContext">The database context used to probe connectivity.</param>
public sealed class DatabaseHealthCheck(LibraryManagementDbContext dbContext) : IHealthCheck
{
    /// <summary>
    ///     Executes the database connectivity probe.
    /// </summary>
    /// <param name="context">The health check execution context.</param>
    /// <param name="cancellationToken">The token used to cancel the probe.</param>
    /// <returns>A healthy result when the database is reachable; otherwise an unhealthy result.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        bool canConnect = await dbContext.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false);

        return canConnect
            ? HealthCheckResult.Healthy("Database connection is available.")
            : HealthCheckResult.Unhealthy("Database connection is unavailable.");
    }
}
