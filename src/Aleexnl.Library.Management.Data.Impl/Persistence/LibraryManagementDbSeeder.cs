using Aleexnl.Library.Management.Data.Contracts.Entities;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Aleexnl.Library.Management.Data.Impl.Persistence;

/// <summary>
///     Seeds the library management database with sample data for local environments.
/// </summary>
internal static class LibraryManagementDbSeeder
{
    private const int DefaultBookCount = 25;
    private const string SeedingEnabledKey = "DatabaseSeeding:Enabled";
    private const string SeedingBookCountKey = "DatabaseSeeding:BookCount";

    private static readonly DateTime SeedCreatedAtUtc = new(2024, 01, 01, 00, 00, 00, DateTimeKind.Utc);

    public static void Seed(DbContext dbContext, IConfiguration configuration)
    {
        if (!ShouldSeed(dbContext, configuration))
        {
            return;
        }

        dbContext.Set<Book>().AddRange(CreateBooks(GetBookCount(configuration)));
        dbContext.SaveChanges();
    }

    public static async Task SeedAsync(DbContext dbContext, IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        if (!await ShouldSeedAsync(dbContext, configuration, cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        await dbContext.Set<Book>().AddRangeAsync(CreateBooks(GetBookCount(configuration)), cancellationToken)
            .ConfigureAwait(false);
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static bool ShouldSeed(DbContext dbContext, IConfiguration configuration) =>
        IsSeedingEnabled(configuration) && !dbContext.Set<Book>().IgnoreQueryFilters().Any();

    private static async Task<bool> ShouldSeedAsync(
        DbContext dbContext,
        IConfiguration configuration,
        CancellationToken cancellationToken) =>
        IsSeedingEnabled(configuration) &&
        !await dbContext.Set<Book>().IgnoreQueryFilters().AnyAsync(cancellationToken).ConfigureAwait(false);

    private static bool IsSeedingEnabled(IConfiguration configuration) =>
        !bool.TryParse(configuration[SeedingEnabledKey], out bool enabled) || enabled;

    private static int GetBookCount(IConfiguration configuration) =>
        int.TryParse(configuration[SeedingBookCountKey], out int bookCount) && bookCount > 0
            ? bookCount
            : DefaultBookCount;

    private static Book[] CreateBooks(int count) =>
        Enumerable.Range(1, count)
            .Select(CreateBook)
            .ToArray();

    private static Book CreateBook(int seed)
    {
        string normalizedIsbn = CreateNormalizedIsbn(seed);

        return new Faker<Book>()
            .UseSeed(seed)
            .RuleFor(book => book.Id, _ => Guid.NewGuid())
            .RuleFor(book => book.Title,
                faker => Truncate(faker.Lorem.Sentence(faker.Random.Int(2, 5)).TrimEnd('.'), 200))
            .RuleFor(book => book.Author, faker => Truncate(faker.Name.FullName(), 200))
            .RuleFor(book => book.Isbn, _ => FormatIsbn(normalizedIsbn))
            .RuleFor(book => book.NormalizedIsbn, _ => normalizedIsbn)
            .RuleFor(book => book.Description, faker => Truncate(faker.Lorem.Paragraphs(faker.Random.Int(1, 2)), 2000))
            .RuleFor(book => book.PublishedOn,
                faker => DateOnly.FromDateTime(faker.Date.Past(50,
                    new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc))))
            .RuleFor(book => book.PageCount, faker => faker.Random.Int(120, 900))
            .RuleFor(book => book.CreatedAtUtc, _ => SeedCreatedAtUtc.AddMinutes(seed))
            .RuleFor(book => book.UpdatedAtUtc, _ => null)
            .RuleFor(book => book.IsDeleted, _ => false)
            .RuleFor(book => book.DeletedAtUtc, _ => null)
            .Generate();
    }

    private static string CreateNormalizedIsbn(int seed) => $"978{seed:D10}";

    private static string FormatIsbn(string normalizedIsbn) =>
        $"{normalizedIsbn[..3]}-{normalizedIsbn[3]}-{normalizedIsbn[4..7]}-{normalizedIsbn[7..12]}-{normalizedIsbn[12]}";

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..maxLength];
}
