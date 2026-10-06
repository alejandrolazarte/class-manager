# class-manager

Multi-tenant management app for class-based businesses (swimming, yoga, pilates, functional training): students, weekly group classes, enrollments, attendance and monthly fees. Each business (an instructor or a studio) signs up and gets its own tenant; all businesses share one database, isolated by `TenantId`.

## Stack

- **Backend**: ASP.NET Core Minimal API (.NET 10), C#
- **Persistence**: SQL Server via EF Core
- **Frontend**: Expo (React Native) + TypeScript + NativeWind — one codebase for Android, iOS and web
- **Tests**: xUnit + Shouldly + Moq; integration tests use Testcontainers (SQL Server) running on Podman

## Structure

```
src/Api/             ← Minimal API endpoints + composition root (Program.cs)
src/Core/            ← domain entities, use cases, abstractions (no infrastructure dependencies)
src/Infrastructure/  ← EF Core DbContext, repositories, external services, adapters to Security
src/Security/        ← reusable auth library: Identity accounts, JWT + refresh tokens (no business concepts, ARCH003)
src/Tenancy/         ← reusable tenancy abstractions: ITenantOwned, ITenantContext (no dependencies, ARCH004)
src/Tenancy.AspNetCore/ ← tenant query filters, stamping interceptor, claims tenant context (ARCH004)
src/ImportExport/    ← reusable import/export engine: CSV, column matching, limits (no dependencies, ARCH005)
src/Notifications/   ← reusable email and web push building blocks: email content, branded HTML layout, VAPID, push encryption (no dependencies, ARCH006)
src/Notifications.Delivery/ ← SMTP (MailKit) and web push senders, DI extensions (ARCH006)
src/Storage/        ← reusable file storage abstraction and image format detection (no dependencies, ARCH008)
src/Storage.AzureBlob/ ← Azure Blob Storage implementation, DI extensions (ARCH008)
src/Records/         ← record conventions: ICreatedOn, IDeletedOn, IExpiredOn and their rules (no dependencies, ARCH009)
src/Subscriptions/   ← reusable plans, features, add-ons and effective features of a subscriber (no dependencies, ARCH007)
src/Subscriptions.AspNetCore/ ← EF model for the billing schema, per-request feature access, RequireFeature endpoint filter (ARCH007)
tests/ClassManager.Core.U.Tests/  ← unit tests: domain and use cases, no database
tests/ClassManager.Api.I.Tests/   ← integration tests: endpoints and persistence (Testcontainers)
tests/ClassManager.ImportExport.U.Tests/ ← unit tests: import/export engine
tests/ClassManager.Notifications.U.Tests/ ← unit tests: email rendering and web push
tests/ClassManager.Storage.U.Tests/ ← unit tests: image formats and file paths
tests/ClassManager.Storage.I.Tests/ ← integration tests: Azure Blob Storage against Azurite (Testcontainers)
tests/ClassManager.Subscriptions.U.Tests/ ← unit tests: plans, subscriptions and effective features
app/                 ← Expo app (mobile + web)
e2e/                 ← Playwright e2e tests: web build + real API + SQL Server (critical flows only)
docs/                ← decisions and runbooks
scripts/             ← standalone scripts (setup, seeding)
```

## Language

Everything is written in English: code, identifiers, comments in config files, commit messages and docs. The only exception is user-facing UI copy, which is localized.

## RULE: main only changes through reviewed pull requests

- Never push to `main`, never merge pull requests (`gh pr merge`), never use `--no-verify`.
- Work on a branch (`feature/...`, `fix/...`, `chore/...`, `docs/...`), push it and open a PR with `gh pr create`.
- The owner reviews and merges on GitHub. Stop after opening the PR.
- Enforcement: `.githooks/pre-push` blocks pushes to `main`, and `.claude/settings.json` denies the commands above. See [docs/github-protection.md](docs/github-protection.md).

## RULE: GitHub Actions only from `actions/*`

The repository only allows GitHub-owned actions. Any other action (for example `pnpm/action-setup`) makes the whole workflow fail at startup, including jobs that don't use it. Install tools with shell steps instead (`corepack enable` for pnpm).

## Running locally

The API and the Expo app run directly on the host. Containers are used only for SQL Server: a long-lived one for development, and throwaway ones created by Testcontainers for integration tests.

```powershell
dotnet run --project src/Api/Api.csproj   # API (needs the dev database, see README)
cd app; pnpm install; pnpm expo start     # Expo app
```

pnpm does not run dependency install scripts unless they are allowed in `package.json` (`pnpm.onlyBuiltDependencies`). Only allow a package there after checking why it needs a build step.

## Tests

```powershell
dotnet build class-manager.slnx
dotnet test class-manager.slnx
```

Frontend (typecheck + jest):

```powershell
cd app; pnpm typecheck; pnpm test
```

E2E (needs SQL Server and a dedicated `ClassManagerE2E` database, see [docs/e2e/running-e2e-tests.md](docs/e2e/running-e2e-tests.md)):

```powershell
cd e2e; pnpm db:migrate; pnpm test
```

Add an e2e test only for a critical flow that crosses the app and the API; business rules belong in unit and integration tests.

## Before pushing

CI runs the same checks and fails on any of them:

```powershell
dotnet build class-manager.slnx -warnaserror
dotnet test class-manager.slnx
dotnet format class-manager.slnx --verify-no-changes   # dotnet format class-manager.slnx to fix
cd app; pnpm typecheck; pnpm test --ci; pnpm lint; pnpm format:check   # pnpm format to fix
cd e2e; pnpm typecheck; pnpm format:check; pnpm test                     # e2e job
```

Line endings are LF everywhere (`.editorconfig` and `.gitattributes`). Constants and `static readonly` fields are PascalCase; other private fields use `_camelCase`. Naming violations (`IDE1006`) and unnecessary `using` directives (`IDE0005`) break the build and `dotnet format --verify-no-changes`; `dotnet format class-manager.slnx` removes unused usings. Don't disable a lint rule to get green: fix the code, or raise the rule in the PR.

Integration tests start SQL Server and Azurite (Blob Storage emulator) through Testcontainers, which talks to Podman via the `\\.\pipe\docker_engine` pipe. The Podman machine must be running (`podman machine start`).

## Multi-tenancy

- Every tenant-owned table has a `TenantId` column (the business), even when it could be derived through a relationship.
- Tenant isolation is enforced centrally (EF Core global query filters from `src/Tenancy.AspNetCore`, driven by the `tenant_id` claim of the authenticated user), never by remembering to filter in each query. See [docs/tenancy.md](docs/tenancy.md).
- Every new tenant-owned entity needs a test proving that business A cannot read business B's data.

## Code conventions

### Naming
- No abbreviations: `sourceFilePath`, not `src`; `appointment`, not `appt`
- No comments: if you feel the need to comment, improve the name
- Names must communicate intent

### Constants
- No hardcoded strings in logic: use constants in the class or a constants class
- Routes, connection string names, column lengths and file extensions go in constants

### Design
- SOLID: one responsibility per class, depend on abstractions
- Every new service has its interface `I<Name>Service`
- `Core` never references `Infrastructure` or ASP.NET
- Every use case input implements `ICommand` (changes data, runs in a transaction) or `IQuery` (only reads, no tracking), and every use case is registered with `AddUseCase`. Cross-cutting code goes in a behavior, not in each use case. See [docs/backend/use-case-behaviors.md](docs/backend/use-case-behaviors.md).

### Testing (TDD)
- **Test first**: the test fails (RED) before writing the implementation
- Layout: folder `When_<condition>/` → file and class `Then_<result>.cs` → one `[Fact]` per file, method `Then_<result>_Run` (enforced by analyzers TEST001–TEST005, see [docs/analyzers.md](docs/analyzers.md))
- Isolated tests: unit tests use in-memory fakes; database tests use Testcontainers, never a shared dev database
- One main assertion per test (supporting asserts allowed)

### Records that change over time
- A record that is replaced instead of edited (a subscription, an add-on) implements the interfaces in `src/Records`: `ICreatedOn` (`CreatedOn`, date and time it applies from), `IDeletedOn` (`DeletedOn`, date and time another record replaced it, `null` = current, and `Delete(now)`) and, when there is an agreed end, `IExpiredOn` (`ExpiredOn`, date, inclusive).
- Never delete such rows physically and never store an `IsDeleted` column. Query with `WhereCurrent()` and decide with `IsCurrent()` / `IsActiveOn(date)` from `RecordExtensions`; don't rewrite the rule per entity.
- Replacing is one save: `Delete(now)` on the current record and add the new one. Active = current, created on or before the date, and `ExpiredOn` empty or not past.
- Enforced by tests: a column named `CreatedOn`, `DeletedOn` or `ExpiredOn` requires its interface, and every `IDeletedOn` entity needs a unique index filtered on `[DeletedOn] IS NULL`.
- Business dates with their own meaning (a fee that applies from a month) are not this convention and keep their names.

### Deleting
- Every action that deletes asks for confirmation first with `DeleteConfirmation`, and a test proves nothing is deleted until the user confirms.
- Data with business meaning is soft deleted: the entity implements `ISoftDeletable` from `src/Records` (`DeletedOn`, the date and time it was deleted; `IsDeleted`, computed from it and not stored; `Delete(now)`). Removing one is turned into `Delete(now)` by `SoftDeleteSaveChangesInterceptor`, and a global query filter (`SoftDelete`) hides deleted rows from every query, so repositories don't filter by hand, and unique indexes are filtered on `[DeletedOn] IS NULL`. Physical deletes only for rows with no value once gone (push subscriptions, rows the system recreates). See [docs/deleting-data.md](docs/deleting-data.md).

### Empty states
- The create action of a list lives only in its floating action button, which stays visible when the list is empty. `EmptyState` holds no buttons: pass `createActionLabel` to point to the floating button. Every `EmptyState` has an icon (`icon` is required). See [docs/frontend/empty-states.md](docs/frontend/empty-states.md).

### Forms
- Required fields carry `isRequired` (or `withRequiredMark` for choices), the form shows `RequiredFieldsLegend` at the top (except sign-in and the password screens, which users already know), and the main button stays disabled (grey) until every required field is filled (`useRequiredFieldsFilled`, `isFilled`). Never write "(opcional)" in a label. See [docs/frontend/forms.md](docs/frontend/forms.md).

### Documentation
- Operational, security or infrastructure decisions go in a Markdown file under `docs/`, linked from `README.md`.
