# .NET WebAPI Standards

## Commands

Run commands from the repository root unless local documentation says otherwise.

```bash
dotnet restore
dotnet build --no-restore
dotnet test --no-build
dotnet format --verify-no-changes
```

If there are multiple solutions or project names are unknown, inspect first:

```bash
find . -maxdepth 4 -name "*.sln" -o -name "*.csproj"
```

Run the API by targeting the actual API project:

```bash
dotnet run --project src/Api/Api.csproj
```

Use project-specific paths after inspection. Do not pretend a command succeeded; report exact failures and the command used.

## C# and ASP.NET Core style

- Respect nullable reference types.
- Use `async`/`await` for I/O.
- Pass `CancellationToken` from the endpoint/controller into application and persistence calls.
- Prefer constructor injection and interfaces at architectural boundaries.
- Keep controllers and minimal endpoints thin.
- Put business rules in application/domain code.
- Use DTOs for request and response contracts.
- Do not expose EF Core entities directly from API responses.
- Prefer `IOptions<T>` or validated options objects for configuration.
- Use structured logging message templates, not string interpolation, for runtime logs.
- Avoid static mutable state and service locator patterns.
- Do not manually resolve services unless framework integration requires it.

## API design rules

- Preserve the repository's existing controller vs minimal API style.
- Use clear resource-oriented routes such as `/api/orders/{orderId}`.
- Return appropriate HTTP status codes: `200`, `201`, `204`, `400`, `401`, `403`, `404`, `409`, `422`, and `500` where appropriate.
- Use `ProblemDetails` for errors and validation failures when the project has no competing standard.
- Include OpenAPI metadata for new endpoints when the project uses Swagger, Swashbuckle, or NSwag.
- Keep request models narrow.
- Do not allow clients to set server-owned fields such as ids, audit fields, roles, tenant ids, or security-sensitive flags.
- Validate route ids, query parameters, request bodies, and pagination values.
- Use explicit pagination for list endpoints that can grow.
- Preserve API compatibility unless the user explicitly asks for a breaking change.

## Data access and EF Core

- Follow the existing persistence pattern before introducing repositories, unit-of-work wrappers, query objects, or CQRS layers.
- Prefer EF Core async APIs for database I/O.
- Use `AsNoTracking()` for read-only queries unless tracking is needed.
- Avoid N+1 queries; load only the data needed for the response.
- Do not concatenate SQL strings. Use LINQ or parameterized SQL.
- Use transactions for multi-step writes that must succeed or fail together.
- Model concurrency where users can overwrite each other's changes.
- Review generated migrations before accepting them.
- Treat destructive migration operations as high-risk and call them out explicitly.
- For production changes, prefer generating an idempotent migration script when the repository supports it.

## Security baseline

- Enforce authentication and authorization at endpoint/controller boundaries for protected resources.
- Validate tenant/user ownership before returning or mutating data.
- Treat all client input as untrusted.
- Keep CORS restrictive; do not use wildcard origins with credentials.
- Never log secrets, passwords, tokens, authorization headers, cookies, private keys, or sensitive personal data.
- Use configuration providers, user secrets, environment variables, or a secret manager for secrets.
- Avoid binding privileged fields directly from client requests.
- Consider rate limiting, request size limits, and auth hardening for public endpoints.

## Testing expectations

Add or update tests for behavioral changes.

Recommended split:

```text
unit tests: validators, domain logic, application services
integration tests: routing, auth, middleware, persistence, serialization, error contracts
contract tests: public response shapes and status codes when api compatibility matters
```

Use the repository's existing test framework: xUnit, NUnit, MSTest, or another established choice. For ASP.NET Core integration tests, use `WebApplicationFactory<TEntryPoint>` when it is already available or appropriate.

## Documentation expectations

- Update README, OpenAPI comments, sample requests, environment setup docs, or changelog entries when behavior or setup changes.
- Add request and response examples for new public endpoints when practical.
- Keep docs close to the code they describe.

## Final response checklist

Report these items at the end of coding work:

1. What changed.
2. Tests or checks run.
3. Checks not run and why.
4. Migration, configuration, or deployment notes.
5. Remaining risks or assumptions, only when relevant.
