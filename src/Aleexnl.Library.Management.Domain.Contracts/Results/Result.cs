namespace Aleexnl.Library.Management.Domain.Contracts.Results;

/// <summary>
/// Represents the outcome of an operation without a return value.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="error">The operation error when the result is not successful.</param>
    protected internal Result(Error? error) => Error = error;

    /// <summary>
    /// Gets a successful result.
    /// </summary>
    public static Result Success { get; } = new(null);

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess => Error is null;

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the operation error when the result failed.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The operation error.</param>
    /// <returns>A failed result.</returns>
    public static Result Failure(Error error) => new(error);
}
