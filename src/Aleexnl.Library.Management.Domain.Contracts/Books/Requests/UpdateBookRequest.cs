using System.ComponentModel.DataAnnotations;

namespace Aleexnl.Library.Management.Domain.Contracts.Books.Requests;

/// <summary>
/// Represents the payload required to update an existing book.
/// </summary>
public sealed class UpdateBookRequest
{
    /// <summary>
    /// Gets the title of the book.
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the author of the book.
    /// </summary>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Author { get; init; } = string.Empty;

    /// <summary>
    /// Gets the ISBN provided for the book.
    /// </summary>
    [Required]
    [StringLength(32, MinimumLength = 10)]
    public string Isbn { get; init; } = string.Empty;

    /// <summary>
    /// Gets the optional description of the book.
    /// </summary>
    [StringLength(2000)]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the optional publication date of the book.
    /// </summary>
    public DateOnly? PublishedOn { get; init; }

    /// <summary>
    /// Gets the optional number of pages in the book.
    /// </summary>
    [Range(1, 100_000)]
    public int? PageCount { get; init; }
}
