namespace Aleexnl.Library.Management.Domain.Contracts.Books.Dtos;

/// <summary>
/// Represents a book returned by the public API.
/// </summary>
/// <param name="Id">The unique identifier of the book.</param>
/// <param name="Title">The title of the book.</param>
/// <param name="Author">The author of the book.</param>
/// <param name="Isbn">The ISBN stored for the book.</param>
/// <param name="Description">An optional description of the book.</param>
/// <param name="PublishedOn">The optional publication date.</param>
/// <param name="PageCount">The optional number of pages.</param>
/// <param name="CreatedAtUtc">The UTC timestamp when the book was created.</param>
public sealed record BookDto(
    Guid Id,
    string Title,
    string Author,
    string Isbn,
    string? Description,
    DateOnly? PublishedOn,
    int? PageCount,
    DateTime CreatedAtUtc);
