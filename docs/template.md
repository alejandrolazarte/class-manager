# Using the template

This repository is a GitHub template repository and a `dotnet new` template at the same time. It was generated from `salon-manager` on 2026-09-24 by removing the hair-salon domain and keeping the platform.

## What a new app gets

| Area | Contents |
|---|---|
| Authentication | `src/Security`: Identity accounts, JWT access tokens, rotated refresh tokens, lockout, rate limit ([security.md](security.md)) |
| Multi-tenancy | `src/Tenancy` and `src/Tenancy.AspNetCore`: query filters, tenant stamping, tenant from the `tenant_id` claim ([tenancy.md](tenancy.md)) |
| Onboarding | Owner sign up creates the user, the `Business` (tenant root) and its `BusinessMember` in one transaction; sign in, refresh, sign out |
| Settings | Business name, time zone, currency and calling code (`GET` and `PUT /api/business`) |
| Sample entity | `Client`: tenant-owned entity with its configuration, repository, endpoints, isolation tests and Expo screens. Copy it for the first real entity |
| Rules | Analyzers TEST001–005 and ARCH001–004 ([analyzers.md](analyzers.md)), `-warnaserror`, `dotnet format`, ESLint and Prettier |
| Tests | Unit tests, integration tests with Testcontainers, Playwright e2e for sign up and sign in |
| CI | `.github/workflows/ci.yml` with GitHub-owned actions only |

## Create a new app

1. Create the empty repository on GitHub (for example `swim-school`).

2. Install the template from a clone of this repository (only once per machine, and again after pulling template changes):

   ```powershell
   git clone https://github.com/alejandrolazarte/class-manager.git
   dotnet new install ./class-manager
   ```

3. Generate the app. `-n` is the PascalCase name used for namespaces and projects; the kebab-case form (`swim-school`) is derived from it for the solution, package names, the Expo slug and the database names. `--allow-scripts yes` lets the template run `dotnet format`, which reorders `using` directives for the new name.

   ```powershell
   dotnet new class-manager -n SwimSchool --displayName "Swim School" -o swim-school --allow-scripts yes
   ```

4. Check the result before the first commit:

   ```powershell
   cd swim-school
   dotnet build swim-school.slnx -warnaserror
   dotnet test swim-school.slnx
   cd app; pnpm install; pnpm typecheck; pnpm test; cd ..
   ```

5. Push it and apply what GitHub doesn't copy:

   ```powershell
   git init -b main
   git config core.hooksPath .githooks
   git add -A
   git commit -m "chore: initial app from class-manager"
   git remote add origin https://github.com/alejandrolazarte/swim-school.git
   git push -u origin main
   ```

   - Branch protection and the GitHub-owned-actions-only setting: follow [github-protection.md](github-protection.md) for the new repository.
   - Secrets and the JWT signing key: [backend/local-development.md](backend/local-development.md#jwt-signing-key).
   - The Claude Code cloud environment setup script: [cloud-environment.md](cloud-environment.md).

6. Rewrite the first paragraph of `README.md` and `CLAUDE.md` to describe the new app.

## Adapting the domain

- **Tenant root.** `Business` is the tenant. Rename it when the app has a better word (for example `School`), in one PR: entity, repository, endpoints (`/api/business`), the Expo `features/business` folder and the Spanish copy ("negocio").
- **First entity.** Copy the `Client` slice: entity implementing `ITenantOwned`, configuration with an index that starts with `TenantId`, repository interface in `Core` and implementation in `Infrastructure`, use cases, endpoints, a migration, the `When_<entity>_belongs_to_another_business/Then_it_is_not_returned` test, and the Expo feature folder.
- **Remove `Client`** once the app has its own entities, if it doesn't need it.

## Keeping apps up to date

Apps don't receive template changes automatically. Fix platform bugs (`Security`, `Tenancy`, analyzers) in this repository first, then port the same commit to each app. When two apps are in production, extract those projects to NuGet packages (see [tenancy.md](tenancy.md#extracting-to-packages)).

## Audit

```powershell
gh repo view alejandrolazarte/class-manager --json isTemplate,visibility
dotnet new list class-manager
```
