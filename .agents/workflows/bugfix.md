# Workflow: bug fix

## Steps

1. Reproduce or reason to the failing behavior from code and tests.
2. Add a failing test when practical.
3. Identify the narrowest root cause.
4. Implement the smallest safe fix.
5. Confirm the new test fails before the fix when feasible, then passes after.
6. Run relevant regression tests.
7. Check nearby edge cases.

## Output

- Root cause.
- Fix summary.
- Regression test added or why not.
- Commands run.
- Risks of the fix.
