using Aleexnl.Library.Management.Data.Contracts.Entities;

namespace Aleexnl.Library.Management.Domain.UnitTests.Books;

internal static class BookTestFactory
{
    public static Book CreateBook(string isbnSuffix, string title) =>
        new()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = "Author",
            Isbn = $"978-0-{isbnSuffix}",
            NormalizedIsbn = $"9780{isbnSuffix}",
            CreatedAtUtc = DateTime.UtcNow
        };
}
