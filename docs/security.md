# Security library

`src/Security` (`ClassManager.Security`) holds the parts of authentication that don't depend on businesses: user accounts, JWT access tokens and rotated refresh tokens. It is a project inside this repository, prepared to become a NuGet package **when a second application needs it**.

## Why a separate project and not a package (yet)

- There is one consumer. A package designed for one app guesses the needs of the next one.
- Authentication is still changing (password reset, email confirmation, employee invitations, several businesses per owner). Inside the repository a change is one PR; as a package it's a release plus an update in every app.
- A shared security package spreads a bug to every app at once. If several apps need sign-in, consider an identity provider (Entra External ID, Keycloak, Auth0) before growing this library.

The project boundary gives most of the benefit now: business concepts can't leak into it (analyzer **ARCH003** breaks the build if `Security` uses `ClassManager.Core`, `Infrastructure` or `Api`), and extracting it later is moving a folder.

## What it contains

| Area | Types |
|---|---|
| Accounts | `ApplicationUser`, `ApplicationRole`, `IUserAccountService` (create, verify credentials with lockout), `IPasswordResetService` (single-use reset tokens stored hashed; a reset clears the lockout and revokes every session) |
| Tokens | `ITokenIssuer` (issue, refresh with rotation and reuse detection, revoke), `JwtOptions`, `SecurityClaimTypes`, `TokenSubject` |
| Persistence | `SecurityDbContext` (schema `identity`) and its migrations |
| Hosting | `AddSecurityServices`, `AddSecurityAuthentication` (JWT bearer validation + rate limit policy `authentication`) |

Tokens carry `sub`, `email`, `role` and a **tenant claim whose name the application chooses** (`TenantClaimType`, default `tenant_id`). This application uses the default, `tenant_id` (`TenantClaimTypes.TenantId` from the [tenancy library](tenancy.md)).

## How the application plugs in

```text
Core            ports: IIdentityService, ITokenService, SessionUser (business + BusinessRole), Result
  ▲
Infrastructure  adapters: IdentityService, TokenService, BusinessTokenSubjectResolver
  │                  ▼
  └──────────► Security   generic: IUserAccountService, ITokenIssuer, ITokenSubjectResolver
Api             AddSecurityAuthentication + permission policies (see authorization.md), AddClaimsTenancy
```

- `Core` never references `Security`: use cases keep talking to their own ports and `Result`.
- `Infrastructure` registers the library and its adapters:

  ```csharp
  services.AddSecurityServices(security =>
  {
      security.TenantClaimType = TenantClaimTypes.TenantId;
      security.PasswordMinLength = OwnerAccount.PasswordMinLength;
      security.ConfigureDbContext = (serviceProvider, options) =>
          options.UseSqlServer(appDbConnection, SecurityDbContext.ConfigureSqlServer);
  });
  services.AddScoped<ITokenSubjectResolver, BusinessTokenSubjectResolver>();
  ```

  `ConfigureDbContext` shares the `AppDbContext` connection so sign up (user + business + membership) runs in one transaction.
- `ITokenSubjectResolver` is the only thing the library needs from the app: when a refresh token is exchanged, who is this user now (tenant and role)? It also receives the token's `kind`, an opaque label the app chooses to separate kinds of sessions (this app uses `team` and `family`). The kind is fixed when the session starts, stored in the refresh token and copied to every access token as the `kind` claim, so a refresh can never change it. It receives the tenant to resolve: the one stored in the refresh token, or the one asked for when switching tenant (`ITokenIssuer.RefreshAsync(refreshToken, tenantId)`), or none for tokens issued before tenants were stored. Returning `null` ends the session, or refuses the switch.
- `Api` calls `AddSecurityAuthentication()` and adds its own authorization policies.

## Extracting it to a NuGet package later

When another application needs it:

1. Move `src/Security` to its own repository with its tests.
2. Publish to GitHub Packages (private feed; `NuGet.config` gets the feed, CI gets a token with `read:packages`).
3. Replace the project reference with a package reference; keep the migrations in the package (they belong to `SecurityDbContext`).
4. Version with SemVer; a change to the token format or the schema is a major version.

Until then, keep the rule: nothing business-specific in `src/Security`.
