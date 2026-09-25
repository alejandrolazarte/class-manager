# class-manager

Multi-tenant app template: .NET 10 API + Expo app (Android, iOS and web). Each business signs up and gets its own tenant; all businesses share one database, isolated by `TenantId`. It ships with authentication, tenancy, business settings and one sample tenant-owned entity (`Client`) to copy when adding the real domain.

New app from this template: see [Using the template](docs/template.md).

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
- [Using the template](docs/template.md) — create a new app, what to rename and what GitHub doesn't copy
- [GitHub protection](docs/github-protection.md) — how `main` is protected on a free private repo, and audit commands
- [Running the e2e tests](docs/e2e/running-e2e-tests.md) — Playwright against the web app, the real API and SQL Server
- [Cloud environment](docs/cloud-environment.md) — setup script for the Claude Code on the web environment
- [Hosting plan](docs/hosting-plan.md) — free Azure setup for a pilot, limits and costs to watch
- [Domain model](docs/backend/domain-model.md) — entities, invariants and multi-tenancy rules
- [Local development](docs/backend/local-development.md) — migrations, JWT signing key, demo seed data and owner
- [Running the Expo app](docs/frontend/running-the-app.md) — configuration and frontend tests
- [Roslyn analyzers](docs/analyzers.md) — test layout and architecture rules enforced by the build
- [Security library](docs/security.md) — what `src/Security` contains, how the app plugs into it, and when to extract it to a NuGet package
- [Tenancy library](docs/tenancy.md) — what `src/Tenancy` and `src/Tenancy.AspNetCore` contain and how the app plugs into them
