# .NET WebAPI architecture guide

Use this as a default only when the repository does not already have stronger conventions.

## Recommended layering

```text
src/
  Api/                 HTTP boundary, auth, filters, middleware, OpenAPI
  Application/         use cases, services, validators, interfaces
  Domain/              domain entities, value objects, domain rules
  Infrastructure/      EF Core, external clients, file/storage providers

tests/
  UnitTests/
  IntegrationTests/
```

Small projects may keep layers in one project using folders. Do not create layers just for ceremony.

## Endpoint flow

1. Bind request DTO.
2. Validate boundary input.
3. Authorize user/action.
4. Call application service/handler with cancellation token.
5. Map domain/application result to HTTP response.
6. Return typed response and document OpenAPI behavior.

## Controllers

Controllers should contain HTTP-specific concerns only:

- route attributes
- auth attributes
- model binding
- response mapping
- status code selection

Avoid business rules, database queries, or external API calls inside controllers.

## Minimal APIs

For Minimal APIs:

- Group related routes with `MapGroup`.
- Keep route handlers small.
- Move logic into services or endpoint classes when handlers grow.
- Use typed results when possible.
- Add endpoint filters for cross-cutting validation when it matches existing style.

## Application services and handlers

- Express use cases in application services or handlers.
- Return explicit success/failure results rather than throwing for expected domain failures.
- Throw only for unexpected/system failures.
- Keep authorization either at the HTTP boundary or in explicit policy services, not scattered.

## Data access

- Use EF Core DbContext from the data layer.
- Use `AsNoTracking()` for read-only queries when entity tracking is not needed.
- Keep query projection close to the query to avoid over-fetching.
- Avoid N+1 queries. Use includes, projections, or split queries deliberately.
- Add indexes for new query patterns when needed.

## OpenAPI

- Document response types and error status codes.
- Add useful examples for non-obvious request/response shapes.
- Do not expose internal-only fields.
