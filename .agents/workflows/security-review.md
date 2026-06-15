# Workflow: security review

## Scope

Use this workflow for PRs or tasks touching auth, identity, user input, external calls, file handling, package changes, logging, configuration, or persistence.

## Steps

1. Identify trust boundaries.
2. List user-controlled inputs.
3. Trace sensitive data and secrets.
4. Check route-level and object-level authorization.
5. Check input validation and output encoding assumptions.
6. Check data access for injection or unsafe dynamic query construction.
7. Check logs and telemetry for sensitive data exposure.
8. Check dependency changes for necessity and vulnerabilities.
9. Recommend minimal fixes and tests.

## Severity rubric

- Critical: likely unauthorized data access, credential exposure, or remote code execution.
- High: plausible auth bypass, injection, sensitive data leakage, or destructive action.
- Medium: missing defense-in-depth, risky config, or incomplete validation with realistic abuse.
- Low: hardening, clarity, or maintainability issue with limited exploitability.
