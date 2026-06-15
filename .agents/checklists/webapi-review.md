# WebAPI review checklist

## Contract

- Route follows existing conventions.
- HTTP method matches semantics.
- Status codes are accurate.
- DTOs are explicit and stable.
- Error shape matches repository convention.
- OpenAPI metadata is correct.

## Behavior

- Controller/endpoint is thin.
- Business rules live in services/handlers/domain code.
- Cancellation token is passed through I/O paths.
- Async code is not blocked.
- Pagination/filtering has limits and validation.

## Tests

- Success path.
- Validation failure.
- Auth failure.
- Not found.
- Conflict/concurrency when relevant.
- Persistence side effects when relevant.
