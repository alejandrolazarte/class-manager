# Running the e2e tests

The suite in `e2e/` drives the **Expo web build** with Playwright against the **real API** and a **real SQL Server**. It covers the critical flows only; business rules stay in unit and integration tests.

## What a run does

1. `pnpm db:migrate` applies both EF Core migrations (`AppDbContext` and `SecurityDbContext`) to the database in `E2E_DATABASE_CONNECTION_STRING`, using the `dotnet-ef` version pinned in `dotnet-tools.json`.
2. `pnpm web:build` exports the web app (`expo export --platform web`) into `e2e/.web-build`, pointing it at the API URL.
3. `playwright test` starts the API (`dotnet run`, `Development` environment, random JWT key, high auth rate limit) and a static server for the web build (`serve --single`), then runs the tests.

Every test signs up its own business with a unique email, so the database never needs to be cleaned and tests don't depend on each other.

## Locally

Use a **dedicated database** (`ClassManagerE2E`), never the development one. The SQL Server container used for development works; only the database name changes.

```powershell
podman start class-manager-sql
cd e2e
pnpm install
pnpm exec playwright install chromium
$env:E2E_DATABASE_CONNECTION_STRING = "Server=tcp:[::1],1433;Database=ClassManagerE2E;User Id=sa;Password=<your-password>;TrustServerCertificate=True"
pnpm db:migrate
pnpm test            # builds the web app, then runs the tests
pnpm test:no-build   # reuses the last web build
```

- If the API or the web server is already running on ports 5000 and 8081, Playwright reuses them (locally only). Stop them if they point to another database.
- `pnpm exec playwright show-report` opens the last HTML report; failed tests keep a trace (`pnpm exec playwright show-trace <path>`).
- Optional variables: `E2E_API_URL` (default `http://localhost:5000`), `E2E_WEB_URL` / `E2E_WEB_PORT` (default `http://localhost:8081`), `E2E_JWT_SIGNING_KEY` (random by default).

## In CI

The `e2e` job in `.github/workflows/ci.yml` runs SQL Server as a job service container, then the same three steps. The SA password in the workflow is only for that throwaway container. On failure the job uploads the `playwright-report` artifact (HTML report, screenshots and traces) for 7 days.

## Writing a test

- File layout: `e2e/tests/When_<condition>/Then_<result>.spec.ts`, one test per file, named after the file.
- Prepare data through the API (`support/businessApi.ts`) and use the UI only for the flow under test. The `business` fixture (`support/fixtures.ts`) gives a signed-in business with one client.
- Select by role and label with the Spanish copy (`getByRole("button", { name: "Crear cuenta" })`), the same text users see.
