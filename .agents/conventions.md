# .NET WebAPI conventions

## General C# style

- Prefer clear, intention-revealing names over abbreviations.
- Keep methods small enough to test and review.
- Use nullable reference types correctly. Do not silence nullable warnings without a reason.
- Prefer `record` or immutable DTOs when appropriate, but follow existing style.
- Use `var` when the type is obvious from the right-hand side; otherwise use explicit types.
- Avoid static global state unless it is immutable and safe.
- Do not introduce new dependencies unless they remove more complexity than they add.

## API contracts

- Request DTO names: `CreateThingRequest`, `UpdateThingRequest`, `SearchThingsRequest`.
- Response DTO names: `ThingResponse`, `ThingSummaryResponse`, `PagedResponse<T>`.
- Do not return EF entities directly.
- Preserve existing casing and JSON naming policy.
- Use stable route names and avoid ambiguous route templates.
- Use appropriate HTTP status codes:
  - `200 OK` for successful reads and updates with response bodies.
  - `201 Created` for successful creates with a location when possible.
  - `204 NoContent` for successful deletes or updates without bodies.
  - `400 BadRequest` for validation and malformed input.
  - `401 Unauthorized` when authentication is required.
  - `403 Forbidden` when authenticated user lacks permission.
  - `404 NotFound` when a resource does not exist or should not be disclosed.
  - `409 Conflict` for optimistic concurrency or uniqueness conflicts.

## Validation

- Validate boundary DTOs, not persistence entities.
- Prefer existing validation mechanism.
- Use explicit max lengths, ranges, and formats for public inputs.
- Normalize user input only when the behavior is documented and tested.
- Validate pagination inputs and cap page size.

## Error handling

- Prefer a single error-handling middleware/filter/pipeline if the repo has one.
- Return `ProblemDetails` or the repo's existing equivalent.
- Do not leak exception details in production responses.
- Include correlation/request IDs when the project already supports them.

## Logging

- Use structured logging with named placeholders.
- Log events that help diagnose behavior, not every line of execution.
- Never log credentials, tokens, API keys, connection strings, passwords, or sensitive PII.

## Dependency injection

- Register interfaces and implementations in the existing composition root.
- Use `Scoped` for per-request application/data services, `Singleton` only for stateless thread-safe services, and `Transient` for lightweight stateless components.
- Avoid resolving services from `IServiceProvider` outside composition or framework-required factories.

## Async and cancellation

- Use async all the way for I/O.
- Accept and pass `CancellationToken` in endpoints, handlers, repositories, and service methods.
- Avoid `.Result`, `.Wait()`, and blocking over async.
