# AGENTS.md

Instructions for AI coding agents (Codex and others) working in this repository.

`CLAUDE.md` is the source of truth for the stack, structure and code conventions. Read it before making changes. The rules below are repeated here because they must never be broken.

## RULE: main only changes through reviewed pull requests

- Never push to `main`, never merge pull requests, never use `--no-verify`.
- Work on a branch (`feature/...`, `fix/...`, `chore/...`, `docs/...`), push it and open a PR.
- The owner reviews and merges on GitHub. Stop after opening the PR.

## Running locally

The API and the Expo app run on the host. SQL Server runs in a container (dev database and Testcontainers). See `CLAUDE.md` for commands.

```powershell
dotnet build class-manager.slnx
dotnet test class-manager.slnx
```

## RULE: English everywhere

Code, identifiers, docs and commit messages are in English. Only user-facing UI copy is localized.
