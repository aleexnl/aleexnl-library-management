using Aleexnl.Library.Management.Data.Contracts.Entities;
using Aleexnl.Library.Management.Domain.Contracts.Books.Dtos;
using Aleexnl.Library.Management.Domain.Contracts.Books.Requests;
using Aleexnl.Library.Management.Domain.Contracts.Results;
using Aleexnl.Library.Management.Domain.Impl.Books.Services;

namespace Aleexnl.Library.Management.Domain.UnitTests.Books;

public sealed class BookServiceGetTests
{
    [Fact]
    public async Task GetByIdAsync_ReturnsBook_WhenBookExists()
    {
        Book book = new()
        {
            Id = Guid.NewGuid(),
            Title = "Patterns of Enterprise Application Architecture",
            Author = "Martin Fowler",
            Isbn = "978-0321127426",
            NormalizedIsbn = "9780321127426",
            CreatedAtUtc = DateTime.UtcNow
        };

        FakeBookRepository repository = new();
        repository.Books.Add(book);
        BookService service = new(repository);

        Result<BookDto> result = await service.GetByIdAsync(book.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(book.Id, result.Value.Id);
        Assert.Equal(book.Title, result.Value.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenBookDoesNotExist()
    {
        BookService service = new(new FakeBookRepository());

        Result<BookDto> result = await service.GetByIdAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.NotFound, result.Error!.Code);
    }

    [Fact]
    public async Task GetPageAsync_ReturnsRequestedPage_AndTotalCount()
    {
        FakeBookRepository repository = new();
        repository.Books.AddRange(
            BookTestFactory.CreateBook("b", "B"),
            BookTestFactory.CreateBook("a", "A"),
            BookTestFactory.CreateBook("c", "C"));

        BookService service = new(repository);

        Result<PagedResult<BookDto>> result =
            await service.GetPageAsync(new GetBooksRequest { PageNumber = 2, PageSize = 1 });

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.PageNumber);
        Assert.Equal(1, result.Value.PageSize);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Single(result.Value.Items);
        Assert.Equal("B", result.Value.Items.Single().Title);
    }
}
