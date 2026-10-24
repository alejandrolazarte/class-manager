# Error reporting (Sentry)

Date: 2026-10-24

Crashes and failed requests reach [Sentry](https://sentry.io) instead of depending on someone writing through the feedback button ([app feedback](app-feedback.md)). The API and the team and student app (web, Android and iOS) report to two Sentry projects. The backoffice does not report: only the operator uses it.

## Plan and cost

The free **Developer** plan: one person (the owner) signs in to Sentry, and about 5,000 errors a month are stored. The number of people using the app does not count. When the month's quota runs out, Sentry drops new errors until the next month and charges nothing. To keep one bug from using up the quota, set a rate limit per project (below).

## What is reported

| From | What | How |
|---|---|---|
| API | Unexpected exceptions (the 500s that `GlobalExceptionHandler` logs at `Error`) and any other `Error` log, such as a background task failing | `Sentry.AspNetCore`, through the logging integration. `Warning` logs are kept as breadcrumbs of the next error |
| App | Uncaught errors and rejected promises, on the web and on phones | `@sentry/react-native`, started by `startErrorReporting()` in `app/app/_layout.tsx` |

Not reported: expected answers (validation errors, 401, 403, 404, plan limits), which are results and not exceptions, and Entity Framework's own logs: a failed save already reaches Sentry once through the exception handler, and EF's logs repeat the exception text.

## What is kept out

Sentry stores errors about businesses and their students, so the configuration sends as little personal data as possible. `sendDefaultPii` is off on both sides, and before an event leaves:

- **API** (`src/Api/ErrorReporting/ErrorReportPrivacy.cs`): the user, the query string, the headers, cookies, the request body and the environment variables are removed. The message of a `SqlException` is replaced: SQL Server writes the duplicate value of a unique index into it (an email, for example).
- **App** (`app/src/errorReporting/errorReportPrivacy.ts`): the user, the query string (reset password and invitation tokens travel there), the headers and cookies are removed. Tap and console breadcrumbs are dropped, since a tapped element is described by its accessibility label, which can hold a student's name. Request and navigation breadcrumbs stay, without their query string.
- No tracing, session replay or profiling.

Request paths stay (`/api/clients/123`): they hold ids, not names.

Sentry is listed among the providers in the privacy policy (`legalDocuments.ts`, section 5), and the legal terms version moved to 2026-10-24, so every account accepts them again ([legal texts](legal-and-privacy.md#changing-the-texts)).

## Setting it up

1. Create the Sentry account and choose the **European Union (Germany)** data storage location. It can't be changed later.
2. Create two projects: `class-manager-api` (platform ASP.NET Core) and `class-manager-app` (platform React Native).
3. In each project, **Settings → Security & Privacy**: turn on **Data Scrubber** and **Use Default Scrubbers**, and turn on **Prevent Storing of IP Addresses**.
4. In each project, **Settings → Client Keys (DSN) → Configure → Rate Limiting**: 100 errors per hour, so a loop doesn't use up the month.
5. Give the API its DSN (from `class-manager-api` → Client Keys). The DSN is not a secret: it only allows sending errors to the project.

   ```powershell
   az containerapp update --name $containerApp --resource-group $resourceGroup `
     --set-env-vars "Sentry__Dsn=<API DSN>"
   ```

6. Give the web app its DSN and the source maps upload (step 7 of the [pilot runbook](pilot-deployment.md#7-github-variables)). The auth token is an **organization token** (**Settings → Developer Settings → Organization Tokens**), which already knows the EU region:

   ```powershell
   gh variable set EXPO_PUBLIC_SENTRY_DSN --repo $repository --body "<app DSN>"
   gh variable set SENTRY_ORG --repo $repository --body "<organization slug>"
   gh variable set SENTRY_APP_PROJECT --repo $repository --body "class-manager-app"
   gh secret set SENTRY_AUTH_TOKEN --repo $repository --body "<organization token>"
   ```

7. Merge any pull request (or re-run the deploy). Check it in Sentry: **Issues** stays empty until something fails.

Without `Sentry:Dsn` (empty in `appsettings.json`) the API sends nothing, and without `EXPO_PUBLIC_SENTRY_DSN` the app doesn't start Sentry: local development, CI and e2e never report.

## Releases and source maps

- The deploy builds the API image with `--build-arg SOURCE_REVISION=<commit>`, which the `Containerfile` turns into `SENTRY_RELEASE`. The web app gets the same commit in `EXPO_PUBLIC_SENTRY_RELEASE`. An error shows the commit that was running.
- `pnpm export:web` writes source maps (`--source-maps`). `metro.config.js` starts from `getSentryExpoConfig`, which puts a debug id in each bundle and its map, and the bundle reports it with every error, so Sentry finds the right map without matching file names. The deploy uploads `dist/_expo` with `sentry-cli sourcemaps upload` when `SENTRY_AUTH_TOKEN` exists, then deletes every `.map` before publishing, so the source maps never go online. The [rollback](rollback.md) workflow deletes them too and uploads nothing.
- The web policy allows sending to the DSN's origin (`connect-src`, [Content Security Policy](frontend/content-security-policy.md)).

## Not yet

- Source maps and native crash symbols for the Android APK and iOS builds: they need the `@sentry/react-native/expo` config plugin and `SENTRY_AUTH_TOKEN` in EAS. Phones use the web app (PWA) during the pilot.
- Failed requests the app sees but the API never received (no connection, a timeout): they show as toasts and are not reported.
- A `tenant` tag on API errors, to know which business hit the error without reading the path.
