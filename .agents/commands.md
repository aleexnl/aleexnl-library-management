# Commands

Adjust paths to match the solution. Use existing scripts or `Makefile`/`justfile` targets when present.

## Discover

```bash
find . -maxdepth 4 -name "*.sln" -o -name "*.csproj"
find . -maxdepth 4 -name "Program.cs" -o -name "appsettings*.json"
```

PowerShell equivalent:

```powershell
Get-ChildItem -Recurse -Depth 4 -Include *.sln,*.csproj,Program.cs,appsettings*.json
```

## Restore, build, test

```bash
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
```

## Format

```bash
dotnet format --verify-no-changes
dotnet format
```

## Run API locally

```bash
dotnet run --project src/<ApiProject>/<ApiProject>.csproj
dotnet watch --project src/<ApiProject>/<ApiProject>.csproj run
```

## Test with filters

```bash
dotnet test --filter "FullyQualifiedName~<TestClassOrNamespace>"
dotnet test --filter "Category=Integration"
dotnet test --collect:"XPlat Code Coverage"
```

## Package and vulnerability checks

```bash
dotnet list package --vulnerable --include-transitive
dotnet list package --outdated
```

## EF Core

```bash
dotnet ef migrations list --project src/<InfrastructureProject> --startup-project src/<ApiProject>
dotnet ef migrations add <MigrationName> --project src/<InfrastructureProject> --startup-project src/<ApiProject>
dotnet ef database update --project src/<InfrastructureProject> --startup-project src/<ApiProject>
dotnet ef migrations script --idempotent --output artifacts/migrations.sql --project src/<InfrastructureProject> --startup-project src/<ApiProject>
```

## Docker Compose when present

```bash
docker compose up -d
docker compose logs -f
docker compose down
```

## Final validation contract

At the end of a coding task, report:

- Commands run.
- Pass/fail result.
- Any command skipped and why.
- Any manual verification performed.
