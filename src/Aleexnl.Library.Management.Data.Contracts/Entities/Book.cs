namespace Aleexnl.Library.Management.Data.Contracts.Entities;

/// <summary>
/// Represents the persisted book entity.
/// </summary>
public sealed class Book
{
    /// <summary>
    /// Gets or sets the unique identifier of the book.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the book.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the author of the book.
    /// </summary>
    public required string Author { get; set; }

    /// <summary>
    /// Gets or sets the ISBN stored for the book.
    /// </summary>
    public required string Isbn { get; set; }

    /// <summary>
    /// Gets or sets the normalized ISBN used for lookups and uniqueness checks.
    /// </summary>
    public required string NormalizedIsbn { get; set; }

    /// <summary>
    /// Gets or sets the optional description of the book.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the optional publication date.
    /// </summary>
    public DateOnly? PublishedOn { get; set; }

    /// <summary>
    /// Gets or sets the optional number of pages.
    /// </summary>
    public int? PageCount { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the book was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp of the last update.
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the book is soft-deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the book was soft-deleted.
    /// </summary>
    public DateTime? DeletedAtUtc { get; set; }
}
