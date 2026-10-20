# Running the e2e tests

The suite in `e2e/` drives the **Expo web build** with Playwright against the **real API**, a **real SQL Server** and **Azurite** (the Azure Storage emulator, where the API writes its audit log). It covers the critical flows only; business rules stay in unit and integration tests.

## What a run does

1. `pnpm db:migrate` applies both EF Core migrations (`AppDbContext` and `SecurityDbContext`) to the database in `E2E_DATABASE_CONNECTION_STRING`, using the `dotnet-ef` version pinned in `dotnet-tools.json`.
2. `pnpm web:build` exports the web app (`expo export --platform web`) into `e2e/.web-build`, pointing it at the API URL, and moves package assets out of `node_modules` the same way the production build does.
3. `playwright test` starts an SMTP sink (`support/globalSetup.ts`), the API (`dotnet run`, `Development` environment, random JWT key, high auth rate limit, emails sent to the sink, audit log written to Azurite) and a static server for the web build (`serve --single`), then runs the tests. Before them, the `audit-log` project checks that Azurite answers; after them, the `audit-log-rules` project checks what the run left in the audit log ([Audit log rules](#audit-log-rules)).

Every test signs up its own business with a unique email, so the database never needs to be cleaned and tests don't depend on each other. Signing up needs the link from the confirmation email (see [Sign-up confirmed by email](../backend/20261011-sign-up-confirmation/plan.md)): the sink saves every email the API sends to `e2e/.mailbox/` (emptied at the start of each run), and `support/mailbox.ts` waits for an email and reads its link, from any test worker.

## Locally

Use a **dedicated database** (`ClassManagerE2E`), never the development one. The SQL Server container used for development works; only the database name changes.

```powershell
podman start class-manager-sql
podman run --detach --name class-manager-azurite -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite:3.37.0 azurite --blobHost 0.0.0.0 --queueHost 0.0.0.0 --tableHost 0.0.0.0 --skipApiVersionCheck --inMemoryPersistence   # first time; then podman start class-manager-azurite
cd e2e
pnpm install
pnpm exec playwright install chromium
$env:E2E_DATABASE_CONNECTION_STRING = "Server=tcp:[::1],1433;Database=ClassManagerE2E;User Id=sa;Password=<your-password>;TrustServerCertificate=True"
pnpm db:migrate
pnpm test            # builds the web app, then runs the tests
pnpm test:no-build   # reuses the last web build
```

- If the API or the web server is already running on ports 5000 and 8081, Playwright reuses them (locally only). Stop them if they point to another database.
- A reused API must send its emails to the sink (`Email:Smtp:Host` `localhost`, `Email:Smtp:Port` 2525, `Email:Smtp:FromAddress`), or seeding a business times out waiting for the confirmation email.
- `pnpm exec playwright show-report` opens the last HTML report; failed tests keep a trace (`pnpm exec playwright show-trace <path>`).
- Azurite runs in memory: restarting it empties it. The rules only read what was written since the run started, so leftovers from earlier runs don't count either way.
- Optional variables: `E2E_AUDIT_LOG_CONNECTION_STRING` (default `UseDevelopmentStorage=true`, Azurite on its default ports), `E2E_API_URL` (default `http://localhost:5000`), `E2E_WEB_URL` / `E2E_WEB_PORT` (default `http://localhost:8081`), `E2E_JWT_SIGNING_KEY` (random by default), `E2E_SMTP_PORT` (default 2525), `E2E_MAILBOX_DIR` (default `e2e/.mailbox`).

## In CI

The `e2e` job in `.github/workflows/ci.yml` runs SQL Server as a job service container and starts Azurite with `docker run` (a service container can't take Azurite's arguments), then the same three steps. The SA password in the workflow is only for that throwaway container. On failure the job uploads the `playwright-report` artifact (HTML report, screenshots and traces) for 7 days.

## Writing a test

- File layout: `e2e/tests/When_<condition>/Then_<result>.spec.ts`, one test per file, named after the file.
- Prepare data through the API (`support/businessApi.ts`) and use the UI only for the flow under test. The `business` fixture (`support/fixtures.ts`) gives a signed-in business with one client.
- Select by role and label with the Spanish copy (`getByRole("button", { name: "Crear cuenta" })`), the same text users see.
- Personal data a test types or sends (names, emails, phones, birth dates) goes through `personal(value)` from `support/personalData.ts`, so the audit log rules can prove it never reaches a payload. `support/businessApi.ts` already does it for the data it seeds.

## Audit log rules

Every run ends with `auditLog/auditLog.teardown.ts`, which reads the command log and the security events the run wrote to Azurite and fails with the list of violations. A new test is checked by them without doing anything. The rules (`support/auditLogRules.ts`):

| Rule | The bug it finds |
|---|---|
| At least one command was written | The API is not writing the audit log (a reused API without `ConnectionStrings:AuditLog`), which would make every other rule pass |
| No command ended with `Outcome = Exception` | A use case that threw while the screen looked fine |
| Every `Failed` command is in `expectedFailures` | The app sending requests the API refuses, without the user noticing |
| No payload contains a value passed to `personal()`, nor any email address | A command property missing `[AuditPersonal]` that leaks what a person typed |
| Every command has its business and user, except `commandsWithoutSession` | A command running without a tenant or a session user |
| Every security event is in `expectedSecurityEvents` | A screen calling an endpoint its user may not use (`Forbidden`), a broken session refresh (`RefreshTokenRejected`, `UnauthorizedBurst`) |

A test that makes a command fail on purpose, or a new command that runs before signing in, adds it to `auditLog/auditLogExpectations.ts` with the reason; the list is the record of what is expected.
