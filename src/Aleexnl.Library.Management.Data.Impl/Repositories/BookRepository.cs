using Aleexnl.Library.Management.Data.Contracts.Entities;
using Aleexnl.Library.Management.Data.Contracts.Repositories;
using Aleexnl.Library.Management.Data.Impl.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aleexnl.Library.Management.Data.Impl.Repositories;

public sealed class BookRepository(LibraryManagementDbContext dbContext) : IBookRepository
{
    public Task<bool>
        ExistsByNormalizedIsbnAsync(string normalizedIsbn, CancellationToken cancellationToken = default) =>
        dbContext.Books.AnyAsync(book => book.NormalizedIsbn == normalizedIsbn, cancellationToken);

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Books
            .AsNoTracking()
            .SingleOrDefaultAsync(book => book.Id == id, cancellationToken);

    public Task<Book?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Books.SingleOrDefaultAsync(book => book.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Book>> GetPageAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default) =>
        await dbContext.Books
            .AsNoTracking()
            .OrderBy(book => book.Title)
            .ThenBy(book => book.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        dbContext.Books.CountAsync(cancellationToken);

    public Task<bool> ExistsByNormalizedIsbnAsync(string normalizedIsbn, Guid excludedBookId,
        CancellationToken cancellationToken = default) =>
        dbContext.Books.AnyAsync(
            book => book.NormalizedIsbn == normalizedIsbn && book.Id != excludedBookId,
            cancellationToken);

    public Task AddAsync(Book book, CancellationToken cancellationToken = default) =>
        dbContext.Books.AddAsync(book, cancellationToken).AsTask();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
