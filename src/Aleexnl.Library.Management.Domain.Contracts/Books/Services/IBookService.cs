using Aleexnl.Library.Management.Domain.Contracts.Books.Dtos;
using Aleexnl.Library.Management.Domain.Contracts.Books.Requests;
using Aleexnl.Library.Management.Domain.Contracts.Results;

namespace Aleexnl.Library.Management.Domain.Contracts.Books.Services;

/// <summary>
/// Provides book-related domain operations.
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Creates a new book.
    /// </summary>
    /// <param name="request">The data required to create the book.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The result of the create operation.</returns>
    Task<Result<BookDto>> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a book by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the book.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The matching book when found; otherwise, <see langword="null"/>.</returns>
    Task<Result<BookDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a page of books.
    /// </summary>
    /// <param name="request">The paging request.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The requested page of books.</returns>
    Task<Result<PagedResult<BookDto>>> GetPageAsync(GetBooksRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing book.
    /// </summary>
    /// <param name="id">The identifier of the book to update.</param>
    /// <param name="request">The replacement data for the book.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The result of the update operation.</returns>
    Task<Result<BookDto>> UpdateAsync(Guid id, UpdateBookRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft-deletes an existing book.
    /// </summary>
    /// <param name="id">The identifier of the book to delete.</param>
    /// <param name="cancellationToken">The cancellation token for the operation.</param>
    /// <returns>The result of the delete operation.</returns>
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
