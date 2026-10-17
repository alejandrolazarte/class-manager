# Backoffice

Status: proposal.

A platform administration area for the people who run Laztty, not for businesses: see every brand, its subscription and usage, change its plan, give or take away add-ons, read the audit log and, later, sign in to the app as a user to reproduce a bug.

Today the operator changes a plan with `scripts/set-subscription.sql` in the Azure portal ([runbook](../../changing-a-subscription.md)), adding an add-on is not scripted, and the audit log is read with Azure Storage Explorer ([audit log](../20261012-audit-log/plan.md#reading-it)). The backoffice replaces both with screens, behind a sign-in that is stronger than the app's and not shared with it.

## Decisions

### A separate host, not a role in the API

The backoffice is a second ASP.NET Core host, `src/Backoffice.Api`, deployed as its own container (a second container app in the same environment, `admin.<domain>`). It is not a "platform admin" role inside `src/Api`:

- **The API can't read the audit log, on purpose.** Its identity can only add entries, so whoever compromises the API can't read other businesses' logs or erase their tracks ([access](../20261012-audit-log/plan.md#access-private-and-append-only)). A log viewer in the API would need read access for the API's identity. The backoffice has its own identity with the read roles, and the API keeps only the writer.
- **The app's sign-in is not enough for this.** App accounts have email and password, no second factor, and a password reset by email. The API signs its own JWTs, so a leaked signing key would mint a platform admin token. The backoffice signs people in with an external identity provider with mandatory MFA ([Sign-in](#sign-in-openid-connect-with-microsoft-entra-as-the-provider)) and never trusts the app's tokens.
- **Cross-tenant code stays out of the API.** The API's whole model is "one tenant per request, from the `tenant_id` claim". The backoffice reads every brand, so its queries use `IgnoreTenantFilter()` throughout. Keeping them in their own projects means the API binary doesn't contain them, and an analyzer rule keeps it that way ([Projects](#projects)).

### Sign-in: OpenID Connect, with Microsoft Entra as the provider

- The backoffice signs people in with standard **OpenID Connect** in code (`AddOpenIdConnect` + a cookie), not with Container Apps' built-in authentication (Easy Auth). Easy Auth is Azure only and doesn't run locally; OIDC works the same in development, in tests and in any host, and moving to another provider (Google Workspace, Auth0, Okta, Keycloak) is a configuration change.
- The provider today is **Microsoft Entra ID**, the directory the operator already signs in to the Azure portal with. Configuration (`appsettings.json` + secrets):

```json
"Backoffice": {
  "Authentication": {
    "Authority": "https://login.microsoftonline.com/<directory id>/v2.0",
    "ClientId": "<backoffice app registration>",
    "RoleClaimType": "roles"
  }
}
```

- The client secret (or certificate) is a Container Apps secret, never in the repository. No person's id or email is in configuration or code either: the repository is public, and who may enter is decided in the provider ([Access](#access)).
- **MFA** is required by the provider: Entra's security defaults (free) for every user of the directory. Conditional Access (country, device) needs Entra ID P1 and is not needed at first.
- The session cookie is `HttpOnly`, `Secure`, `SameSite=Strict`, lasts at most 8 hours, and doesn't slide: an operator removed in the provider is out by the end of the day at the latest. Forms use ASP.NET Core antiforgery.
- Integration tests replace OIDC with a test authentication scheme that sets the roles, so they don't depend on the provider.

### Access: roles assigned in the provider

The backoffice defines two roles. They arrive in the token (`roles` in Entra; the claim name is configurable) and every endpoint requires one, like the API requires a permission:

| Role | Can |
|---|---|
| `Backoffice.Reader` | See brands, subscriptions, usage, the command log and security events |
| `Backoffice.Operator` | Everything above, plus change a plan, give and take away add-ons, and impersonate (step 3) |

- A signed-in person with neither role gets `403`. Every route requires a role explicitly; a test lists the endpoints and fails on one without a role requirement.
- In Entra the app registration defines both as app roles and its enterprise application has **Assignment required = Yes**, so only assigned people can even sign in.
- Giving someone access: invite them as a guest (they sign in with their own account) or create a member, then assign them to the backoffice app with a role. Removing access: unassign or disable them. Neither gives any permission in Azure itself (portal, database, secrets, storage): those are separate (Azure RBAC) and operators get none. Runbook: `docs/backoffice-access.md` (step 1).

### The backoffice is audited like the API

- Backoffice use cases run through the same pipeline and the same behaviors as the API's. Today the behaviors are registered in `src/Api/UseCaseServiceCollectionExtensions.cs`; that registration moves to `Infrastructure` (`AddUseCaseBehaviors()`) and both hosts call it. When the audit log's `CommandAuditBehavior` lands, every backoffice command is logged without changes to the backoffice.
- The command log's actor is not an app user. Its row gets `ActorKind` (`User` or `Operator`) and, for operators, the provider's subject id and display name, so "who changed this brand's plan" has an answer. The audit columns (`DeletedByUserId`, `UpdatedByUserId`) point to `Users` and stay `null` for operators, as they are today for `set-subscription.sql`.
- The backoffice can start before the audit log is built: it is covered from the moment the behavior exists.

### Personal data stays out, except to reproduce a bug

- The backoffice shows brands and branches (name, owner's name and email, country, creation date), subscriptions, add-ons and usage against limits. It does not list students, guardians, payments or anything about minors.
- The command log viewer shows the payload as stored, already without secrets and with personal fields reduced ([payload](../20261012-audit-log/plan.md#payload-yes-but-without-sensitive-fields)).
- Impersonation (step 3) is the only way to see a business's data, and it is recorded, time-limited and read-only by default.

### UI: server-rendered pages in the backoffice itself

- Razor Pages served by `Backoffice.Api`, styled with a small stylesheet that uses the app's colors and font. Not in the Expo app: the customer app never ships a line of backoffice code, and the backoffice needs no build pipeline of its own.
- Texts are in Spanish only, in one constants class: it is an internal tool for the operators. If an operator needs English, the texts move to resource files then.
- The screens follow the app's rules where they apply: confirm before an action that touches money (changing a plan, removing an add-on), required fields marked, generic placeholders.

## Projects

```
src/Backoffice.Api/            host: Program.cs, Razor Pages, endpoints, OIDC, Containerfile
src/Backoffice.Core/           use cases: queries and commands for brands, subscriptions, add-ons, logs
src/Backoffice.Infrastructure/ cross-tenant repositories, audit log reader, DI extensions
tests/ClassManager.Backoffice.Core.U.Tests/  use cases with in-memory fakes
tests/ClassManager.Backoffice.Api.I.Tests/   endpoints and persistence (Testcontainers: SQL Server, Azurite)
```

- `Backoffice.Core` references `Core` (entities, `ICommand`/`IQuery`, `UseCasePipeline`, abstractions) and `Subscriptions`. The domain is not duplicated: changing a plan uses the same `Subscription` and `SubscriptionFeature` rules as everywhere else.
- `Backoffice.Infrastructure` references `Infrastructure` and reuses `AppDbContext`, the interceptors and the behaviors. Only what the API must not have lives there: repositories that read across brands and the audit log reader.
- Same database, same migrations: the backoffice adds no tables in steps 1 and 2. Migrations stay in `Infrastructure` and are applied by the API's deploy, as today.
- **ARCH011:** assemblies `Api` and `Core` have no `using` of `ClassManager.Backoffice.*`. The existing ARCH001 applies to `Backoffice.Core` too (no EF Core, ASP.NET or Infrastructure), and ARCH002/ARCH010 to every backoffice assembly.

### Tenant context

`ClaimsTenantContext` throws when the token has no `tenant_id`, and an operator's token never has one. In the backoffice:

- Reads across brands use `IgnoreTenantFilter()` in `Backoffice.Infrastructure` repositories. Billing tables (`Subscriptions`, `SubscriptionFeatures`) and `Organizations` are not tenant-owned and need nothing.
- A command on one branch establishes it first with `ITenantScope.Establish(tenantId)`, which already exists, so tenant stamping and filters work as in the API. None of the step 1 commands need it: they change billing rows of an organization.
- A test proves a backoffice read with no established tenant never falls back to an arbitrary tenant: it either ignores the filter explicitly or fails.

## Screens (step 1 and 2)

| Screen | Content | Role |
|---|---|---|
| Marcas | Every brand: name, owner, country, plan, status (trial with days left, active, ended), price, branches, students; search by name or owner email; filter by plan and status | Reader |
| Marca | Branches, current subscription and add-ons, usage of counted features ("Alumnos: 24 de 30"), subscription history (replaced rows, newest first) | Reader |
| Cambiar plan | Plan, price, currency, note, optional end date; confirm dialog with the consequence ("Los add-ons actuales se pierden") | Operator |
| Add-ons | Give an add-on or a limit override (feature, price, currency, limit, optional end date); remove the current one | Operator |
| Actividad (step 2) | Command log of a brand's branches by date range, newest first; filter by command, user, outcome | Reader |
| Seguridad (step 2) | Security events of a day: event, branch, user, route, IP | Reader |

Changing a plan and an add-on follow the [records convention](../../../CLAUDE.md#records-that-change-over-time), the same as the script: `Delete(now)` on the current row and add the new one, in one save. Add-ons belong to a subscription, so the plan change screen shows which add-ons will be lost and links to give them again.

## Deployment

- One more image built by `deploy.yml` from `src/Backoffice.Api/Containerfile`, pushed to `ghcr.io` next to the API's.
- A second container app in the existing environment, scaling to zero (the free allowance is per subscription and the backoffice is used a few times a week; the first page after a quiet period takes a few seconds).
- Its own system-assigned managed identity:
  - Azure SQL: a contained user with read and write on the tables the use cases touch, like the API's.
  - Logs storage account: Storage Table Data Reader and Storage Blob Data Reader (step 2). The API's identity keeps only the writer role.
- `admin.<domain>` as a DNS-only record, like `app.` and `api.` ([domain and DNS](../../domain-and-dns.md)).
- Security headers and a strict Content Security Policy (no third-party scripts), `X-Robots-Tag: noindex`.
- What ties it to Azure is configuration only: the OIDC provider, the managed identity and the storage reader. The audit log reader sits behind `IAuditLogReader`, like `src/Storage` keeps Blob Storage behind an interface.

## Impersonation (step 3)

To reproduce a bug as a user sees it:

1. An operator chooses a branch and a member or student account in the backoffice and writes a reason.
2. The backoffice saves a one-time grant in SQL (`ImpersonationGrants`: target user, tenant, operator subject, reason, `AllowsChanges`, expires in 5 minutes) and shows a link `app.<domain>/impersonate?code=…`.
3. The API exchanges the code (once) for an access token of 30 minutes **without a refresh token**, with the user's normal claims plus `impersonated_by`. The API signs it; the backoffice never holds the JWT signing key.

Rules, enforced in the API:

- **Read only unless the grant allows changes**: with `impersonated_by` and no permission to change, every request that isn't `GET`, `HEAD` or `OPTIONS` answers `403`, like an inactive subscription.
- **Never account actions**, even with changes allowed: password, email, sessions, deleting the account, payments and orders.
- **Recorded**: a security event when the grant is used, and every command logged with the operator as `ImpersonatedBy`.
- **Visible**: the app shows a banner while impersonating, with a button to leave (new text in `es.json` and `en.json`).
- The terms and the privacy policy must say that support can access an account to solve an incident before this is enabled in production ([legal texts](../../legal-and-privacy.md)).

## Steps

1. **Brands and subscriptions.** Move the behaviors' registration to `Infrastructure`. `Backoffice.Api`, `Backoffice.Core`, `Backoffice.Infrastructure`, ARCH011, OIDC with the two roles and the test scheme. Screens Marcas, Marca, Cambiar plan and Add-ons, with tests: each role sees only what it may, a reader can't change anything, a plan change replaces the subscription and drops its add-ons, an add-on overrides the plan's limit. Container app, image, DNS and `docs/backoffice-access.md`. `docs/changing-a-subscription.md` points to the backoffice and the script stays as a fallback.
2. **Logs.** After the audit log's steps 3 to 5: `IAuditLogReader` (Table Storage and append blobs, Azurite in tests), the reader roles for the backoffice's identity, screens Actividad and Seguridad, and `ActorKind` in the command log row.
3. **Impersonation.** `ImpersonationGrants`, the code exchange in the API, the read-only and account-action rules, the banner, and the legal text update.

Each step starts with its failing tests, as every change in the repository.

## Open questions

- Does any operator other than the owner need access in the first months? If not, step 1 can ship with only `Backoffice.Operator` assigned and `Backoffice.Reader` unused until someone joins.
- Should a brand owner see in "Actividad" (in the app) that support entered their branch and when? It costs one query on the command log by `ImpersonatedBy` once step 3 exists.
