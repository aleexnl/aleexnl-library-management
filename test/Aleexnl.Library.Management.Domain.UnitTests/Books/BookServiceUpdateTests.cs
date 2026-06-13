using Aleexnl.Library.Management.Data.Contracts.Entities;
using Aleexnl.Library.Management.Domain.Contracts.Books.Dtos;
using Aleexnl.Library.Management.Domain.Contracts.Books.Requests;
using Aleexnl.Library.Management.Domain.Contracts.Results;
using Aleexnl.Library.Management.Domain.Impl.Books.Services;

namespace Aleexnl.Library.Management.Domain.UnitTests.Books;

public sealed class BookServiceUpdateTests
{
    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenBookDoesNotExist()
    {
        BookService service = new(new FakeBookRepository());

        Result<BookDto> result = await service.UpdateAsync(Guid.NewGuid(),
            new UpdateBookRequest
            {
                Title = "Clean Architecture", Author = "Robert C. Martin", Isbn = "978-0134494166"
            });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.NotFound, result.Error!.Code);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsConflict_WhenAnotherBookHasSameNormalizedIsbn()
    {
        Book target = BookTestFactory.CreateBook("1", "Target");
        Book other = BookTestFactory.CreateBook("2", "Other");
        other.NormalizedIsbn = "9780134494166";
        other.Isbn = "978-0134494166";

        FakeBookRepository repository = new();
        repository.Books.AddRange(target, other);
        BookService service = new(repository);

        Result<BookDto> result = await service.UpdateAsync(target.Id,
            new UpdateBookRequest { Title = "Updated", Author = "Author", Isbn = "978-0134494166" });

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.Conflict, result.Error!.Code);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingBook_WhenRequestIsValid()
    {
        Book existing = BookTestFactory.CreateBook("1", "Original");
        FakeBookRepository repository = new();
        repository.Books.Add(existing);
        BookService service = new(repository);

        Result<BookDto> result = await service.UpdateAsync(existing.Id,
            new UpdateBookRequest
            {
                Title = " Updated Title ",
                Author = " Updated Author ",
                Isbn = "978-0132350884",
                Description = " Updated description ",
                PageCount = 321
            });

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated Title", existing.Title);
        Assert.Equal("Updated Author", existing.Author);
        Assert.Equal("978-0132350884", existing.Isbn);
        Assert.Equal("9780132350884", existing.NormalizedIsbn);
        Assert.Equal("Updated description", existing.Description);
        Assert.Equal(321, existing.PageCount);
        Assert.NotNull(existing.UpdatedAtUtc);
    }
}
