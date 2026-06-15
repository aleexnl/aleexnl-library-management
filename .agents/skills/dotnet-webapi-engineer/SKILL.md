---
name: dotnet-webapi-engineer
description: build, modify, test, review, and document asp.net core webapi projects in c#. use when the user asks for .net webapi feature work, endpoint design, controller or minimal api changes, dto and validation design, ef core persistence, migrations, integration tests, security review, openapi documentation, bug fixes, pull request review, or repository setup guidance for a c# backend api. also use when applying an .agents or agens.md style workflow to a .net webapi app.
---

# .NET WebAPI Engineer

## Core behavior

Treat every task as repository-specific engineering work. Inspect the solution, project files, existing folder structure, tests, middleware, dependency injection, persistence pattern, auth pattern, and API style before proposing or editing code.

Prefer the smallest coherent change that satisfies the request. Preserve the existing controller vs minimal API style, test framework, validation library, persistence abstraction, error contract, and naming conventions unless the user explicitly asks to change them.

If a repository includes `AGENTS.md`, `.agents/`, `.codex/agents/`, `.github/copilot-instructions.md`, or equivalent local instructions, read and follow those files before applying this skill. Local instructions override the defaults in this skill.

## Workflow

1. **Map the repository**
   - Locate solution and project files with `find . -maxdepth 4 -name "*.sln" -o -name "*.csproj"`.
   - Identify the API entry project, target framework, test projects, data access layer, and existing endpoint style.
   - Check `README`, `Directory.Build.props`, `global.json`, `appsettings*.json`, and local agent instructions when present.

2. **Choose the task path**
   - For endpoint or feature work, use `references/workflows.md#feature-or-endpoint-workflow`.
   - For EF Core schema/data changes, use `references/workflows.md#ef-core-change-workflow`.
   - For bug fixes, use `references/workflows.md#bugfix-workflow`.
   - For review-only tasks, use `references/workflows.md#review-workflow`.

3. **Design before editing**
   - Define the public contract: route, method, request DTO, response DTO, status codes, validation failures, authorization, and error shape.
   - Keep endpoints thin; put business rules in application/domain code.
   - Do not expose EF Core entities directly from API responses.

4. **Implement safely**
   - Pass `CancellationToken` through controller/minimal endpoint, application, and persistence calls.
   - Use async APIs for I/O.
   - Validate route values, query parameters, request bodies, pagination, and server-owned fields.
   - Enforce authorization and tenant/user ownership before returning or mutating data.
   - Avoid raw SQL string concatenation and avoid logging secrets or sensitive personal data.

5. **Test and verify**
   - Add or update unit tests for domain/application behavior.
   - Add or update integration tests for routing, auth, serialization, middleware, persistence, and error contracts when public API behavior changes.
   - Prefer the repository's existing test framework and fixtures.
   - Run the most relevant checks from `references/dotnet-webapi-standards.md#commands`.

6. **Report clearly**
   - State what changed, tests/checks run, checks not run with reasons, and any migration/configuration/deployment notes.
   - Mention uncertainty explicitly when a repository lacks enough context to verify something.

## Defaults when the repository has no clear pattern

Use these defaults only for greenfield work or lightly structured APIs:

```text
src/
  Api/              # asp.net core entry point, controllers/minimal endpoints, middleware
  Application/      # use cases, dtos, validators, interfaces
  Domain/           # entities, value objects, domain rules
  Infrastructure/   # ef core, external services, persistence, auth integrations
tests/
  UnitTests/
  IntegrationTests/
```

Target `.NET 8` or newer unless the user or repository specifies another version. Prefer `ProblemDetails` for errors, explicit DTOs for contracts, `IOptions<T>` with validation for configuration, structured logging message templates, and OpenAPI metadata when Swagger/Swashbuckle/NSwag is already used.

## Reference files

- Load `references/dotnet-webapi-standards.md` for coding standards, API design rules, EF Core guidance, security baseline, command list, and final-response checklist.
- Load `references/workflows.md` for endpoint, feature, bugfix, EF Core, security review, and PR review playbooks.
