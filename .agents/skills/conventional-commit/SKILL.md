---
name: conventional-commit
description: Generate, validate, and create git commits that follow Conventional Commits v1.0.0. Use when Codex needs to turn a diff or staged changes into a commit message, perform `git commit`, suggest a conventional commit message, split work into meaningful commits, or check whether an existing commit message follows the Conventional Commits format.
---

# Conventional Commit

## Overview

Generate commit messages and create commits that conform to Conventional Commits v1.0.0.
Inspect the actual code changes first, then choose the narrowest accurate type, optional scope, and concise subject.

## Workflow

1. Inspect the repository state before writing a message.
2. Determine whether the current changes belong in one commit or multiple commits.
3. Choose the conventional commit header from the change semantics, not from filenames alone.
4. Add a body or footer only when it carries useful information.
5. Create the commit only after the staged content matches the intended message.

## Inspect Changes First

Start by checking:

- `git status --short` to see staged and unstaged files
- `git diff --staged` to describe the commit if changes are already staged
- `git diff` if the user asks for a message before staging or asks what should be committed

Do not invent a commit message from a vague request without examining the diff.

If unrelated changes are mixed together, do not hide that behind a broad message. Either:

- stage only the relevant subset and commit that subset, or
- ask the user whether the work should be split into multiple commits

## Header Format

Use this header format:

```text
<type>[optional scope][!]: <description>
```

Apply these rules:

- Keep `type` lowercase.
- Keep `scope` short, lowercase, and specific; prefer a module, feature, bounded area, or package name.
- Omit `scope` when no clear scope improves the message.
- Write `description` in the imperative mood, lowercase first word, with no trailing period.
- Keep the subject concise. Prefer a single line that explains the user-visible or developer-visible change.
- Use `!` only when the commit introduces a breaking change.

## Choose the Type

Use the most specific valid type:

- `feat`: add or expand user-facing functionality
- `fix`: correct a bug or regression
- `docs`: change documentation only
- `style`: make non-functional formatting or stylistic changes
- `refactor`: restructure code without changing external behavior
- `perf`: improve performance behavior
- `test`: add or change tests without production behavior changes
- `build`: change build system or external dependencies
- `ci`: change CI or automation configuration
- `chore`: repository maintenance that does not fit another type

Prefer these distinctions:

- Use `fix`, not `refactor`, when the observable behavior is corrected.
- Use `refactor`, not `feat`, when behavior stays materially the same.
- Use `build` for dependency or packaging changes that affect build/runtime wiring.
- Use `chore` only after ruling out a more specific type.

## Choose the Scope

Choose a scope only when it clarifies the change.

Good scope sources:

- project or package name
- top-level feature area
- bounded subsystem such as `auth`, `catalog`, `api`, `ui`, `db`

Avoid scopes that add noise:

- single generic filenames
- vague labels like `misc`, `stuff`, `cleanup`
- multiple scopes joined together unless the repository already uses that style

## Breaking Changes

Mark breaking changes in one or both supported ways:

- put `!` before the colon in the header
- add a footer starting with `BREAKING CHANGE:`

Use a breaking change marker when consumers must change code, configuration, contracts, CLI usage, public API calls, or persisted data expectations.

If the change is breaking, include a footer that states what broke and what the consumer must do next.

Example:

```text
feat(api)!: rename loan status field

BREAKING CHANGE: `loanState` replaces `status` in API responses.
```

## Body and Footer

Skip the body when the header is enough.

Add a body when it helps explain:

- why the change was needed
- a non-obvious implementation choice
- migration guidance

Add footers for:

- `BREAKING CHANGE: ...`
- issue references when the repository uses them, such as `Refs: #123` or `Closes: #123`

Do not pad the body with obvious restatements of the diff.

## Commit Creation Rules

When the user wants an actual commit:

1. Confirm the staged diff matches the intended commit.
2. Stage files if the user asked you to prepare the commit and the intended scope is clear.
3. Run `git commit` with the conventional message.
4. Re-check `git status --short` after the commit to confirm the repository state.

If the repository already has commit conventions beyond Conventional Commits, follow them as long as they do not conflict with the spec.

## Quick Decision Heuristics

- New capability: `feat`
- Bug fix: `fix`
- Rename or restructure with same behavior: `refactor`
- Dependency bump for tooling: `build` or `chore`, prefer `build` when it affects packaging or build behavior
- Test-only update: `test`
- CI pipeline edit: `ci`
- Readme or comment-only update: `docs`

## Examples

```text
feat(search): add author filtering to catalog results
```

```text
fix(ui): prevent duplicate loan submissions
```

```text
refactor(db): extract loan query builder
```

```text
build: upgrade entity framework packages
```

```text
docs(api): document overdue fee endpoint
```

```text
feat(auth)!: require refresh token rotation

BREAKING CHANGE: existing long-lived refresh tokens become invalid after deployment.
```

## Validation

Before finalizing, verify that:

- the type matches the actual semantic change
- the scope is useful or omitted
- the subject is imperative and has no trailing period
- the header matches `<type>[scope]: <description>` or `<type>[scope]!: <description>`
- breaking changes are marked explicitly
- the commit does not hide multiple unrelated changes
