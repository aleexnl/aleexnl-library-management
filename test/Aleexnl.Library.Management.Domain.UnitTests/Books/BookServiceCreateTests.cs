using Aleexnl.Library.Management.Data.Contracts.Entities;
using Aleexnl.Library.Management.Domain.Contracts.Books.Dtos;
using Aleexnl.Library.Management.Domain.Contracts.Books.Requests;
using Aleexnl.Library.Management.Domain.Contracts.Results;
using Aleexnl.Library.Management.Domain.Impl.Books.Services;

namespace Aleexnl.Library.Management.Domain.UnitTests.Books;

public sealed class BookServiceCreateTests
{
    [Fact]
    public async Task CreateAsync_ReturnsConflict_WhenIsbnAlreadyExists()
    {
        FakeBookRepository repository = new() { ExistsByNormalizedIsbnResult = true };
        BookService service = new(repository);

        Result<BookDto> result = await service.CreateAsync(new CreateBookRequest
        {
            Title = "Domain-Driven Design", Author = "Eric Evans", Isbn = "978-0321125217"
        });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.Conflict, result.Error!.Code);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task CreateAsync_PersistsTrimmedBook_WhenRequestIsValid()
    {
        FakeBookRepository repository = new();
        BookService service = new(repository);

        Result<BookDto> result = await service.CreateAsync(new CreateBookRequest
        {
            Title = " Clean Code ",
            Author = " Robert C. Martin ",
            Isbn = "978-0132350884",
            Description = " A handbook of agile software craftsmanship. ",
            PageCount = 464
        });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(repository.Books);

        Book savedBook = repository.Books.Single();
        Assert.Equal("Clean Code", savedBook.Title);
        Assert.Equal("Robert C. Martin", savedBook.Author);
        Assert.Equal("978-0132350884", savedBook.Isbn);
        Assert.Equal("9780132350884", savedBook.NormalizedIsbn);
        Assert.Equal("A handbook of agile software craftsmanship.", savedBook.Description);
        Assert.Equal(464, savedBook.PageCount);
        Assert.NotEqual(Guid.Empty, savedBook.Id);
    }

    [Theory]
    [InlineData("978-0132350884", "9780132350884")]
    [InlineData("isbn 978 0 13 235088 4", "ISBN9780132350884")]
    public async Task CreateAsync_NormalizesIsbn_WhenPersisting(string input, string expected)
    {
        FakeBookRepository repository = new();
        BookService service = new(repository);

        await service.CreateAsync(new CreateBookRequest
        {
            Title = "Refactoring", Author = "Martin Fowler", Isbn = input
        });

        Assert.Equal(expected, repository.Books.Single().NormalizedIsbn);
    }
}
