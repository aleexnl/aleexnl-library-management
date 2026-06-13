namespace Aleexnl.Library.Management.Domain.Contracts.Results;

/// <summary>
/// Represents the outcome of an operation with a return value.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public sealed class Result<T> : Result
{
    private Result(T? value, Error? error)
        : base(error) =>
        Value = value;

    /// <summary>
    /// Gets the successful value.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="value">The successful value.</param>
    /// <returns>A successful result.</returns>
    public static new Result<T> Success(T value) => new(value, null);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The operation error.</param>
    /// <returns>A failed result.</returns>
    public static new Result<T> Failure(Error error) => new(default, error);
}
