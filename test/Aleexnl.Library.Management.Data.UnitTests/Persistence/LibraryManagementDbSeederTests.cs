using Aleexnl.Library.Management.Data.Contracts.Entities;
using Aleexnl.Library.Management.Data.Impl;
using Aleexnl.Library.Management.Data.Impl.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aleexnl.Library.Management.Data.UnitTests.Persistence;

public sealed class LibraryManagementDbSeederTests
{
    [Fact]
    public async Task EnsureCreatedAsync_PopulatesEmptyDatabase_OnlyOnce()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db");
        IConfiguration configuration = new ConfigurationManager
        {
            ["ConnectionStrings:LibraryManagement"] = $"Data Source={databasePath}",
            ["DatabaseSeeding:Enabled"] = bool.TrueString,
            ["DatabaseSeeding:BookCount"] = "7"
        };

        ServiceProvider serviceProvider = new ServiceCollection()
            .AddData(configuration)
            .BuildServiceProvider();

        try
        {
            await SeedDatabaseAsync(serviceProvider);
            await SeedDatabaseAsync(serviceProvider);

            await using AsyncServiceScope verificationScope = serviceProvider.CreateAsyncScope();
            LibraryManagementDbContext dbContext =
                verificationScope.ServiceProvider.GetRequiredService<LibraryManagementDbContext>();

            List<Book> seededBooks = await dbContext.Books
                .IgnoreQueryFilters()
                .OrderBy(book => book.CreatedAtUtc)
                .ToListAsync();

            Assert.Equal(7, seededBooks.Count);
            Assert.All(seededBooks, book =>
            {
                Assert.False(book.IsDeleted);
                Assert.False(string.IsNullOrWhiteSpace(book.Title));
                Assert.False(string.IsNullOrWhiteSpace(book.Author));
                Assert.Equal(book.NormalizedIsbn, book.Isbn.Replace("-", string.Empty, StringComparison.Ordinal));
            });
        }
        finally
        {
            await serviceProvider.DisposeAsync();
        }
    }

    private static async Task SeedDatabaseAsync(ServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        LibraryManagementDbContext dbContext = scope.ServiceProvider.GetRequiredService<LibraryManagementDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
    }
}
