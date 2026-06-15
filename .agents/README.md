# .agents prompt pack for .NET WebAPI

This directory contains reusable role prompts, workflows, checklists, and templates for AI coding agents working on an ASP.NET Core WebAPI repository.

The canonical repository instruction file is the root `AGENTS.md`. This `.agents/` directory is a prompt pack that `AGENTS.md` points to. It is intentionally tool-agnostic: Cursor, Copilot Chat, Codex, Claude Code, and other agents can use the markdown prompts manually. Codex-native subagent definitions are in `.codex/agents/`.

## Recommended usage

Ask your coding agent to use a specific role or workflow, for example:

```text
Use .agents/workflows/add-endpoint.md and .agents/prompts/test-engineer.md to add a POST /orders endpoint with validation and integration tests.
```

```text
Review this branch using .agents/workflows/pr-review.md. Spawn api_architect, security_reviewer, and test_engineer if your agent supports subagents.
```

```text
Use .agents/prompts/db-migration-specialist.md to inspect the EF migration for data-loss risks before I merge.
```

## ChatGPT Skill included

This setup now includes a real ChatGPT Skill in `skills/dotnet-webapi-engineer/` plus an upload-ready package at `skills/skill.zip`. Install that skill in ChatGPT when you want the same ASP.NET Core WebAPI workflow available outside this repository prompt pack.

The Skill covers endpoint implementation, DTO and validation design, EF Core change review, unit and integration test expectations, security review, OpenAPI documentation, and final-response reporting.

## Directory map

- `project-context.md` - fill-in project facts agents should know.
- `conventions.md` - coding standards and repository conventions.
- `commands.md` - build, test, run, format, security, and EF Core commands.
- `architecture.md` - default .NET WebAPI architecture guidance.
- `prompts/` - reusable role prompts.
- `workflows/` - task playbooks.
- `checklists/` - review and readiness checklists.
- `templates/` - structured outputs for features, endpoint contracts, tests, and PR summaries.

## Maintenance

Update `project-context.md`, `commands.md`, and `conventions.md` after the first agent pass through the repository. Keep rules specific, short, and enforceable.
