# Workflow: database or EF Core change

## Steps

1. Identify DbContext, entity configuration, and migrations project.
2. Update model/configuration deliberately.
3. Generate migration with the correct startup project.
4. Review generated migration line by line.
5. Add seed/data backfill logic only when necessary and safe.
6. Add or update tests for persistence behavior.
7. Generate an idempotent SQL script when requested for deployment review.
8. Report data-loss, locking, and rollback considerations.

## Migration review checklist

- Unintended column/table drops.
- Rename detected as drop/create.
- Type narrowing or precision loss.
- Non-null column added without default/backfill.
- Cascade delete change.
- Index missing for new query shape.
- Unique constraint may fail on existing data.
- Long-running migration risk on large tables.
