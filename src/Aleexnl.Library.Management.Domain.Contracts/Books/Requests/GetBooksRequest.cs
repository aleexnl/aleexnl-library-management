using System.ComponentModel.DataAnnotations;

namespace Aleexnl.Library.Management.Domain.Contracts.Books.Requests;

/// <summary>
/// Represents the query parameters used to page through books.
/// </summary>
public sealed class GetBooksRequest
{
    /// <summary>
    /// Gets the 1-based page number.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Gets the number of items to return per page.
    /// </summary>
    [Range(1, 100)]
    public int PageSize { get; init; } = 20;
}
