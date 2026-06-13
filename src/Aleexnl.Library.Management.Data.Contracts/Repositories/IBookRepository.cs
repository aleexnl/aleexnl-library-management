using Aleexnl.Library.Management.Data.Contracts.Entities;

namespace Aleexnl.Library.Management.Data.Contracts.Repositories;

/// <summary>
/// Provides persistence operations for books.
/// </summary>
public interface IBookRepository
{
    /// <summary>
    /// Determines whether a non-deleted book exists for the provided normalized ISBN.
    /// </summary>
    /// <param name="normalizedIsbn">The normalized ISBN to search for.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns><see langword="true"/> when a matching book exists; otherwise, <see langword="false"/>.</returns>
    Task<bool> ExistsByNormalizedIsbnAsync(string normalizedIsbn, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a non-deleted book by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the book.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The matching book when found; otherwise, <see langword="null"/>.</returns>
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a non-deleted book by its identifier for update operations.
    /// </summary>
    /// <param name="id">The identifier of the book.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The tracked matching book when found; otherwise, <see langword="null"/>.</returns>
    Task<Book?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a page of non-deleted books.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>A page of books ordered for stable pagination.</returns>
    Task<IReadOnlyList<Book>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the total number of non-deleted books.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The total count of books.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether another non-deleted book exists for the provided normalized ISBN.
    /// </summary>
    /// <param name="normalizedIsbn">The normalized ISBN to search for.</param>
    /// <param name="excludedBookId">The identifier to exclude from the check.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns><see langword="true"/> when another matching book exists; otherwise, <see langword="false"/>.</returns>
    Task<bool> ExistsByNormalizedIsbnAsync(string normalizedIsbn, Guid excludedBookId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a book to the current unit of work.
    /// </summary>
    /// <param name="book">The book to add.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    Task AddAsync(Book book, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists pending changes to the underlying store.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
