# Dependency and secret scanning

Date: 2026-10-03

How the repository finds vulnerable packages and leaked secrets. The repository is private on GitHub Free, which decides what is free and what is not.

| Check | What it finds | Cost on a private repository | Where |
|---|---|---|---|
| Dependabot alerts | NuGet and npm packages with a known vulnerability | Free | Repository settings |
| Dependabot security updates | Opens a PR that upgrades the vulnerable package | Free | Repository settings |
| Dependabot version updates | Weekly PRs that keep packages current | Free | `.github/dependabot.yml` |
| Push protection for yourself | Blocks a push from the owner's account that contains a known secret | Free | Personal settings |
| gitleaks | Secrets anywhere in the history, on demand | Free | `.gitleaks.toml`, run locally |
| Repository secret scanning (Secret Protection) | Secrets in the history and in every push, from any account | Paid per committer | Not enabled |
| CodeQL (Code Security) | Vulnerable code patterns | Paid per committer | Not enabled |

## Dependabot

### Settings

In GitHub: **repository → Settings → Code security**, enable:

- **Dependency graph** (needed by the other two).
- **Dependabot alerts**: an email and an entry in the Security tab when a dependency has a known vulnerability.
- **Dependabot security updates**: a PR that upgrades the vulnerable package. It goes through the normal review like any other PR, so `main` still only changes through reviewed pull requests.

### Version updates

`.github/dependabot.yml` opens version update PRs every Monday:

- **NuGet** from the root (`Directory.Packages.props`), **pnpm** for `app/` and `e2e/`, and **GitHub Actions** monthly.
- Minor and patch updates are grouped into one PR per ecosystem, so a week produces a handful of PRs, not one per package.
- **Expo SDK packages are excluded from minor and major updates** (`expo`, `expo-*`, `@expo/*`, `react`, `react-dom`, `react-native`, `react-native-*`). The Expo SDK pins their versions together; moving one alone breaks the app. Upgrade the SDK on purpose with `pnpm expo install --fix` after reading the SDK release notes. Patch updates still arrive.
- GitHub Actions updates only bump `actions/*` versions, which the allowed-actions rule already permits ([GitHub protection](github-protection.md)).

A Dependabot PR is reviewed like any other: CI must be green; read the changelog for anything beyond a patch.

### Local check

Without waiting for Dependabot:

```powershell
dotnet list class-manager.slnx package --vulnerable --include-transitive
cd app; pnpm audit
cd e2e; pnpm audit
```

## Secrets

### Push protection for yourself

In GitHub: **profile picture → Settings → Code security → Push protection for yourself** → Enable. GitHub then rejects a push from the owner's account that contains a recognised secret (Azure, Stripe, GitHub tokens and others), in any repository. It does not cover pushes made with other credentials, such as Claude Code on the web sessions, and it does not look at the existing history; gitleaks covers both.

### gitleaks

[gitleaks](https://github.com/gitleaks/gitleaks) scans the whole history for secrets. `.gitleaks.toml` extends its default rules and allowlists the public example keys of RFC 8291 appendix A, which the web push encryption test uses as a test vector.

```powershell
gitleaks git . --redact --log-opts="--all"   # every commit of every branch
gitleaks dir . --redact                      # the working tree, including untracked files
```

Run it before the pilot and after anyone with access leaves. Last run: 2026-10-03, 162 commits on all branches, no leaks after the allowlist.

### When a secret is found

Removing it from the history is not enough: anyone who cloned the repository still has it. Rotate it first, then remove it:

1. Rotate the secret where it lives (Container Apps secret, SMTP app password, VAPID key pair; see [pilot deployment](pilot-deployment.md) for the commands).
2. Remove it from the code and load it from configuration instead.
3. Add a finding that is not a secret (a test vector, a public example) to `.gitleaks.toml` with a description, never by disabling a rule.

## Not enabled

Repository secret scanning and CodeQL need GitHub Secret Protection and Code Security, paid per active committer on private repositories. With a single contributor, push protection for yourself and gitleaks cover the same risk for free. Revisit when the team grows or before the public launch, together with a dynamic scan (OWASP ZAP) and an external penetration test.
