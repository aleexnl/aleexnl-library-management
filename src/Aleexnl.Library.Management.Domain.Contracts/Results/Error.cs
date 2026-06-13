namespace Aleexnl.Library.Management.Domain.Contracts.Results;

/// <summary>
/// Represents a domain error.
/// </summary>
/// <param name="Code">The error category.</param>
/// <param name="Message">The error message.</param>
public sealed record Error(ErrorCode Code, string Message);
