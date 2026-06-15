# Workflow: implement a backend feature

## Steps

1. Clarify the user-visible behavior in a short feature brief.
2. Map affected layers: API, application, domain, infrastructure, tests.
3. Identify existing patterns and extension points.
4. Implement from inside out when possible:
   - domain/application logic
   - persistence/external integrations
   - API boundary
   - documentation
5. Add tests close to the behavior.
6. Run targeted validation early.
7. Run final build/test/format checks.
8. Provide a concise implementation summary.

## Guardrails

- Avoid speculative abstractions.
- Avoid unrelated cleanup.
- Do not add packages without a clear reason.
- Preserve public contracts unless explicitly asked to change them.
- Keep migration and schema work reviewable.
