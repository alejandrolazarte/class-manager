# Backoffice

Status: step 1 in progress: the two web builds, the backend projects, the platform roles, signing in with Google and the deploy are built (runbook: [backoffice access](../../backoffice-access.md)); the rest is a proposal.

A platform administration area for the people who run Laztty, not for businesses: see every brand, its subscription and usage, change its plan, give or take away add-ons, read the audit log, manage who may enter and, later, see the app as a user to reproduce a bug. It is a web app installable as a PWA, so the operator can run it from their phone.

Today the operator changes a plan with `scripts/set-subscription.sql` in the Azure portal ([runbook](../../changing-a-subscription.md)), adding an add-on is not scripted, and the audit log is read with Azure Storage Explorer ([audit log](../20261012-audit-log/plan.md#reading-it)). The backoffice replaces all three with screens.

## Decisions

### The operators are users of the Laztty organization

- Laztty signs up from the app like any customer ([runbook](../../changing-a-subscription.md#your-own-business)) and gets `enterprise` at 0. Its id goes in configuration (`Backoffice:PlatformOrganizationId`), never in code or a migration: the repository is public and migrations run in every environment.
- An operator is a person with an account and a user in a branch of that organization, like any team member. Their user is what the audit columns store (`CreatedByUserId`, `UpdatedByUserId`, `DeletedByUserId`) when they act from the backoffice, so "who changed this" has the same answer everywhere.
- Two permissions, `platform.view` and `platform.manage`, in a system role **Operador de plataforma** (both) and **Lector de plataforma** (`platform.view`). They only count for users of the platform organization.
- **The app can never grant them.** The team screens don't offer the platform roles, custom roles can't include `platform.*` (the API refuses it, and a test proves it for every business, Laztty included), and only the backoffice assigns them. The owner of Laztty in the app manages its team like any business, and still can't make anyone an operator. **Built:** `PlatformOperator` and `PlatformReader` are `BusinessRole` values with their permissions in `SystemRolePermissions`; `platform.*` is `Permissions.PlatformOnly`, outside `All` and `Assignable`; `MemberRules.CanManageRoleAsync` refuses every platform role, so the app can't invite, give, change or remove one; the team screen lists such a member without opening it. The rule doesn't depend on the business, so it holds for Laztty without special cases.
- That they only count in the Laztty organization is checked by the backoffice when it signs an operator in and on every refresh (`Backoffice:PlatformOrganizationId`), with the OIDC sign-in: in the app they grant nothing, because no app endpoint requires `platform.*`.
- A user of another business shown in the app as the author of a change made by an operator (`UpdatedByUserId` points to a Laztty user the business can't read) shows as "Soporte Laztty".

### Signing in: OpenID Connect, plus an identity enabled by hand

Being an operator takes **two independent conditions**, both managed only from the backoffice:

1. The user has `platform.*` in the Laztty organization.
2. The identity they sign in with (provider + the provider's id for them) is enabled in the backoffice.

A partner in Laztty, even its owner in the app, doesn't enter the backoffice until an operator does both. Stealing an operator's app password or session doesn't either: the backoffice never accepts the app's sign-in.

**The provider only proves who someone is.** `Backoffice.Api` signs people in with standard OpenID Connect (`AddOpenIdConnect`), not with Container Apps' built-in authentication, so it works the same locally, in tests and on any host, and changing provider is configuration. Today the provider is **Google**, with the operator's personal Gmail (MFA with a passkey, recovery not only by SMS). Microsoft can be added later the same way.

```json
"Backoffice": {
  "Google": { "Authority": "https://accounts.google.com", "ClientId": "<Laztty Backoffice client>" },
  "PlatformOrganizationId": "<Laztty organization id>",
  "WebAppUrl": "https://admin.laztty.com"
}
```

One provider today, so one section; a second provider adds its own section and scheme.

- The OAuth client `Laztty Backoffice` is its own, separate from any client the app uses later. Its secret is a Container Apps secret.
- **Identities, not emails.** The backoffice looks up `(provider, provider's id)` in its own table `BackofficeLogins` (`Provider`, `ProviderKey`, `AccountId`, `EnabledAt`, `EnabledByUserId`): Google's `sub`, and for Microsoft `tid` + `oid` (its `sub` changes per application). The email is shown, never trusted to identify: another provider can carry anyone's email (nOAuth), and emails change and get reused.
- **The table is the backoffice's own**, not `identity.AspNetUserLogins`, which the app will use for its own social sign-in. The app's code paths that list, add or remove logins never see it: an analyzer rule keeps `Api` and `Core` from referencing it, and a test proves that linking a provider from the app never enables the backoffice. **Built:** `BackofficeLogins` and `BackofficeAccessRequests` live in their own `BackofficeDbContext` (schema `backoffice`, its own migrations in `Backoffice.Infrastructure`, applied by `scripts/migrate-database.mjs` with `Backoffice.Api` as startup project), so the app's assemblies can't even name them. The test about linking a provider from the app comes with the app's social sign-in.
- **Enabling someone, never automatic.** The OIDC handler validates the token and stores nothing; `OnTokenValidated` only looks the identity up:
  1. The person opens `admin.laztty.com` and signs in with Google.
  2. Their identity is not enabled: they see "Acceso no permitido" with a short code (`K7-3QF`), and the backoffice saves an **access request** (provider, id, email, code, date) and a security event.
  3. An operator sees the request in **Operadores**, confirms the code with the person through another channel (a call, a message), and enables it, choosing their Laztty user and role.
  4. The person signs in again and enters.

  The code makes sure the operator enables the right request, not someone who tried just before with a similar email. The first operator is enabled with a script (`scripts/enable-first-operator.sql`, run in the portal like `set-subscription.sql`).
- **Disabling** removes the identity from `BackofficeLogins` and revokes the person's sessions (until the Operadores screen, with the SQL in the [runbook](../../backoffice-access.md#disabling-someone-or-a-lost-phone)).
- Every backoffice sign-in sends the operator an email ("Nuevo inicio de sesión en el backoffice"), so an access they didn't make shows at once. Not built yet, nor the security event of an access request: both need the audit log worker in the backoffice and come with the Operadores screen.

### Sessions: the app's JWT model, with its own keys

The backoffice reuses `src/Security` and the app's web session ([web session cookie](../../web-session-cookie.md)): an access token in memory and the refresh token in an `HttpOnly` cookie. `admin.laztty.com` and `admin-api.laztty.com` are the same site, so `SameSite=Strict` works as it does for the app.

1. "Entrar con Google" opens `admin-api.laztty.com/api/auth/signin/google`.
2. Google returns to `Backoffice.Api`, which checks the identity and `platform.*`, issues a refresh token in the cookie and redirects to `admin.laztty.com`.
3. The web app calls `/api/auth/refresh` with `X-Refresh-Token-Cookie` and keeps the access token in memory.

What is shared is code; what keeps the two apart is configuration and tests:

| | App | Backoffice |
|---|---|---|
| Signing key | The API's secret | Its own secret |
| Issuer and audience | `class-manager` | `laztty-backoffice`: each API refuses the other's tokens |
| Access token | 15 minutes | 10 minutes |
| Session | 30 days, renewed | 8 hours from sign-in, never extended by rotation |
| Refresh token `Kind` | The app's kinds | `backoffice`: the backoffice accepts only it, the API refuses it |
| Cookie | `class-manager-refresh-token` | `laztty-backoffice-refresh-token` |
| CORS | `app.laztty.com` | `admin.laztty.com` only |

- Every refresh checks again that the identity is enabled and the user has `platform.*`, so a disabled operator is out within one access token (10 minutes).
- **Built:** the session doesn't grow with refreshes because `JwtOptions.ExtendSessionOnRefresh` is `false` for the backoffice: each rotated refresh token keeps the first one's expiry. The app's `BusinessTokenSubjectResolver` refuses any kind other than `team` and `student`, so a backoffice refresh token never opens an app session, and the backoffice's resolver only accepts `backoffice`.
- A leaked app signing key can't mint backoffice tokens, and the other way round.
- Integration tests sign in through a test OIDC scheme, so they don't depend on Google.

### Separate hosts

- **`Backoffice.Api`** is a second ASP.NET Core host and a second container app in the same environment (`admin-api.laztty.com`). It is not a role inside `src/Api`: the API's model is one tenant per request, the backoffice reads every brand, and the API must not be able to read the audit log ([access](../20261012-audit-log/plan.md#access-private-and-append-only)). The backoffice's managed identity gets the read roles; the API keeps only the writer.
- **The web app** is a second Azure Static Web App (`admin.laztty.com`), built from the same Expo project as the app ([Web app](#web-app-same-expo-project-two-builds)).

### Audited like the API

- Backoffice use cases run through the same pipeline and behaviors. The behaviors' registration moved from `src/Api/UseCaseServiceCollectionExtensions.cs` to `Infrastructure` (`AddUseCaseBehaviors()`, with the `AddUseCase` helper) and both hosts call it, so every backoffice command is logged by `CommandAuditBehavior` without changes to the backoffice. **Built.**
- The command log is written by `AuditLogWorker`, which lives in `src/Api/Audit`. It moves to `Infrastructure` and `Backoffice.Api` starts it with the first backoffice command; until then the backoffice has none to write.
- The command log row gets `Source` (`App` or `Backoffice`). The user is the operator's Laztty user, and `TenantId` the branch the command changed.

### Personal data stays out

- The backoffice shows brands and branches (name, owner's name and email, country, creation date), subscriptions, add-ons and usage against limits. It does not list students, guardians, payments or anything about minors.
- The command log viewer shows the payload as stored, already without secrets and with personal fields reduced ([payload](../20261012-audit-log/plan.md#payload-yes-but-without-sensitive-fields)).
- Seeing a business's data as its users see it is impersonation (step 4): recorded, time-limited and read-only by default.

## Projects

```
src/Backoffice.Api/            host: Program.cs, endpoints, OIDC, JWT, Containerfile
src/Backoffice.Core/           use cases: brands, subscriptions, add-ons, operators, logs
src/Backoffice.Infrastructure/ cross-tenant repositories, BackofficeLogins, audit log reader, DI extensions
tests/ClassManager.Backoffice.Core.U.Tests/  use cases with in-memory fakes
tests/ClassManager.Backoffice.Api.I.Tests/   endpoints and persistence (Testcontainers: SQL Server, Azurite)
```

- `Backoffice.Core` references `Core` (entities, `ICommand`/`IQuery`, `UseCasePipeline`, abstractions) and `Subscriptions`. The domain is not duplicated: changing a plan uses the same `Subscription` and `SubscriptionFeature` rules as everywhere else.
- `Backoffice.Infrastructure` references `Infrastructure` and `Security`, and reuses `AppDbContext`, the interceptors and the behaviors. Only what the API must not have lives there: repositories that read across brands, `BackofficeLogins` and the audit log reader.
- Same database and migrations, in `Infrastructure`, applied by the API's deploy as today. New tables: `BackofficeLogins` and `BackofficeAccessRequests` (step 1), `ImpersonationGrants` (step 4).
- **ARCH011:** assemblies `Api` and `Core` have no `using` of `ClassManager.Backoffice.*`. ARCH001 applies to `Backoffice.Core` (no EF Core, ASP.NET or Infrastructure), and ARCH002/ARCH010 to every backoffice assembly; the existing analyzers already match them by name. **Built.**
- **Skeleton (built):** `Backoffice.Api` answers `/health` (with the database check) and calls `AddBackofficeInfrastructure()`: `AddInfrastructure()`, `AddSessionClaims()` (the current user and the tenant from the token's claims, moved from `src/Api` to `Infrastructure` so both hosts read them the same way) and `AddUseCaseBehaviors()`. Its JWT issuer and audience are `laztty-backoffice` with 10-minute access tokens; the signing key is its own secret. The 8-hour session is enforced with the sign-in (step 1, OIDC). `Backoffice.Core` has no use cases yet. `tests/ClassManager.Backoffice.Api.I.Tests` proves the host starts and audits commands around the transaction; `ClassManager.Backoffice.Core.U.Tests` comes with the first use case.

### Tenant context

`ClaimsTenantContext` throws when the token has no `tenant_id`. A backoffice token carries the operator's own Laztty branch, which is never the business they act on:

- Reads across brands use `IgnoreTenantFilter()` in `Backoffice.Infrastructure` repositories. Billing tables (`Subscriptions`, `SubscriptionFeatures`) and `Organizations` are not tenant-owned and need nothing.
- A command on another business's branch establishes it first with `ITenantScope.Establish(tenantId)`, so stamping and filters work as in the API.
- A test proves a backoffice read never falls back to the operator's own tenant by mistake.

## Web app: same Expo project, two builds

The backoffice's screens live in the existing Expo project and share `src/ui`, the theme, motion, i18n and the API client; each build only carries its own screens:

```
app/
  app/          routes of the customer app (unchanged)
  backoffice/   routes of the backoffice
  src/
    ui/, theme/, i18n/, …   shared
    features/…              customer app only
    backoffice/…            backoffice only (screens, its session client)
```

- `app.config.js` picks the routes folder (`extra.router.root`, which expo-router 57 reads) from `APP_TARGET`, and names the build **Laztty Admin**. `pnpm export:web` builds the app into `dist` as today; `pnpm export:backoffice` (`scripts/exportBackoffice.js`, so it also works from PowerShell) builds the backoffice into `dist-backoffice`. Metro bundles only the chosen routes and what they import: the backoffice bundle is about 1.1 MB against the app's 3.8 MB. Builds can alternate without clearing Metro's cache. **Built.**
- The backoffice is web only: no EAS builds.
- **Kept apart by checks:** `no-restricted-imports` stops everything outside the backoffice from importing `@/backoffice`, and `backoffice/` and `src/backoffice` from importing `@/features` and `@/navigation` (tests in `eslint/__tests__` prove both against the real config). The `frontend` CI job builds both and runs `pnpm check:bundles` (`scripts/checkBundleSeparation.js`), which fails if one bundle contains a route only the other has, or none of its own routes (then the check would prove nothing); the route keys of `require.context` survive minification. knip runs with both entries. **Built.**
- `src/forms` imports from `src/features` today, so the backoffice can't use its date fields until they move to shared code.
- `CLAUDE.md`'s rules apply as in the app: buttons from `src/ui`, skeletons, empty states, required fields, confirm dialogs, texts in `es.json` and `en.json` under a `backoffice` key.

### PWA on the phone

- Its own manifest: name **Laztty Admin**, its own icon (the Laztty mark on an ink tile, to tell it apart from the app's violet one), scope `admin.laztty.com`. Installed from Chrome on Android, it is one more icon on the phone.
- Every screen is designed phone first, like the app: lists and forms at 390 px, the sign-in to Google in the same window.
- On Android, Chrome and the installed web app share cookies, so the Google redirect returns to the PWA with the session. iOS keeps an installed web app's cookies apart from Safari and the redirect can land in Safari; it is not supported until an operator needs it (then: sign in through a pop-up and hand the session over with a one-time code).
- Losing the phone: the session ends in 8 hours at most, entering again needs the Google account (passkey), and the operator signs out every Google session and is disabled with the script.

## Screens

| Screen | Content | Permission |
|---|---|---|
| Sign in | Laztty Admin mark, "Entrar con Google"; "Acceso no permitido" with the request code | — |
| Inicio | Brands in trial ending this week, ended subscriptions, add-ons expiring, security events of today | `platform.view` |
| Marcas | Every brand: name, owner, plan, status (trial with days left, active, ended), branches, students; search by name or owner email; filter by plan and status | `platform.view` |
| Marca | Branches, current plan and price, usage of counted features, add-ons, subscription history (replaced rows, newest first) | `platform.view` |
| Cambiar plan | Plan, price, currency, note, optional end date; confirm dialog with the consequence (current add-ons are dropped) | `platform.manage` |
| Add-on | Give an add-on or a limit override (feature, price, currency, limit, optional end date); remove the current one, with undo | `platform.manage` |
| Actividad (step 2) | Command log by brand and date, newest first; filter by command, user, outcome | `platform.view` |
| Seguridad (step 2) | Security events of a day: event, branch, user, route, IP | `platform.view` |
| Operadores | Enabled operators and pending access requests with their code; enable (choose user and role) and disable, with confirm | `platform.manage` |

Changing a plan and an add-on follow the [records convention](../../../CLAUDE.md#records-that-change-over-time), the same as the script: `Delete(now)` on the current row and add the new one, in one save.

## Deployment and cost

| Piece | Service | Cost |
|---|---|---|
| `admin.laztty.com` | Second Static Web App, Free plan | None: the Free plan allows 10 apps per subscription, 2 custom domains each; the 100 GB a month are per subscription and shared with the app |
| `admin-api.laztty.com` | Second container app in the existing environment, scale to zero | None while the shared per-subscription allowance lasts; the backoffice adds very little |
| DNS | Two DNS-only records in Cloudflare | None: subdomains of the domain already paid |
| Google sign-in | OAuth client in Google Cloud | None |
| Database | Same Azure SQL free offer | None: a few queries |
| Reading logs | Table and blob reads | Fractions of a cent |

Checked on 2026-10-18 ([Static Web Apps quotas](https://learn.microsoft.com/azure/static-web-apps/quotas)); the USD 1 budget alert in the [hosting plan](../../hosting-plan.md) is the safety net.

- `deploy.yml` builds one more image (`src/Backoffice.Api/Containerfile`) and one more web export, and deploys both like the app's.
- `Backoffice.Api` has its own system-assigned managed identity: a contained SQL user like the API's, and in step 2 Storage Table Data Reader and Storage Blob Data Reader on the logs account.
- Security headers and a Content Security Policy like the app's, and `X-Robots-Tag: noindex`.
- Runbook `docs/backoffice-access.md`: the Google client, the first operator's script, enabling and disabling operators, and what to do when a phone is lost.

## Impersonation (step 4)

To reproduce a bug as a user sees it:

1. An operator chooses a branch and a member or student account and writes a reason.
2. The backoffice saves a one-time grant (`ImpersonationGrants`: target user, tenant, operator's user, reason, `AllowsChanges`, expires in 5 minutes) and shows a link `app.laztty.com/impersonate?code=…`.
3. The API exchanges the code (once) for an access token of 30 minutes **without a refresh token**, with the user's normal claims plus `impersonated_by`. The API signs it; the backoffice never holds the app's key.

Rules, enforced in the API:

- **Read only unless the grant allows changes**: every request that isn't `GET`, `HEAD` or `OPTIONS` answers `403`, like an inactive subscription. Corrections are made from the backoffice under the operator's own user, not by impersonating.
- **Never account actions**, even with changes allowed: password, email, sessions, deleting the account, payments and orders.
- **Recorded**: a security event when the grant is used, and every command logged with `ImpersonatedBy`.
- **Visible**: the app shows a banner while impersonating, with a button to leave.
- The terms and the privacy policy say that support can access an account to solve an incident before this is enabled in production ([legal texts](../../legal-and-privacy.md)).

## Steps

1. **Brands, subscriptions, add-ons and operators.**
   - Prove two web builds from the Expo project (routes root by `APP_TARGET`, the bundle check). **Built.**
   - Move the behaviors' registration to `Infrastructure`. `Backoffice.Api`, `Backoffice.Core`, `Backoffice.Infrastructure`, ARCH011. **Built.**
   - `platform.*` permissions and system roles, refused in custom roles and in the app's team screens. **Built.**
   - OIDC with Google, `BackofficeLogins`, access requests, the sign-in email, JWT with its own key, audience, `Kind` and lifetimes; the first operator's script. **Built**, except the sign-in email.
   - Screens: sign in, Inicio, Marcas, Marca, Cambiar plan, Add-on, Operadores. The PWA manifest. **Marcas and Marca built:** `GET /api/brands` and `GET /api/brands/{id}` (`platform.view`, both roles), audited queries (`[AuditedQuery]`, the list with its row count). `IBrandDirectory` reads across brands with `IgnoreTenantFilter()` and the subscription history with `IncludeDeleted()`; a brand's status is `Trial` (current plan `free`, with the days left), `Active`, `Ended` (the current subscription expired) or `WithoutSubscription`. Search by brand name or owner email and the plan and status filters run in the web app over the whole list. Inicio links to Marcas until it has its own content.
   - Tests: each permission sees only what it may, the app's tokens are refused and the other way round, a disabled identity is refused on refresh, a plan change replaces the subscription and drops its add-ons, an add-on overrides the plan's limit.
   - Second Static Web App, container app, DNS and `docs/backoffice-access.md`. `docs/changing-a-subscription.md` points to the backoffice and the script stays as a fallback. **Deploy built** ([backoffice deployment](../../backoffice-deployment.md)), with the sign-in screen and a first screen showing who is signed in.
2. **Logs.** After the audit log's steps 3 to 5: `IAuditLogReader` (Table Storage and append blobs, Azurite in tests), the reader roles, screens Actividad and Seguridad, `Source` in the command log row.
3. **Corrections.** Backoffice commands that fix a business's data under the operator's user, added one by one when a real case needs them.
4. **Impersonation.** `ImpersonationGrants`, the code exchange in the API, the read-only and account-action rules, the banner and the legal text update.

Each step starts with its failing tests, as every change in the repository.

## Open questions

- Should a brand owner see in the app that support entered their branch, and when? It is one query on the command log by `ImpersonatedBy` once step 4 exists.
