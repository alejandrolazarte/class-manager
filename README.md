# class-manager

Multi-tenant management app for class-based businesses (swimming, yoga, pilates, functional training): students, weekly group classes, enrollments, attendance and monthly fees. Each business (an instructor or a studio) signs up and gets its own tenant; all businesses share one database, isolated by `TenantId`. Users know the product as Laztty, see [Laztty brand](docs/brand.md).

Generated from [app-template](https://github.com/alejandrolazarte/app-template) on 2026-09-25. Fix platform bugs (`Security`, `Tenancy`, analyzers) in the template first, then port them here. What the MVP includes: [MVP plan](docs/mvp-plan.md).

- **Backend**: ASP.NET Core Minimal API (.NET 10) + EF Core + SQL Server
- **Frontend**: Expo (React Native) for Android, iOS and web

## License

[Functional Source License 1.1, Apache 2.0 future license](LICENSE.md) (`FSL-1.1-ALv2`): anyone may read, use and change the code for any purpose except offering it as a product or service that competes with class-manager. Two years after each version is published, that version becomes available under Apache 2.0. Why this license and the checklist before making the repository public: [Making the repository public](docs/making-the-repository-public.md).

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
- [Pilot plan: DF Swimming Team](docs/pilot-df-swimming.md) — what the first pilot needs, in phases (private lessons, instructor accounts)
- [GitHub protection](docs/github-protection.md) — how `main` is protected on a free private repo, and audit commands
- [Making the repository public](docs/making-the-repository-public.md) — the FSL license, what the history review found, and the checklist before switching the repository to public
- [Dependency and secret scanning](docs/dependency-and-secret-scanning.md) — Dependabot alerts and version updates, push protection, gitleaks, and what to do when a secret leaks
- [Running the e2e tests](docs/e2e/running-e2e-tests.md) — Playwright against the web app, the real API and SQL Server
- [Demo screenshots](docs/e2e/demo-screenshots.md) — load a demo business and capture every screen at phone size
- [Cloud environment](docs/cloud-environment.md) — setup script for the Claude Code on the web environment
- [Hosting plan](docs/hosting-plan.md) — free Azure setup for a pilot, limits and costs to watch
- [Pilot deployment](docs/pilot-deployment.md) — runbook: Azure, GitHub deploy pipeline, Azure Static Web Apps, the installable web app (PWA) and the Android APK
- [Rolling back a deploy](docs/rollback.md) — the manual Rollback workflow: list the last successful deploys, put one back online (API image and web app, no migrations), and why migrations stay backward compatible
- [Domain and DNS](docs/domain-and-dns.md) — `laztty.com` on Cloudflare Registrar, `app.` and `api.` records, and why no record goes through the Cloudflare proxy (LaLiga blocks in Spain)
- [Database on the Basic tier](docs/database-basic-tier.md) — DTU vs vCore, what Basic costs and includes, and the runbook to move the pilot database from the free offer to Basic and back
- [Domain model](docs/backend/domain-model.md) — entities, invariants and multi-tenancy rules
- [Use case behaviors](docs/backend/use-case-behaviors.md) — the pipeline around every use case: commands run in a transaction, queries don't track, and how to add a behavior
- [Local development](docs/backend/local-development.md) — migrations, JWT signing key, demo seed data and owner
- [Running the Expo app](docs/frontend/running-the-app.md) — configuration and frontend tests
- [Laztty brand](docs/brand.md) — the product name, the mark, its color, where Laztty shows and where the business brand does, and how to render the app icons
- [Theming and design tokens](docs/frontend/theming.md) — themes, light/dark, semantic tokens, the type scale, AppText, Icon and the lint rules that keep screens on the theme
- [Navigation](docs/frontend/navigation.md) — screens stay in their tab, shared screens per tab, ✕ for forms and ← for navigation, and the tests that enforce it
- [Web session cookie](docs/web-session-cookie.md) — on the web the refresh token lives in an `HttpOnly` cookie set by the API, the mobile app keeps it in secure storage, and how cross-site requests are refused
- [Content Security Policy](docs/frontend/content-security-policy.md) — the web app's report-only policy, what it allows and why, how it was tested, and how to enforce it
- [Buttons](docs/frontend/buttons.md) — one button per role (`Button` variants, icon, floating, tonal, links), how to choose, when a new button is justified, and the test that keeps screens from building their own
- [Empty states](docs/frontend/empty-states.md) — an empty list keeps the floating "New…" button as its only create action, and the tests that enforce it
- [Deleting data](docs/deleting-data.md) — every delete asks for confirmation, and data with business meaning is soft deleted with `IsDeleted`, `DeletedAt` and `DeletedByUserId`
- [Currencies and time zones](docs/currencies-and-time-zones.md) — payments, pack sales and orders keep the currency they were charged in, prices are not converted, and classes keep their local time when the business changes its time zone
- [Languages](docs/frontend/languages.md) — Spanish and English UI texts, the language setting saved to the account, email texts per language (`IEmailTexts`), push notification texts per language (team-written texts stay as written), and what stays in Spanish (invitations, legal texts)
- [Forms](docs/frontend/forms.md) — required fields marked with an asterisk, the main button grey until they are filled, and the shared step header and total bar
- [Unused code](docs/frontend/unused-code.md) — knip and the translation keys test fail CI when a file, export, dependency or `es.json` key has no user, and what their configuration allows
- [Continuous integration](docs/ci.md) — the CI jobs, which files run backend or frontend checks on a pull request, and why e2e always runs
- [Roslyn analyzers](docs/analyzers.md) — test layout and architecture rules enforced by the build
- [Security library](docs/security.md) — what `src/Security` contains, how the app plugs into it, and when to extract it to a NuGet package
- [Student accounts](docs/student-accounts.md) — an app account belongs to the person and is kept when no business links it; deleted only when the person asks ("Eliminar mi cuenta" in Mi perfil; pending: home with no business, finding a business)
- [Legal texts and acceptance](docs/legal-and-privacy.md) — aviso legal, términos de uso and política de privacidad, the acceptance required to create an account, and how to publish a new version
- [Ages and minors](docs/ages-and-minors.md) — every account has a birth date, the legal minimum ages in Argentina and Spain, and the age rules the app enforces
- [Countries](docs/countries.md) — the `Countries` table: calling code, currency, minimum age for an own account and time zones of each country, and what a business takes from its country
- [Authorization](docs/authorization.md) — organizations, brand and branch roles, and the permission every endpoint requires
- [Tenancy library](docs/tenancy.md) — what `src/Tenancy` and `src/Tenancy.AspNetCore` contain and how the app plugs into them
- [Notifications library](docs/notifications.md) — `src/Notifications` and `src/Notifications.Delivery`: email content and branded HTML layout, SMTP, VAPID and web push encryption, and what stays in the app
- [Storage library and catalog images](docs/storage.md) — `src/Storage` and `src/Storage.AzureBlob`: product and class pack photos in Azure Blob Storage, the `Documents` table with public/private visibility, photos per plan, cost comparison with Cloudflare R2, Azurite for development and tests
- [Several PDFs per class](docs/backend/20261012-class-material-files/plan.md) — proposal: more than one material PDF per class, paid through the document storage add-on instead of a count of files
- [Import and export library](docs/import-export.md) — the import/export engine in `src/ImportExport` and `src/ImportExport.Xlsx`: XLSX and CSV reading, header matching, limits and why exports are XLSX
- [Import and export plan](docs/backend/20260929-import-export/plan.md) — students and instructors from and to a spreadsheet
- [Session substitutes plan](docs/backend/20260929-session-substitutes/plan.md) — another instructor teaches one date of a class
- [Student app, shop and online payments plan](docs/backend/20260929-online-shop-and-payments/plan.md) — accounts for parents and adult students, products and stock, orders paid at the branch, then Stripe Connect per branch (steps 1–4 built, the rest is a proposal)
- [Subscriptions plan](docs/backend/20261003-subscriptions/plan.md) — one subscription per brand, plans Free (30-day trial, read-only once it ends)/Lite/Pro/Enterprise, features and add-ons checked like permissions, and the `Subscriptions` library, enforced centrally (steps 1–4 built)
- [Changing a brand's subscription](docs/changing-a-subscription.md) — runbook for `scripts/set-subscription.sql`: plan, price and note of a brand, run in the Azure portal
- [Repeated emails before removing the people copies](docs/repeated-emails.md) — runbook for `scripts/list-repeated-emails.sql`: find and fix the emails repeated in a business before the migration that makes them unique
- [Branding and plans](docs/branding-and-plans.md) — what a business can brand, which parts could be paid, and brands with two colors
- [Themes and business icon plan](docs/frontend/20260928-themes-and-business-icon/plan.md) — more color themes (now chosen by the business, not each user), a theme created by the business from its color, and a business icon
- [Brand plan](docs/frontend/20260930-brand/plan.md) — Ajustes → Marca: logo, business theme (built-in or own colors with accent), welcome screen, and where students and the team see the brand
- [Team notifications](docs/backend/20261001-team-notifications/plan.md) — avisos for instructors and owners (no voy, makeups, new orders), who gets each one, and web push for the team
- [Background tasks](docs/backend/20261007-background-tasks/plan.md) — order emails and push notifications queued in the `BackgroundTasks` table, sent and retried by one worker that sleeps until there is work instead of polling the database
- [Audit log](docs/backend/20261012-audit-log/plan.md) — audit columns (`CreatedAt`/`ByUserId`, `UpdatedAt`/`ByUserId`, `DeletedAt` + `DeletedByUserId`, built: `IAuditable` on every tenant table, filled by an interceptor), every command logged to Azure Table Storage with secrets left out and personal fields masked (built), and 403s and failed sign-ins as immutable security events (append blobs under a retention policy, built), in a logs storage account without keys where the API can only add
- [Audit log storage](docs/audit-log-storage.md) — runbook: the `classmanageraudit` storage account without keys, the `Audit Log Writer` role that lets the API only add, `scripts/setup-audit-log-storage.sh` (Cloud Shell) that creates and proves it, reader roles for the owner, audit commands and deleting old command tables
- [Backoffice deployment](docs/backoffice-deployment.md) — runbook: the backoffice's container app, static web app, database user, domains and GitHub variables
- [Backoffice access](docs/backoffice-access.md) — runbook: the Google client, configuration, enabling the first operator with `scripts/enable-first-operator.sql`, and disabling one
- [Backoffice](docs/backend/20261017-backoffice/plan.md) — plan, step 1 in progress: the platform operators' area (brands, subscriptions, add-ons, audit log, operators, impersonation), a second API and a second web build of the Expo project installable as a PWA; operators are users of the Laztty organization with `platform.*`, signed in with Google and an identity enabled by hand, never with the app's accounts
- [Team home](docs/frontend/20261001-team-home/plan.md) — Inicio replaces Hoy for the team: greeting, next class, tiles and the full day agenda
- [Calma redesign](docs/frontend/20261011-calma-redesign/plan.md) — lighter type (Figtree), no cards, hairlines and short animations for both apps and the emails, in steps; no feature or text may change, one component base, no old or unused code left, and before/after screenshots for important changes
- [Confirm before cancelling an order](docs/frontend/20261011-confirm-order-cancellation/plan.md) — pending: "Cancelar pedido" in the student app and in Cobros → Pedidos asks first with `ConfirmDialog`
- [Product stock, sizes and colors](docs/frontend/20261012-product-stock-sizes-and-colors/plan.md) — built in two PRs: the stock in one place below the Stock chips (initial units on create, movements in a sheet on edit) and sizes as chips; then colors from a fixed palette, size × color variants and the student picker
- [Better announcements](docs/frontend/20261014-better-announcements/plan.md) — announcements for some classes, with a category, a photo, a visible-until date and one pinned on top (step 1 built); who saw each one, a reminder and answers to events (step 2 built); scheduling for a later day and time (step 3 built)
- [Logros that make progress exciting](docs/frontend/20261011-student-achievements/plan.md) — proposal: level-up and medal moments, progress as a journey, and the limits for minors
- [Ajustes and Cobros](docs/frontend/20261002-settings-and-collections/plan.md) — Ajustes grouped by topic with values and sheets for appearance and notifications; Cobros with Cuotas and Pedidos, pending orders badge and counter sale button
- [Branded emails](docs/backend/20261002-branded-emails/plan.md) — one HTML layout with the business logo and colors for every email, plain-text alternative, and why it is an HTML template instead of Razor
- [Sign-in emails and app accounts per person](docs/backend/20261006-sign-in-emails/plan.md) — one email per person for invitations and sign-in, why only the person changes their sign-in email (Mi perfil, confirmed by a link), and the proposal for one app account per person with the age rules for Spain
- [Users per business](docs/backend/20261017-users/plan.md) — one `Users` row per person in each branch (contact data, history of who did what, kept when the account is deleted), the account (`AccountId`) only for signing in, and the steps to move every table to it
- [Sign-up confirmed by email](docs/backend/20261011-sign-up-confirmation/plan.md) — the owner gets a link to confirm the email and choose the password; the business is created when the link is opened, and a registered email gets an "account exists" email instead
