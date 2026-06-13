# Repository Guidelines

## Project Structure & Module Organization
The solution file is [Aleexnl.Library.Management.slnx](/Users/aleexnl/RiderProjects/Aleexnl.Library.Management/Aleexnl.Library.Management.slnx). Production code lives under `src/` and is split by layer:
- `Aleexnl.Library.Management.WebAPI`: HTTP endpoints and host startup
- `*.Domain.Contracts` / `*.Domain.Impl`: domain interfaces, DTOs, and business logic
- `*.Data.Contracts` / `*.Data.Impl`: EF Core entities, repositories, and persistence
- `Aleexnl.Library.Management.Common`: shared utilities

Tests live under `test/` in matching `*.UnitTests` projects. Keep new tests near the layer they validate.

## Build, Test, and Development Commands
Use the .NET 10 SDK. If `dotnet` resolves to .NET 9 on your machine, use `~/.dotnet/dotnet`.

- `~/.dotnet/dotnet build Aleexnl.Library.Management.slnx`
  Builds the full solution.
- `~/.dotnet/dotnet test test/Aleexnl.Library.Management.Domain.UnitTests/Aleexnl.Library.Management.Domain.UnitTests.csproj`
  Runs domain unit tests.
- `~/.dotnet/dotnet run --project src/Aleexnl.Library.Management.WebAPI/Aleexnl.Library.Management.WebAPI.csproj`
  Starts the API locally.
- `~/.dotnet/dotnet test Aleexnl.Library.Management.slnx`
  Runs all test projects.

## Coding Style & Naming Conventions
This repository uses C# with nullable reference types and implicit usings enabled via `Directory.Build.props`. Follow `.editorconfig` for formatting. Use:
- 4 spaces for indentation
- `PascalCase` for public types and members
- `camelCase` for locals and parameters
- one type per file, with file names matching the type name

Prefer thin endpoints, domain logic in services, and persistence concerns in repositories.

## Testing Guidelines
Tests use xUnit. Name test methods as `Method_Scenario_ExpectedBehavior`, for example `UpdateAsync_ReturnsConflict_WhenAnotherBookHasSameNormalizedIsbn`. Add or update tests for any behavior change, especially around pagination, soft delete, and ISBN uniqueness.

## Commit & Pull Request Guidelines
Recent history follows Conventional Commits, for example `chore: add initial project configuration files` and `docs: add conventional commit guidelines and configuration`. Continue using prefixes like `feat:`, `fix:`, `docs:`, and `chore:`.

Pull requests should include a short description, the behavior changed, test evidence, and any API contract impact. Include sample requests or response notes when endpoints change.

## Configuration Notes
Connection strings are read from `appsettings.json`. Do not commit secrets. The project currently emits `NU1507` warnings because multiple NuGet sources are configured without source mapping; avoid changing package sources unless required.
