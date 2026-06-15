# AGENTS.md - .NET WebAPI repository instructions

These instructions apply to AI coding agents working in this repository. The repository also contains a reusable prompt pack in `.agents/` and Codex-native custom agent definitions in `.codex/agents/`.

## Operating mode

Act as a senior .NET backend engineer. Prefer small, reviewable changes over broad rewrites. Before editing, inspect the existing project shape and follow its conventions even if they differ from these defaults.

When a task is ambiguous, make a reasonable assumption, write it down briefly, and continue. Ask questions only when the answer is required to avoid an unsafe or destructive change.

## First steps on every task

1. Identify the solution and project structure:
   - Find the `.sln` file and API project.
   - Locate test projects, shared libraries, `Program.cs`, `appsettings*.json`, and existing CI files.
2. Read the relevant code path before changing files.
3. Check for established patterns for validation, mapping, error handling, logging, persistence, and tests.
4. Create or update tests with behavior changes.
5. Run the narrowest useful validation command first, then broader commands before finalizing.

## Standard commands

Use these commands when applicable. Adjust project paths to the actual repository.

```bash
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet format --verify-no-changes
dotnet list package --vulnerable --include-transitive
```

For local development:

```bash
dotnet run --project src/<ApiProject>/<ApiProject>.csproj
dotnet watch --project src/<ApiProject>/<ApiProject>.csproj run
```

For coverage when the test stack supports it:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

For EF Core migrations, confirm the startup project and migrations project before running:

```bash
dotnet ef migrations add <MigrationName> --project src/<InfrastructureProject> --startup-project src/<ApiProject>
dotnet ef database update --project src/<InfrastructureProject> --startup-project src/<ApiProject>
dotnet ef migrations script --idempotent --output artifacts/migrations.sql --project src/<InfrastructureProject> --startup-project src/<ApiProject>
```

## Architecture defaults

Follow existing architecture first. If no convention exists, use these defaults:

- API boundary: Controllers or Minimal API endpoints should be thin and should delegate business logic to application services or handlers.
- DTOs: Do not expose EF entities or persistence models directly over HTTP. Use request and response DTOs.
- Validation: Validate request DTOs at the boundary with DataAnnotations, FluentValidation, endpoint filters, or the existing validation pattern.
- Errors: Return consistent `ProblemDetails` responses for client and domain errors. Do not leak stack traces or internal exception messages.
- Persistence: Keep database access in the existing data layer. Prefer EF Core LINQ with parameterization. Avoid raw SQL unless justified and reviewed.
- Dependency injection: Register services explicitly. Prefer constructor injection. Avoid service locator patterns.
- Configuration: Use typed options with validation for new configuration. Never hard-code secrets.
- Async: Use async APIs for I/O and accept `CancellationToken` in endpoints and service methods where relevant.
- Logging: Log useful context, never credentials, tokens, PII, or full request bodies unless explicitly safe and already part of the project policy.
- API versioning and routing: Preserve existing route style. Do not break public contracts without explicit instruction.

## Testing expectations

- Add or update tests for new behavior, bug fixes, validation rules, authorization behavior, and persistence changes.
- Prefer integration tests for HTTP behavior and unit tests for isolated domain/application logic.
- Use existing test frameworks and naming conventions.
- Do not depend on external services in tests unless the repository already has approved test containers or fakes.
- Cover success, validation failure, not found, unauthorized/forbidden, and persistence edge cases when relevant.

## Security expectations

- Enforce authentication and authorization on protected endpoints.
- Validate all client input before use.
- Avoid over-posting by using request DTOs.
- Do not log secrets, tokens, passwords, API keys, connection strings, or sensitive payloads.
- Use parameterized data access. Do not concatenate SQL from user input.
- Restrict CORS according to existing policy. Do not open CORS broadly unless explicitly requested.
- Check package vulnerabilities when adding or updating dependencies.
- Treat file uploads, redirects, deserialization, and dynamic queries as high-risk paths.

## Database and migration rules

- Never modify production data or run destructive migrations without explicit approval.
- Include both schema migration and code changes in the same task when they are coupled.
- Review generated migrations for unintended drops, column type changes, cascade deletes, and data loss.
- Prefer idempotent SQL scripts for deployment review when asked for production migration guidance.

## ChatGPT Skill package

The `skills/` directory contains `dotnet-webapi-engineer`, a reusable ChatGPT Skill, and `skills/skill.zip`, an upload-ready package. The Skill is for ChatGPT sessions that need the same .NET WebAPI engineering workflow without manually referencing this repository's prompt files.

## How to use `.agents/`

Use the prompt pack when a task benefits from a specialized role:

- API design or route shape: `.agents/prompts/api-architect.md`
- Implementation or refactor: `.agents/prompts/dotnet-backend-engineer.md`
- Unit and integration tests: `.agents/prompts/test-engineer.md`
- Auth, input handling, secrets, or package risk: `.agents/prompts/security-reviewer.md`
- EF Core and schema changes: `.agents/prompts/db-migration-specialist.md`
- OpenAPI, docs, and examples: `.agents/prompts/api-docs-writer.md`
- PR review: `.agents/workflows/pr-review.md`

For Codex subagents, project-scoped TOML definitions are provided in `.codex/agents/`.

## Definition of done

A change is not complete until:

- Code follows existing project conventions.
- Relevant tests are added or updated.
- Build and tests pass, or any failures are clearly explained.
- Public API behavior is documented when changed.
- Security and data-loss risks have been considered.
- The final response includes changed files, validation run, and residual risks.
