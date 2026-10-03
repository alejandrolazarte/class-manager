# class-manager

Multi-tenant management app for class-based businesses (swimming, yoga, pilates, functional training): students, weekly group classes, enrollments, attendance and monthly fees. Each business (an instructor or a studio) signs up and gets its own tenant; all businesses share one database, isolated by `TenantId`.

Generated from [app-template](https://github.com/alejandrolazarte/app-template) on 2026-09-25. Fix platform bugs (`Security`, `Tenancy`, analyzers) in the template first, then port them here. What the MVP includes: [MVP plan](docs/mvp-plan.md).

- **Backend**: ASP.NET Core Minimal API (.NET 10) + EF Core + SQL Server
- **Frontend**: Expo (React Native) for Android, iOS and web

## Requirements

- .NET 10 SDK
- Node.js 22 LTS and pnpm 10 (`corepack enable` picks the version pinned in `app/package.json`)
- Podman with a running machine (`podman machine start`), only for SQL Server (dev database and integration tests)

## First-time setup

Enable the repository's git hooks (blocks direct pushes to `main`):

```powershell
git config core.hooksPath .githooks
```

## Workflow

`main` only changes through pull requests: create a branch, push it, open a PR, review, merge on GitHub. See [GitHub protection](docs/github-protection.md).

## Build and test

```powershell
dotnet build class-manager.slnx
dotnet test class-manager.slnx
```

- `tests/ClassManager.Core.U.Tests` — unit tests (domain and use cases), no containers.
- `tests/ClassManager.Api.I.Tests` — integration tests; they spin up SQL Server with Testcontainers on Podman.

## Local development

The API and the app run on the host; only SQL Server runs in a container.

1. Start the development database (data survives restarts in the `class-manager-sql-data` volume):

   ```powershell
   podman run -d --name class-manager-sql -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD="<your-password>" -p 1433:1433 -v class-manager-sql-data:/var/opt/mssql mcr.microsoft.com/mssql/server:2022-latest
   ```

   Afterwards, `podman start class-manager-sql` is enough.

   If a VPN blocks the Podman machine's network (image pulls time out), disconnect it while pulling; cached images keep working afterwards.

2. Copy `src/Api/appsettings.Development.example.json` to `src/Api/appsettings.Development.json` and use the same password in the `BusinessDatabase` connection string. The real file is git-ignored.

   The example uses `tcp:[::1],1433` on purpose: on Windows, Podman publishes the port through WSL's relay, which only listens on IPv6 loopback, and `localhost` times out.

3. Apply migrations and load the demo business (see [Local development (backend)](docs/backend/local-development.md)):

   ```powershell
   dotnet ef database update --project src/Infrastructure --startup-project src/Api --connection "<BusinessDatabase connection string>"
   dotnet run scripts/seed-demo-business.cs -- "<BusinessDatabase connection string>"
   ```

4. Run the API on `http://localhost:5000` in the `Development` environment (from `src/Api/Properties/launchSettings.json`):

   ```powershell
   dotnet run --project src/Api/Api.csproj
   ```

5. Run the Expo app (web on `http://localhost:8081`, or scan the QR code with Expo Go):

   ```powershell
   cd app
   pnpm install
   pnpm expo start
   ```

   See [Running the Expo app](docs/frontend/running-the-app.md) for configuration and frontend tests.

## Docs

- [CLAUDE.md](CLAUDE.md) — conventions and rules for contributors and AI agents
- [MVP plan](docs/mvp-plan.md) — goal, scope, domain sketch and milestones
- [Pilot plan: DF Swimming Team](docs/pilot-df-swimming.md) — what the first pilot needs, in phases (private lessons, coach accounts)
- [GitHub protection](docs/github-protection.md) — how `main` is protected on a free private repo, and audit commands
- [Dependency and secret scanning](docs/dependency-and-secret-scanning.md) — Dependabot alerts and version updates, push protection, gitleaks, and what to do when a secret leaks
- [Running the e2e tests](docs/e2e/running-e2e-tests.md) — Playwright against the web app, the real API and SQL Server
- [Demo screenshots](docs/e2e/demo-screenshots.md) — load a demo business and capture every screen at phone size
- [Cloud environment](docs/cloud-environment.md) — setup script for the Claude Code on the web environment
- [Hosting plan](docs/hosting-plan.md) — free Azure setup for a pilot, limits and costs to watch
- [Pilot deployment](docs/pilot-deployment.md) — runbook: Azure, GitHub deploy pipeline, Cloudflare Pages, the installable web app (PWA) and the Android APK
- [Domain model](docs/backend/domain-model.md) — entities, invariants and multi-tenancy rules
- [Local development](docs/backend/local-development.md) — migrations, JWT signing key, demo seed data and owner
- [Running the Expo app](docs/frontend/running-the-app.md) — configuration and frontend tests
- [Theming and design tokens](docs/frontend/theming.md) — themes, light/dark, semantic tokens, AppText, Icon and the lint rules that keep screens on the theme
- [Navigation](docs/frontend/navigation.md) — screens stay in their tab, shared screens per tab, ✕ for forms and ← for navigation, and the tests that enforce it
- [Content Security Policy](docs/frontend/content-security-policy.md) — the web app's report-only policy, what it allows and why, how it was tested, and how to enforce it
- [Empty states](docs/frontend/empty-states.md) — an empty list keeps the floating "New…" button as its only create action, and the tests that enforce it
- [Roslyn analyzers](docs/analyzers.md) — test layout and architecture rules enforced by the build
- [Security library](docs/security.md) — what `src/Security` contains, how the app plugs into it, and when to extract it to a NuGet package
- [Authorization](docs/authorization.md) — organizations, brand and branch roles, and the permission every endpoint requires
- [Tenancy library](docs/tenancy.md) — what `src/Tenancy` and `src/Tenancy.AspNetCore` contain and how the app plugs into them
- [Notifications library](docs/notifications.md) — `src/Notifications` and `src/Notifications.Delivery`: email content and branded HTML layout, SMTP, VAPID and web push encryption, and what stays in the app
- [Import and export library](docs/import-export.md) — the import/export engine in `src/ImportExport` and `src/ImportExport.Xlsx`: XLSX and CSV reading, header matching, limits and why exports are XLSX
- [Import and export plan](docs/backend/20260929-import-export/plan.md) — students and coaches from and to a spreadsheet
- [Session substitutes plan](docs/backend/20260929-session-substitutes/plan.md) — another instructor teaches one date of a class
- [Family app, shop and online payments plan](docs/backend/20260929-online-shop-and-payments/plan.md) — family and adult student accounts, products and stock, orders paid at the branch, then Stripe Connect per branch (steps 1–4 built, the rest is a proposal)
- [Subscriptions plan](docs/backend/20261003-subscriptions/plan.md) — one subscription per brand, plans Free (30-day trial, read-only once it ends)/Lite/Pro/Enterprise, features and add-ons checked like permissions, and the `Subscriptions` library (steps 1–3 partly built)
- [Branding and plans](docs/branding-and-plans.md) — what a business can brand, which parts could be paid, and brands with two colors
- [Themes and business icon plan](docs/frontend/20260928-themes-and-business-icon/plan.md) — more color themes chosen by each user, a theme created by the business from its color, and a business icon
- [Brand plan](docs/frontend/20260930-brand/plan.md) — Ajustes → Marca: logo, brand colors with accent, theme lock, welcome screen, and where students and the team see the brand
- [Team notifications](docs/backend/20261001-team-notifications/plan.md) — avisos for coaches and owners (no voy, makeups, new orders), who gets each one, and web push for the team
- [Team home](docs/frontend/20261001-team-home/plan.md) — Inicio replaces Hoy for the team: greeting, next class, tiles and the full day agenda
- [Ajustes and Cobros](docs/frontend/20261002-settings-and-collections/plan.md) — Ajustes grouped by topic with values and sheets for appearance and notifications; Cobros with Cuotas and Pedidos, pending orders badge and counter sale button
- [Branded emails](docs/backend/20261002-branded-emails/plan.md) — one HTML layout with the business logo and colors for every email, plain-text alternative, and why it is an HTML template instead of Razor
