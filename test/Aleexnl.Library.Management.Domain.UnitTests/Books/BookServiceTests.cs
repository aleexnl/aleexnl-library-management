using Aleexnl.Library.Management.Data.Contracts.Entities;
using Aleexnl.Library.Management.Data.Contracts.Repositories;
using Aleexnl.Library.Management.Domain.Contracts.Books.Dtos;
using Aleexnl.Library.Management.Domain.Contracts.Books.Requests;
using Aleexnl.Library.Management.Domain.Contracts.Results;
using Aleexnl.Library.Management.Domain.Impl.Books.Services;

namespace Aleexnl.Library.Management.Domain.UnitTests.Books;

public sealed class BookServiceTests
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
            CreateBook("b", "B"),
            CreateBook("a", "A"),
            CreateBook("c", "C"));

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
        Book target = CreateBook("1", "Target");
        Book other = CreateBook("2", "Other");
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
        Book existing = CreateBook("1", "Original");
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
        Book existing = CreateBook("1", "Original");
        FakeBookRepository repository = new();
        repository.Books.Add(existing);
        BookService service = new(repository);

        Result result = await service.DeleteAsync(existing.Id);

        Assert.True(result.IsSuccess);
        Assert.True(existing.IsDeleted);
        Assert.NotNull(existing.DeletedAtUtc);
        Assert.NotNull(existing.UpdatedAtUtc);
    }

    private static Book CreateBook(string isbnSuffix, string title) =>
        new()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = "Author",
            Isbn = $"978-0-{isbnSuffix}",
            NormalizedIsbn = $"9780{isbnSuffix}",
            CreatedAtUtc = DateTime.UtcNow
        };

    private sealed class FakeBookRepository : IBookRepository
    {
        public List<Book> Books { get; } = [];

        public bool ExistsByNormalizedIsbnResult { get; init; }

        public Task<bool> ExistsByNormalizedIsbnAsync(string normalizedIsbn,
            CancellationToken cancellationToken = default) => Task.FromResult(ExistsByNormalizedIsbnResult ||
                                                                              Books.Any(book =>
                                                                                  book.NormalizedIsbn ==
                                                                                  normalizedIsbn));

        public Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Books.SingleOrDefault(book => book.Id == id));

        public Task<Book?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Books.SingleOrDefault(book => book.Id == id));

        public Task<IReadOnlyList<Book>> GetPageAsync(int pageNumber, int pageSize,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<Book> page = Books
                .OrderBy(book => book.Title)
                .ThenBy(book => book.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArray();

            return Task.FromResult(page);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default) => Task.FromResult(Books.Count);

        public Task<bool> ExistsByNormalizedIsbnAsync(string normalizedIsbn, Guid excludedBookId,
            CancellationToken cancellationToken = default) => Task.FromResult(Books.Any(book =>
            book.NormalizedIsbn == normalizedIsbn && book.Id != excludedBookId));

        public Task AddAsync(Book book, CancellationToken cancellationToken = default)
        {
            Books.Add(book);
            return Task.CompletedTask;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }
}
