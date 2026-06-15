using System.Text.Json;
using Aleexnl.Library.Management.Data.Impl.Persistence;
using Aleexnl.Library.Management.WebAPI.Endpoints.Books;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

namespace Aleexnl.Library.Management.WebAPI.Configuration;

/// <summary>
/// Configures the Web API request pipeline and runtime initialization.
/// </summary>
public static class WebApplicationExtensions
{
    /// <param name="app">The web application to configure.</param>
    extension(WebApplication app)
    {
        /// <summary>
        /// Applies the library management API pipeline and startup initialization.
        /// </summary>
        /// <returns>The configured web application.</returns>
        public async Task<WebApplication> UseWebApiAsync()
        {
            app.UseApiDocumentation();
            app.UseApiInfrastructure();
            app.UseApiSecurity();
            await app.EnsureDatabaseCreatedAsync().ConfigureAwait(false);
            app.MapApiEndpoints();

            return app;
        }

        private WebApplication UseApiDocumentation()
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            return app;
        }

        private WebApplication UseApiInfrastructure()
        {
            app.UseExceptionHandler();
            app.UseHttpsRedirection();

            return app;
        }

        private WebApplication UseApiSecurity()
        {
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

        private WebApplication MapApiEndpoints()
        {
            app.MapHealthEndpoints();
            app.MapBookEndpoints();

            return app;
        }

        private WebApplication MapHealthEndpoints()
        {
            app.MapHealthChecks("/health/live",
                new HealthCheckOptions
                {
                    Predicate = registration => registration.Tags.Contains("live"),
                    ResponseWriter = WriteHealthCheckResponseAsync
                });

            app.MapHealthChecks("/health/ready",
                new HealthCheckOptions
                {
                    Predicate = registration => registration.Tags.Contains("ready"),
                    ResponseWriter = WriteHealthCheckResponseAsync
                });

            return app;
        }

        private async Task EnsureDatabaseCreatedAsync()
        {
            await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
            LibraryManagementDbContext dbContext =
                scope.ServiceProvider.GetRequiredService<LibraryManagementDbContext>();

            await dbContext.Database.EnsureCreatedAsync().ConfigureAwait(false);
        }

        private static async Task WriteHealthCheckResponseAsync(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
                {
                    status = report.Status.ToString(),
                    totalDuration = report.TotalDuration,
                    checks = report.Entries.ToDictionary(
                        entry => entry.Key,
                        entry => new
                        {
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description,
                            duration = entry.Value.Duration
                        })
                }, JsonSerializerOptions.Web)
                .ConfigureAwait(false);
        }
    }
}
