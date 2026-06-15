# Workflow: PR review

## Review dimensions

1. Correctness and behavior regressions.
2. API contract compatibility.
3. Security and authorization.
4. Data and migration safety.
5. Test coverage and test quality.
6. Performance and reliability.
7. Maintainability and consistency with repo conventions.

## Process

1. Compare branch against base.
2. Read changed files and nearby context.
3. Run tests or identify the relevant validation commands.
4. Produce only actionable findings.
5. Avoid style-only comments unless they hide a real defect.

## Output format

```text
Finding <n>: <short title>
Severity: Critical | High | Medium | Low
File/symbol:
Why it matters:
Suggested fix:
Suggested test:
```

If approving, summarize what was reviewed and residual risks.
