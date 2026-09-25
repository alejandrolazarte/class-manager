# GitHub protection

Date: 2026-09-24

Repository: `alejandrolazarte/class-manager` (private, GitHub Free account)

## Goal

- Nothing reaches `main` without a pull request reviewed by the owner.
- AI agents (Claude Code, Codex) can open pull requests but never push to `main` or merge.
- GitHub Actions cannot be abused to run untrusted code or write to the repository.

## Initial state

GitHub Free does not allow rulesets or branch protection on private repositories:

```powershell
gh api repos/alejandrolazarte/class-manager/rulesets
```

```json
{"message":"Upgrade to GitHub Pro or make this repository public to enable this feature.","status":"403"}
```

Actions allowed any marketplace action (`allowed_actions: all`).

## Decision

Server-side enforcement is not available without GitHub Pro or making the repository public. Neither is wanted for now (the code is a commercial product and there is a single contributor). Protection is therefore layered on the client side:

| Layer | What it blocks | Can it be bypassed? |
|---|---|---|
| `.githooks/pre-push` | Any push whose target is `refs/heads/main` | Yes, with `--no-verify` or a clone without the hook |
| `.claude/settings.json` deny rules | Claude Code running `git push ... main`, `--no-verify` and `gh pr merge` | Only by editing the file |
| `CLAUDE.md` / `AGENTS.md` rules | Agents pushing to `main` or merging | Instructions, not enforcement |
| CI on pull requests | Merging code that doesn't build or pass tests (visible red check) | Yes, merge is not blocked on free private repos |

This protects against **mistakes** (an accidental push, an agent going too far), not against a malicious actor with write access. That is acceptable while the owner is the only collaborator.

**Upgrade trigger:** when a second person gets write access, move to GitHub Pro (or a Team organization) and apply the same ruleset used in `knowledge-search`: `deletion`, `non_fast_forward`, `pull_request` with 1 approval, admin bypass only through pull requests.

## Commands used

### Local hook (once per clone)

```powershell
git config core.hooksPath .githooks
```

### Restrict Actions to GitHub-owned actions

```powershell
'{"enabled":true,"allowed_actions":"selected"}' | gh api repos/alejandrolazarte/class-manager/actions/permissions --method PUT --input -
'{"github_owned_allowed":true,"verified_allowed":false,"patterns_allowed":[]}' | gh api repos/alejandrolazarte/class-manager/actions/permissions/selected-actions --method PUT --input -
```

Why: only `actions/*` can run, so a compromised third-party action cannot be introduced through a workflow change.

### Read-only workflow token

```powershell
'{"default_workflow_permissions":"read","can_approve_pull_request_reviews":false}' | gh api repos/alejandrolazarte/class-manager/actions/permissions/workflow --method PUT --input -
```

Why: workflows cannot write to the repository or approve pull requests. `ci.yml` also declares `permissions: contents: read`.

## Final state

- Actions: `allowed_actions: selected`, GitHub-owned only, verified creators not allowed.
- Workflow token: `read`, cannot approve pull requests.
- Collaborators: only the owner (`admin`).
- Secrets: none.
- `main`: protected by the local hook and agent deny rules; changes go through pull requests with CI.

## Audit commands

```powershell
git config core.hooksPath
gh api repos/alejandrolazarte/class-manager/actions/permissions
gh api repos/alejandrolazarte/class-manager/actions/permissions/selected-actions
gh api repos/alejandrolazarte/class-manager/actions/permissions/workflow
gh api repos/alejandrolazarte/class-manager/collaborators --paginate
gh api repos/alejandrolazarte/class-manager/actions/secrets
rg -n "pull_request_target|secrets|workflow_run|uses:|permissions:" .github
```

## Rules for the future

- Never use `pull_request_target` to run code from a pull request.
- Keep workflows on `actions/*`; if a third-party action is needed, pin it by SHA and document why.
- Keep `permissions:` minimal in every workflow and job.
- Do not add secrets to workflows that run pull request code.
