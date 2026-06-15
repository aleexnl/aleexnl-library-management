# .NET WebAPI Workflows

## Feature or endpoint workflow

Use this workflow for tasks like adding `POST /orders`, changing a response shape, adding a query parameter, or introducing a new use case.

1. Inspect existing endpoint style, route prefixing, versioning, validation, error handling, auth, OpenAPI metadata, and tests.
2. Define the contract:
   - HTTP method and route.
   - Request DTO and validation rules.
   - Response DTO and status codes.
   - Auth policy and ownership checks.
   - Error responses and `ProblemDetails` fields.
3. Implement the application behavior before or alongside the endpoint.
4. Add persistence changes only if necessary and follow existing EF Core patterns.
5. Add tests:
   - Happy path.
   - Validation failure.
   - Not found, conflict, forbidden/unauthorized, or ownership failure when applicable.
   - Serialization/contract assertions for public APIs.
6. Run targeted tests first, then broader checks when feasible.
7. Update docs or OpenAPI examples when the public contract changes.

## Bugfix workflow

1. Reproduce or locate the failing behavior using existing tests, logs, or code paths.
2. Identify the smallest fix that addresses the root cause.
3. Add a regression test that fails before the fix when feasible.
4. Avoid unrelated refactors.
5. Run the regression test and relevant project tests.
6. Report the root cause, fix, and verification.

## EF Core change workflow

1. Inspect the DbContext, entity configuration, migrations folder, provider, and existing migration conventions.
2. Make model/configuration changes intentionally.
3. Generate the migration with the repository's normal command. If unknown, infer the startup project and DbContext project after inspecting `.csproj` files.
4. Review generated migration operations for data loss, table rewrites, nullability changes, index locks, and seed data changes.
5. Add a rollback or deployment note for risky operations.
6. Add or update integration tests for persistence behavior.
7. If requested or production-bound, generate an idempotent SQL script and review it.

## Security review workflow

Review these areas and classify findings by severity:

- Authentication and authorization gaps.
- Missing tenant/user ownership checks.
- Over-posting or mass assignment.
- Input validation and output encoding.
- Secret handling and sensitive logging.
- Unsafe CORS, cookies, tokens, or headers.
- SQL injection or unsafe raw SQL.
- SSRF, file upload, path traversal, and deserialization risks.
- Rate limiting and request size exposure on public endpoints.
- Error responses leaking internals.

For each finding, provide the risky code path, exploit scenario, recommended fix, and tests to add.

## Review workflow

Use this for pull request, branch, or patch reviews.

1. Identify changed files and understand intent from the request, PR description, commits, or diff.
2. Review correctness, API compatibility, security, data access, test coverage, performance, observability, and docs.
3. Prioritize issues that could break production behavior or create security/data risks.
4. Avoid blocking comments for personal style preferences that conflict with existing repository patterns.
5. Provide concise findings first, then optional suggestions.
6. If no substantive issues are found, say so and list the checks performed.
