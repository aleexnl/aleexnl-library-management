using Aleexnl.Library.Management.Domain.Contracts.Books.Dtos;
using Aleexnl.Library.Management.Domain.Contracts.Books.Requests;
using Aleexnl.Library.Management.Domain.Contracts.Books.Services;
using Aleexnl.Library.Management.Domain.Contracts.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Aleexnl.Library.Management.WebAPI.Endpoints.Books;

/// <summary>
/// Maps HTTP endpoints for book operations.
/// </summary>
public static class BookEndpoints
{
    /// <summary>
    /// Maps the book endpoints into the application route table.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/books")
            .WithTags("Books")
            .RequireAuthorization();

        group.MapGet("/", GetBooksAsync)
            .WithName("GetBooks")
            .WithSummary("Returns a paginated list of books.")
            .WithDescription("Returns non-deleted books using 1-based pagination.");

        group.MapGet("/{id:guid}", GetBookByIdAsync)
            .WithName("GetBookById")
            .WithSummary("Returns a single book by id.")
            .WithDescription("Returns a non-deleted book when it exists.");

        group.MapPost("/", CreateBookAsync)
            .WithName("CreateBook")
            .WithSummary("Creates a new book.")
            .WithDescription("Creates a new book record. ISBN must be unique among non-deleted books.");

        group.MapPut("/{id:guid}", UpdateBookAsync)
            .WithName("UpdateBook")
            .WithSummary("Updates an existing book.")
            .WithDescription("Updates a non-deleted book by id. ISBN must remain unique among non-deleted books.");

        group.MapDelete("/{id:guid}", DeleteBookAsync)
            .WithName("DeleteBook")
            .WithSummary("Soft-deletes an existing book.")
            .WithDescription("Marks a non-deleted book as deleted instead of removing it from the database.");

        return app;
    }

    private static async Task<Results<Created<BookDto>, Conflict<ProblemDetails>>> CreateBookAsync(
        CreateBookRequest request,
        IBookService bookService,
        LinkGenerator linkGenerator,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        Result<BookDto> result = await bookService.CreateAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return ToConflict(result.Error!);
        }

        BookDto book = result.Value!;
        string location = linkGenerator.GetUriByName(httpContext, "GetBookById", new { id = book.Id })
                          ?? $"/api/books/{book.Id}";

        return TypedResults.Created(location, book);
    }

    private static async Task<Ok<PagedResult<BookDto>>> GetBooksAsync(
        [AsParameters] GetBooksRequest request,
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        Result<PagedResult<BookDto>> result = await bookService.GetPageAsync(request, cancellationToken);

        return TypedResults.Ok(result.Value!);
    }

    private static async Task<Results<Ok<BookDto>, NotFound>> GetBookByIdAsync(
        Guid id,
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        Result<BookDto> result = await bookService.GetByIdAsync(id, cancellationToken);

        return result.IsFailure
            ? TypedResults.NotFound()
            : TypedResults.Ok(result.Value!);
    }

    private static async Task<Results<Ok<BookDto>, NotFound, Conflict<ProblemDetails>>> UpdateBookAsync(
        Guid id,
        UpdateBookRequest request,
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        Result<BookDto> result = await bookService.UpdateAsync(id, request, cancellationToken);

        return result.IsFailure switch
        {
            true when result.Error!.Code == ErrorCode.NotFound => TypedResults.NotFound(),
            true when result.Error!.Code == ErrorCode.Conflict => ToConflict(result.Error),
            _ => TypedResults.Ok(result.Value!)
        };
    }

    private static async Task<Results<NoContent, NotFound>> DeleteBookAsync(
        Guid id,
        IBookService bookService,
        CancellationToken cancellationToken)
    {
        Result result = await bookService.DeleteAsync(id, cancellationToken);

        return result.IsFailure
            ? TypedResults.NotFound()
            : TypedResults.NoContent();
    }

    private static Conflict<ProblemDetails> ToConflict(Error error) =>
        TypedResults.Conflict(new ProblemDetails
        {
            Title = "Book already exists", Detail = error.Message, Status = StatusCodes.Status409Conflict
        });
}
