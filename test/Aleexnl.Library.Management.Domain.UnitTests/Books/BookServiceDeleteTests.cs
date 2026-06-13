using Aleexnl.Library.Management.Data.Contracts.Entities;
using Aleexnl.Library.Management.Domain.Contracts.Results;
using Aleexnl.Library.Management.Domain.Impl.Books.Services;

namespace Aleexnl.Library.Management.Domain.UnitTests.Books;

public sealed class BookServiceDeleteTests
{
    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenBookDoesNotExist()
    {
        BookService service = new(new FakeBookRepository());

        Result result = await service.DeleteAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorCode.NotFound, result.Error!.Code);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesBook_WhenBookExists()
    {
        Book existing = BookTestFactory.CreateBook("1", "Original");
        FakeBookRepository repository = new();
        repository.Books.Add(existing);
        BookService service = new(repository);

        Result result = await service.DeleteAsync(existing.Id);

        Assert.True(result.IsSuccess);
        Assert.True(existing.IsDeleted);
        Assert.NotNull(existing.DeletedAtUtc);
        Assert.NotNull(existing.UpdatedAtUtc);
    }
}
