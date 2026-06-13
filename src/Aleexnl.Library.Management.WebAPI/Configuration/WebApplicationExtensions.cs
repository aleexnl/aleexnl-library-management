using Aleexnl.Library.Management.Data.Impl.Persistence;
using Aleexnl.Library.Management.WebAPI.Endpoints.Books;

namespace Aleexnl.Library.Management.WebAPI.Configuration;

/// <summary>
/// Configures the Web API request pipeline and runtime initialization.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Applies the library management API pipeline and startup initialization.
    /// </summary>
    /// <param name="app">The web application to configure.</param>
    /// <returns>The configured web application.</returns>
    public static async Task<WebApplication> UseWebApiAsync(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler();
        app.UseHttpsRedirection();

        await app.EnsureDatabaseCreatedAsync();

        app.MapBookEndpoints();

        return app;
    }

    private static async Task EnsureDatabaseCreatedAsync(this WebApplication app)
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        LibraryManagementDbContext dbContext = scope.ServiceProvider.GetRequiredService<LibraryManagementDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
    }
}
