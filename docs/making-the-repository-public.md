# Making the repository public

Date: 2026-10-06. Status: done. The license is in place and the repository is public since 2026-10-06 (no forks at the time of the switch).

## Why it came up

GitHub Actions minutes are limited on private repositories of a GitHub Free account, and CI stopped starting jobs (`runner_id: 0`, no logs) once they ran out. Public repositories get unlimited minutes on standard runners, and rulesets and branch protection without upgrading (see [GitHub protection](github-protection.md)).

## License: FSL-1.1-ALv2

[LICENSE.md](../LICENSE.md) is the [Functional Source License 1.1](https://fsl.software/) with Apache 2.0 as the future license, created by Sentry:

- Anyone may read, run, change and redistribute the code for any purpose except a **competing use**: offering it, or a derivative, as a commercial product or service that substitutes class-manager.
- Each version becomes available under **Apache 2.0 two years** after it is published.
- It is "fair source", not open source: the code is visible, the competing use is not allowed.

Chosen over the Business Source License because FSL is a fixed, standard text (no use limitation or change date to write), and over AGPL because AGPL allows a competitor to host the app as long as they publish their changes.

What the license does not do: it does not tell who downloaded the code (Insights → Traffic only counts views and clones for 14 days), and nobody enforces it automatically. A copy found later is handled with a DMCA takedown to GitHub or legal action.

## History review (2026-10-06)

Every commit of every branch was reviewed (228 commits without merges, full clone):

- **gitleaks** with the repository rules and with the default rules: no leaks.
- **Manual search** for passwords, connection strings, signing and storage keys: only test values (the throwaway SQL Server password of the e2e job, the RFC 8291 example keys, placeholders such as `<your-password>`). The deploy workflow reads Azure names from repository variables and signs in with OIDC; no secret is stored in the repository.
- **Personal data**: emails are `example.com`, `.local` addresses and the app's sender address; phone numbers are made up test numbers.
- **Pilot client**: the pilot plan and several backend plans describe the real pilot business (DF Swimming Team): how it works, its courses, cancellation policies, the tools it uses and that it is on Enterprise at no cost (`docs/pilot-df-swimming.md`, `docs/backend/20260928-private-lessons/plan.md`, `docs/backend/20260928-roles-and-permissions/plan.md`, `docs/backend/20261003-subscriptions/plan.md`, `docs/backend/20261002-branded-emails/plan.md`, `docs/mvp-plan.md`, and the test business name "DF Swimming Tenerife").

## Checklist before switching to public

1. Decide about the pilot client's information: ask the client whether they mind it being public, or remove it. Removing it from the current files is not enough, because the history keeps it; that needs rewriting the history (`git filter-repo`) and force-pushing every branch, which breaks open pull requests and every existing clone.
2. Settings → Actions → General: keep "Require approval for all external contributors" (or at least first-time contributors) so pull requests from forks do not run CI without review.
3. Azure: already true (see [GitHub protection](github-protection.md#deploy-workflow)). Check that the federated credential used by the deploy workflow only accepts the `main` branch (subject `repo:alejandrolazarte/class-manager:ref:refs/heads/main`), so no other workflow can deploy.
4. Re-run gitleaks on the full history right before switching (`gitleaks git . --log-opts="--all"`) in case something was added since this review.
5. Switch: Settings → General → Danger Zone → Change visibility. It cannot be undone in practice: forks and caches made while public stay.
6. After switching, apply the rulesets from [GitHub protection](github-protection.md) that GitHub Free only allows on public repositories. Done: ruleset `main`.

Still open after the switch: item 1 (the pilot client's information) and item 2 (the fork pull request approval setting, to confirm in Settings).
