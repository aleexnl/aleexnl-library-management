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

    [Fact]
    public async Task Seed_WhenSeedingDisabled_DoesNotInsertBooks()
    {
        await using LibraryManagementDbContext dbContext = CreateDbContext();
        IConfiguration configuration = CreateConfiguration(false, 7);

        await dbContext.Database.EnsureCreatedAsync();

        LibraryManagementDbSeeder.Seed(dbContext, configuration);

        Assert.Equal(0, await CountBooksAsync(dbContext));
    }

    [Fact]
    public async Task Seed_WhenBookCountMissing_UsesDefaultCount()
    {
        await using LibraryManagementDbContext dbContext = CreateDbContext();
        IConfiguration configuration = CreateConfiguration(true);

        await dbContext.Database.EnsureCreatedAsync();

        LibraryManagementDbSeeder.Seed(dbContext, configuration);

        List<Book> seededBooks = await GetSeededBooksAsync(dbContext);

        Assert.Equal(25, seededBooks.Count);
        AssertSeededBooks(seededBooks);
    }

    [Fact]
    public async Task SeedAsync_WhenDatabaseAlreadyContainsBooks_DoesNotReseed()
    {
        await using LibraryManagementDbContext dbContext = CreateDbContext();
        IConfiguration configuration = CreateConfiguration(true, 3);

        await dbContext.Database.EnsureCreatedAsync();
        await LibraryManagementDbSeeder.SeedAsync(dbContext, configuration, CancellationToken.None);
        List<Guid> initialIds = (await GetSeededBooksAsync(dbContext)).Select(book => book.Id).ToList();

        await LibraryManagementDbSeeder.SeedAsync(dbContext, configuration, CancellationToken.None);

        List<Book> seededBooks = await GetSeededBooksAsync(dbContext);

        Assert.Equal(3, seededBooks.Count);
        Assert.Equal(initialIds, seededBooks.Select(book => book.Id));
    }

    private static async Task SeedDatabaseAsync(ServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        LibraryManagementDbContext dbContext = scope.ServiceProvider.GetRequiredService<LibraryManagementDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
    }

    private static LibraryManagementDbContext CreateDbContext()
    {
        string databasePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.db");
        DbContextOptions<LibraryManagementDbContext> options = new DbContextOptionsBuilder<LibraryManagementDbContext>()
            .UseSqlite($"Data Source={databasePath}")
            .Options;

        return new LibraryManagementDbContext(options);
    }

    private static IConfiguration CreateConfiguration(bool? enabled = null, int? bookCount = null)
    {
        ConfigurationManager configuration = new();

        if (enabled is not null)
        {
            configuration["DatabaseSeeding:Enabled"] = enabled.Value.ToString();
        }

        if (bookCount is not null)
        {
            configuration["DatabaseSeeding:BookCount"] = bookCount.Value.ToString();
        }

        return configuration;
    }

    private static Task<int> CountBooksAsync(LibraryManagementDbContext dbContext) =>
        dbContext.Books.IgnoreQueryFilters().CountAsync();

    private static Task<List<Book>> GetSeededBooksAsync(LibraryManagementDbContext dbContext) =>
        dbContext.Books
            .IgnoreQueryFilters()
            .OrderBy(book => book.CreatedAtUtc)
            .ToListAsync();

    private static void AssertSeededBooks(IReadOnlyCollection<Book> seededBooks) =>
        Assert.All(seededBooks, book =>
        {
            Assert.False(book.IsDeleted);
            Assert.False(string.IsNullOrWhiteSpace(book.Title));
            Assert.False(string.IsNullOrWhiteSpace(book.Author));
            Assert.Equal(book.NormalizedIsbn, book.Isbn.Replace("-", string.Empty, StringComparison.Ordinal));
        });
}
