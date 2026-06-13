# Aleexnl.Library.Management

Aleexnl.Library.Management is a layered ASP.NET Core Web API for managing a library catalog. The current implementation
focuses on book management with SQLite persistence, domain-level validation, soft delete behavior, and paginated reads.

This repository uses a pragmatic clean separation between Web API, domain, and data layers so HTTP concerns, business
rules, and persistence stay isolated.

## Features

- Create, read, update, and soft-delete books
- Enforce ISBN uniqueness using a normalized ISBN value
- Return paginated book lists with configurable page size
- Persist data with Entity Framework Core and SQLite
- Expose OpenAPI metadata in development
- Cover domain behavior with xUnit tests

## Project Status

This project is in active early development. The current API surface is centered on book management and the README
documents what exists today.

## Requirements

- .NET 10 SDK
- `~/.dotnet/dotnet` if your default `dotnet` command resolves to an older SDK
- Docker Desktop or a compatible Docker runtime if you want to build the container image

## Installation

1. Clone the repository.
2. Restore dependencies:

```bash
~/.dotnet/dotnet restore Aleexnl.Library.Management.slnx
```

3. Build the solution:

```bash
~/.dotnet/dotnet build Aleexnl.Library.Management.slnx
```

## Configuration

The API reads its connection string from `src/Aleexnl.Library.Management.WebAPI/appsettings.json`:

```json
{
    "ConnectionStrings": {
        "LibraryManagement": "Data Source=library-management.db"
    }
}
```

If the connection string is not provided, the application falls back to `Data Source=library-management.db`.

The database schema is created automatically on startup through `EnsureCreatedAsync()`, which makes local setup simple
but is not a replacement for production-grade migrations.

## Usage

Run the API locally:

```bash
~/.dotnet/dotnet run --project src/Aleexnl.Library.Management.WebAPI/Aleexnl.Library.Management.WebAPI.csproj
```

In development, the default local URLs are:

- `http://localhost:5270`
- `https://localhost:7209`

OpenAPI is available in development when the app is running:

- `GET /openapi/v1.json`

### Example Requests

Create a book:

```bash
curl -X POST http://localhost:5270/api/books/ \
  -H "Content-Type: application/json" \
  -d '{
    "title": "The Pragmatic Programmer",
    "author": "Andrew Hunt",
    "isbn": "978-0201616224",
    "description": "Classic software engineering book.",
    "publishedOn": "1999-10-30",
    "pageCount": 352
  }'
```

Get the first page of books:

```bash
curl "http://localhost:5270/api/books?pageNumber=1&pageSize=10"
```

Get a book by id:

```bash
curl http://localhost:5270/api/books/{bookId}
```

Update a book:

```bash
curl -X PUT http://localhost:5270/api/books/{bookId} \
  -H "Content-Type: application/json" \
  -d '{
    "title": "The Pragmatic Programmer (20th Anniversary Edition)",
    "author": "Andrew Hunt",
    "isbn": "978-0135957059",
    "description": "Updated edition.",
    "publishedOn": "2019-09-13",
    "pageCount": 352
  }'
```

Soft-delete a book:

```bash
curl -X DELETE http://localhost:5270/api/books/{bookId}
```

### API Notes

- Pagination is 1-based.
- `pageSize` accepts values from `1` to `100`.
- `title` and `author` are required and limited to 200 characters.
- `isbn` is required, must be between 10 and 32 characters, and must be unique after normalization.
- `description` is optional and limited to 2000 characters.
- `pageCount` is optional and must be between `1` and `100000`.
- Deleted books are soft-deleted instead of being physically removed.

## Project Structure

```text
src/
  Aleexnl.Library.Management.WebAPI          HTTP endpoints and application startup
  Aleexnl.Library.Management.Domain.Contracts Contracts, DTOs, and result models
  Aleexnl.Library.Management.Domain.Impl      Domain services and business rules
  Aleexnl.Library.Management.Data.Contracts   Entities and repository contracts
  Aleexnl.Library.Management.Data.Impl        EF Core persistence and repository implementations
  Aleexnl.Library.Management.Common           Shared utilities
test/
  Aleexnl.Library.Management.Domain.UnitTests Domain service tests
  Aleexnl.Library.Management.Data.UnitTests   Data-layer test project
  Aleexnl.Library.Management.Common.UnitTests Common-layer test project
```

## Testing

Run the full test suite:

```bash
~/.dotnet/dotnet test Aleexnl.Library.Management.slnx
```

Run only the domain tests:

```bash
~/.dotnet/dotnet test test/Aleexnl.Library.Management.Domain.UnitTests/Aleexnl.Library.Management.Domain.UnitTests.csproj
```

At the time this README was updated, the solution test run completed successfully with 12 passing domain tests. The
Common and Data test projects build, but currently do not contain discovered tests.

## Docker

Build the container image with Docker Compose:

```bash
docker compose build
```

The repository includes:

- `compose.yaml` for local image builds
- `src/Aleexnl.Library.Management.WebAPI/Dockerfile` for the Web API container image

## Contributing

Contributions are welcome. For significant changes, open an issue first to discuss the proposed behavior or API impact.

Before opening a pull request:

1. Keep endpoint code thin and place business rules in the domain layer.
2. Follow the existing C# conventions from `.editorconfig`.
3. Add or update tests for behavior changes, especially around pagination, soft delete, and ISBN uniqueness.
4. Run:

```bash
~/.dotnet/dotnet test Aleexnl.Library.Management.slnx
```

Commit messages should follow Conventional Commits such as `feat:`, `fix:`, `docs:`, or `chore:`.

## Support

Use the repository issue tracker for bugs, questions, and feature requests.

## License

No license file is currently present in this repository. Add a license before treating the project as open source or
redistributing it.
