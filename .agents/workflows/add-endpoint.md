# Workflow: add or change an API endpoint

## Inputs

- Business goal.
- HTTP method and route, if known.
- Auth requirements.
- Request and response shape.
- Persistence requirements.
- Compatibility constraints.

## Steps

1. Use the API architect prompt to design the endpoint contract.
2. Inspect neighboring endpoints and tests for conventions.
3. Create or update request/response DTOs.
4. Add validation at the boundary.
5. Implement service/handler logic.
6. Add persistence changes if needed.
7. Add OpenAPI metadata and examples where the repo supports them.
8. Add tests:
   - success
   - validation failure
   - not found
   - auth failure if protected
   - conflict/concurrency if relevant
9. Run targeted tests, then build/test.
10. Summarize changed files and validation results.

## Acceptance criteria

- Endpoint follows existing route and versioning conventions.
- No EF entities are exposed over HTTP.
- Error responses are consistent.
- Tests prove the HTTP contract.
- OpenAPI docs reflect behavior.
