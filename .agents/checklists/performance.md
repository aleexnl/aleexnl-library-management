# Performance and reliability checklist

- No obvious N+1 queries.
- Read-only EF queries use `AsNoTracking()` where appropriate.
- Queries project only needed fields when possible.
- Pagination is required for unbounded collections.
- New query patterns have suitable indexes.
- External calls have timeouts and cancellation.
- Retrying is bounded and does not amplify failures.
- Large payloads are streamed or limited.
- Background work is durable if it must survive process restarts.
- Logging is useful but not excessive.
