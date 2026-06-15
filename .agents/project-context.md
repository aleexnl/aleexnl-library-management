# Project context

Fill this file with repository-specific facts. Agents should inspect the repo and update placeholders when asked.

## Application

- Product name: `<PROJECT_NAME>`
- API project path: `src/<ApiProject>`
- Test project paths: `tests/<UnitTests>`, `tests/<IntegrationTests>`
- .NET SDK/runtime target: inspect `global.json` and `TargetFramework` in `.csproj`
- API style: `Controllers` / `Minimal APIs` / mixed
- Persistence: `EF Core` / Dapper / external service / none
- Database provider: SQL Server / PostgreSQL / SQLite / MySQL / other
- AuthN/AuthZ: JWT bearer / cookies / Identity / external IdP / none
- OpenAPI tooling: built-in OpenAPI / Swashbuckle / NSwag / other

## Important paths

- API startup: `src/<ApiProject>/Program.cs`
- Configuration: `src/<ApiProject>/appsettings*.json`
- Controllers or endpoints: `src/<ApiProject>/Controllers` or `src/<ApiProject>/Endpoints`
- DTOs/contracts: `src/<ApiProject>/Contracts` or `src/<ApiProject>/Models`
- Application services: `src/<ApplicationProject>`
- Data access: `src/<InfrastructureProject>`
- Migrations: `src/<InfrastructureProject>/Migrations`

## Repository-specific conventions

- Error response pattern:
- Validation pattern:
- Mapping pattern:
- Logging pattern:
- Test naming pattern:
- Branch/PR requirements:

## External dependencies

List external services, emulators, containers, and local secrets required for development.

- Service:
- Local setup:
- Safe test substitute:
