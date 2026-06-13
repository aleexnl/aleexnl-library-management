namespace Aleexnl.Library.Management.Domain.Contracts.Books.Dtos;

/// <summary>
/// Represents a page of items and the associated pagination metadata.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
/// <param name="Items">The items in the current page.</param>
/// <param name="PageNumber">The current 1-based page number.</param>
/// <param name="PageSize">The requested page size.</param>
/// <param name="TotalCount">The total number of items across all pages.</param>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount);
