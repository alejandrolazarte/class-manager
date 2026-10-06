# GitHub protection

Date: 2026-09-25 (settings applied to this repository; the approach comes from salon-manager, 2026-09-24). Updated 2026-10-06: the repository is public and `main` has a server-side ruleset.

Repository: `alejandrolazarte/class-manager` (public since 2026-10-06, GitHub Free account, [FSL-1.1-ALv2](../LICENSE.md); see [Making the repository public](making-the-repository-public.md))

## Goal

- Nothing reaches `main` without a pull request reviewed by the owner.
- AI agents (Claude Code, Codex) can open pull requests but never push to `main` or merge.
- GitHub Actions cannot be abused to run untrusted code or write to the repository.

## Initial state (private repository)

GitHub Free does not allow rulesets or branch protection on private repositories:

```powershell
gh api repos/alejandrolazarte/class-manager/rulesets
```

```json
{"message":"Upgrade to GitHub Pro or make this repository public to enable this feature.","status":"403"}
```

Actions allowed any marketplace action (`allowed_actions: all`).

## Ruleset on `main` (since the repository is public)

Public repositories get rulesets on GitHub Free, so `main` is now protected on the server. Ruleset `main` (id `24615605`), enforcement **active**, target the default branch:

| Rule | Setting | Effect |
|---|---|---|
| Restrict deletions | on | `main` cannot be deleted |
| Block force pushes | on | `main`'s history cannot be rewritten |
| Require a pull request before merging | 0 approvals | every change goes through a pull request; 0 because the only contributor cannot approve their own pull requests |
| Require status checks to pass | `backend`, `frontend`, `e2e`, `container` (GitHub Actions), branches must be up to date | a pull request with red CI, or not tested against the latest `main`, cannot be merged |
| Bypass list | Repository admin, **for pull requests only** | the owner can merge a pull request in an emergency, never push directly |

Set it up in Settings → Rules → Rulesets → New branch ruleset with the values above.

With the ruleset, nobody (the owner, Claude Code or Codex, all with the owner's write access) can push to `main` directly: GitHub rejects it whatever the local hook says. The client-side layers below stay as an earlier, friendlier stop.

## Decision (before the repository was public)

Server-side enforcement was not available without GitHub Pro or making the repository public. Protection was therefore layered on the client side, and these layers are still in place:

| Layer | What it blocks | Can it be bypassed? |
|---|---|---|
| `.githooks/pre-push` | Any push whose target is `refs/heads/main` | Yes, with `--no-verify` or a clone without the hook |
| `.claude/settings.json` deny rules | Claude Code running `git push ... main`, `--no-verify` and `gh pr merge` | Only by editing the file |
| `CLAUDE.md` / `AGENTS.md` rules | Agents pushing to `main` or merging | Instructions, not enforcement |
| CI on pull requests | Merging code that doesn't build or pass tests | No: required status checks in the ruleset |

On their own these layers protect against **mistakes** (an accidental push, an agent going too far), not against a malicious actor with write access; the ruleset closes that gap for `main`.

**When a second person gets write access:** raise the pull request rule to 1 approval so each change is reviewed by someone other than its author.

## Commands used

### Local hook (once per clone)

```powershell
git config core.hooksPath .githooks
```

### Delete head branches after merge

```powershell
'{"delete_branch_on_merge":true}' | gh api repos/alejandrolazarte/class-manager --method PATCH --input -
```

Why: merged branches don't pile up, and a pull request stacked on another one is retargeted to `main` automatically when its base branch is deleted.

### Restrict Actions to GitHub-owned actions

```powershell
'{"enabled":true,"allowed_actions":"selected"}' | gh api repos/alejandrolazarte/class-manager/actions/permissions --method PUT --input -
'{"github_owned_allowed":true,"verified_allowed":false,"patterns_allowed":[]}' | gh api repos/alejandrolazarte/class-manager/actions/permissions/selected-actions --method PUT --input -
```

Why: only `actions/*` can run, so a compromised third-party action cannot be introduced through a workflow change.

### Pull requests from forks

Settings → Actions → General → "Approval for running fork pull request workflows from contributors": **Require approval for all external contributors**. Why: with a public repository anyone can open a pull request from a fork, and its workflows must not run until the owner has read the change. Fork workflows run with a read-only token and without secrets, and `deploy.yml` only runs for pushes to `main`.

### Read-only workflow token

```powershell
'{"default_workflow_permissions":"read","can_approve_pull_request_reviews":false}' | gh api repos/alejandrolazarte/class-manager/actions/permissions/workflow --method PUT --input -
```

Why: workflows cannot write to the repository or approve pull requests. `ci.yml` also declares `permissions: contents: read`.

## Final state

- Actions: `allowed_actions: selected`, GitHub-owned only, verified creators not allowed.
- Workflow token: `read`, cannot approve pull requests.
- Merged branches: deleted automatically (`delete_branch_on_merge: true`).
- Collaborators: only the owner (`admin`).
- Secrets: none. Variables: `AZURE_*` for the deploy workflow (identifiers, not credentials).
- Visibility: public, forking allowed.
- `main`: ruleset `main` active (no deletion, no force push, pull request required, `backend`, `frontend`, `e2e` and `container` must pass on an up-to-date branch, admin bypass only through pull requests), plus the local hook and agent deny rules.
- Fork pull requests: workflows wait for the owner's approval.

## Audit commands

```powershell
git config core.hooksPath
gh api repos/alejandrolazarte/class-manager --jq '.delete_branch_on_merge'
gh api repos/alejandrolazarte/class-manager/actions/permissions
gh api repos/alejandrolazarte/class-manager/actions/permissions/selected-actions
gh api repos/alejandrolazarte/class-manager/actions/permissions/workflow
gh api repos/alejandrolazarte/class-manager/collaborators --paginate
gh api repos/alejandrolazarte/class-manager/actions/secrets
gh api repos/alejandrolazarte/class-manager/rules/branches/main
gh api repos/alejandrolazarte/class-manager/rulesets --jq '.[] | {id, name, enforcement}'
gh api repos/alejandrolazarte/class-manager/rulesets/24615605 --jq '{enforcement, conditions, bypass_actors}'
gh api repos/alejandrolazarte/class-manager/actions/permissions/fork-pr-contributor-approval
rg -n "pull_request_target|secrets|workflow_run|uses:|permissions:" .github
```

## Deploy workflow

`deploy.yml` is the only workflow with more than read access, and only in the job that needs it:

| Job | Extra permission | Why |
|---|---|---|
| `image` | `packages: write` | Push the API image to `ghcr.io` with the built-in `GITHUB_TOKEN` |
| `api` | `id-token: write` | Sign in to Azure with OpenID Connect; no Azure secret is stored in GitHub |

It is triggered by `workflow_run` of CI and only proceeds for a successful **push** to `main` (never for pull requests), checking out the exact commit CI tested. Azure identifiers are repository variables (`AZURE_*`), not secrets. The Entra federated credential only trusts `repo:alejandrolazarte@55626992/class-manager@1388192765:ref:refs/heads/main` (GitHub's subject includes the owner and repository ids). Setup: [pilot deployment runbook](pilot-deployment.md).

## Rules for the future

- Never use `pull_request_target` to run code from a pull request.
- Keep workflows on `actions/*`; if a third-party action is needed, pin it by SHA and document why.
- Keep `permissions:` minimal in every workflow and job.
- Do not add secrets to workflows that run pull request code.
