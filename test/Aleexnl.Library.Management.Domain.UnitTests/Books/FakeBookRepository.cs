using Aleexnl.Library.Management.Data.Contracts.Entities;
using Aleexnl.Library.Management.Data.Contracts.Repositories;

namespace Aleexnl.Library.Management.Domain.UnitTests.Books;

internal sealed class FakeBookRepository : IBookRepository
{
    public List<Book> Books { get; } = [];

    public bool ExistsByNormalizedIsbnResult { get; init; }

    public Task<bool> ExistsByNormalizedIsbnAsync(string normalizedIsbn,
        CancellationToken cancellationToken = default) => Task.FromResult(ExistsByNormalizedIsbnResult ||
                                                                          Books.Any(book =>
                                                                              book.NormalizedIsbn ==
                                                                              normalizedIsbn));

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Books.SingleOrDefault(book => book.Id == id));

    public Task<Book?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Books.SingleOrDefault(book => book.Id == id));

    public Task<IReadOnlyList<Book>> GetPageAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Book> page = Books
            .OrderBy(book => book.Title)
            .ThenBy(book => book.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return Task.FromResult(page);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default) => Task.FromResult(Books.Count);

    public Task<bool> ExistsByNormalizedIsbnAsync(string normalizedIsbn, Guid excludedBookId,
        CancellationToken cancellationToken = default) => Task.FromResult(Books.Any(book =>
        book.NormalizedIsbn == normalizedIsbn && book.Id != excludedBookId));

    public Task AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        Books.Add(book);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
}
