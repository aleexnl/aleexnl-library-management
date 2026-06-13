namespace Aleexnl.Library.Management.Domain.Contracts.Results;

/// <summary>
/// Represents a domain-level error category.
/// </summary>
public enum ErrorCode
{
    /// <summary>
    /// The resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// The operation conflicts with existing state.
    /// </summary>
    Conflict,

    /// <summary>
    /// The request failed validation.
    /// </summary>
    Validation,

    /// <summary>
    /// An unexpected error occurred.
    /// </summary>
    Unexpected
}
