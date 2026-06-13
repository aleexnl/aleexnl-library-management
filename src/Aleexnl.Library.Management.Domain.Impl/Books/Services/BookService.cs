using Aleexnl.Library.Management.Data.Contracts.Entities;
using Aleexnl.Library.Management.Data.Contracts.Repositories;
using Aleexnl.Library.Management.Domain.Contracts.Books.Dtos;
using Aleexnl.Library.Management.Domain.Contracts.Books.Requests;
using Aleexnl.Library.Management.Domain.Contracts.Books.Services;
using Aleexnl.Library.Management.Domain.Contracts.Results;

namespace Aleexnl.Library.Management.Domain.Impl.Books.Services;

public sealed class BookService(IBookRepository bookRepository) : IBookService
{
    public async Task<Result<BookDto>> CreateAsync(CreateBookRequest request,
        CancellationToken cancellationToken = default)
    {
        string normalizedIsbn = NormalizeIsbn(request.Isbn);

        if (await bookRepository.ExistsByNormalizedIsbnAsync(normalizedIsbn, cancellationToken).ConfigureAwait(false))
        {
            return Result<BookDto>.Failure(new Error(
                ErrorCode.Conflict,
                $"A book with ISBN '{request.Isbn.Trim()}' already exists."));
        }

        DateTime now = DateTime.UtcNow;
        Book book = new()
        {
            Id = Guid.CreateVersion7(),
            Title = request.Title.Trim(),
            Author = request.Author.Trim(),
            Isbn = request.Isbn.Trim(),
            NormalizedIsbn = normalizedIsbn,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            PublishedOn = request.PublishedOn,
            PageCount = request.PageCount,
            CreatedAtUtc = now
        };

        await bookRepository.AddAsync(book, cancellationToken).ConfigureAwait(false);
        await bookRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result<BookDto>.Success(new BookDto(
            book.Id,
            book.Title,
            book.Author,
            book.Isbn,
            book.Description,
            book.PublishedOn,
            book.PageCount,
            book.CreatedAtUtc));
    }

    public async Task<Result<BookDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Book? book = await bookRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

        return book is null
            ? Result<BookDto>.Failure(new Error(ErrorCode.NotFound, $"A book with id '{id}' was not found."))
            : Result<BookDto>.Success(MapToDto(book));
    }

    public async Task<Result<PagedResult<BookDto>>> GetPageAsync(GetBooksRequest request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Book> books =
            await bookRepository.GetPageAsync(request.PageNumber, request.PageSize, cancellationToken)
                .ConfigureAwait(false);
        int totalCount = await bookRepository.CountAsync(cancellationToken).ConfigureAwait(false);

        return Result<PagedResult<BookDto>>.Success(new PagedResult<BookDto>(
            books.Select(MapToDto).ToArray(),
            request.PageNumber,
            request.PageSize,
            totalCount));
    }

    public async Task<Result<BookDto>> UpdateAsync(Guid id, UpdateBookRequest request,
        CancellationToken cancellationToken = default)
    {
        Book? book = await bookRepository.GetByIdForUpdateAsync(id, cancellationToken).ConfigureAwait(false);

        if (book is null)
        {
            return Result<BookDto>.Failure(new Error(ErrorCode.NotFound, $"A book with id '{id}' was not found."));
        }

        string normalizedIsbn = NormalizeIsbn(request.Isbn);

        if (await bookRepository.ExistsByNormalizedIsbnAsync(normalizedIsbn, id, cancellationToken)
                .ConfigureAwait(false))
        {
            return Result<BookDto>.Failure(new Error(
                ErrorCode.Conflict,
                $"A book with ISBN '{request.Isbn.Trim()}' already exists."));
        }

        book.Title = request.Title.Trim();
        book.Author = request.Author.Trim();
        book.Isbn = request.Isbn.Trim();
        book.NormalizedIsbn = normalizedIsbn;
        book.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        book.PublishedOn = request.PublishedOn;
        book.PageCount = request.PageCount;
        book.UpdatedAtUtc = DateTime.UtcNow;

        await bookRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result<BookDto>.Success(MapToDto(book));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Book? book = await bookRepository.GetByIdForUpdateAsync(id, cancellationToken).ConfigureAwait(false);

        if (book is null)
        {
            return Result.Failure(new Error(ErrorCode.NotFound, $"A book with id '{id}' was not found."));
        }

        book.IsDeleted = true;
        book.DeletedAtUtc = DateTime.UtcNow;
        book.UpdatedAtUtc = book.DeletedAtUtc;

        await bookRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success;
    }

    internal static string NormalizeIsbn(string isbn) =>
        new string(isbn.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();

    private static BookDto MapToDto(Book book) =>
        new(
            book.Id,
            book.Title,
            book.Author,
            book.Isbn,
            book.Description,
            book.PublishedOn,
            book.PageCount,
            book.CreatedAtUtc);
}
