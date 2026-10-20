# Backoffice access

How operators sign in to the backoffice (`Backoffice.Api`), how the first one is enabled, and what to do when one leaves or loses a device. The design is in the [backoffice plan](backend/20261017-backoffice/plan.md#signing-in-openid-connect-plus-an-identity-enabled-by-hand).

## Two conditions

An operator needs both, and neither is managed from the app:

1. **A platform role** (`PlatformOperator` or `PlatformReader`) as a team member of a branch of the Laztty organization (`Backoffice:PlatformOrganizationId`). The app can't give, change or remove these roles ([authorization](authorization.md)).
2. **An enabled identity**: their Google account (provider `google` and Google's `sub`, never the email) in `backoffice.BackofficeLogins`.

Without both, signing in shows "Acceso no permitido" with a short code and saves an access request (`backoffice.BackofficeAccessRequests`, one per identity, with a new code on every attempt). Every refresh checks both again, so removing either ends the session within one access token (10 minutes).

## Google client

The OAuth client **Laztty Backoffice** in Google Cloud (APIs & Services → Credentials), of type *Web application*:

- **Authorized redirect URIs:** `https://admin-api.laztty.com/signin-google` and, for local development, `http://localhost:5100/signin-google`.
- **Authorized JavaScript origins:** none; the browser never talks to Google from a page of ours.
- The client ID is not a secret and is in `src/Backoffice.Api/appsettings.json` (`Backoffice:Google:ClientId`).
- The client secret is a secret: a Container Apps secret mapped to `Backoffice__Google__ClientSecret` in production, and locally `dotnet user-secrets set "Backoffice:Google:ClientSecret" "<secret>" --project src/Backoffice.Api`.

## Configuration

| Setting | What |
|---|---|
| `ConnectionStrings:BusinessDatabase` | The same database as the API |
| `Authentication:Jwt:SigningKey` | Its own secret, never the API's: a leaked key of one can't mint tokens for the other |
| `Backoffice:PlatformOrganizationId` | Laztty's organization id. Without it nobody is an operator |
| `Backoffice:WebAppUrl` | Where the sign-in returns (`https://admin.laztty.com`) |
| `Backoffice:Google:ClientSecret` | See above |
| `Cors:AllowedOrigins` | The web app's origin only |

Issuer and audience are `laztty-backoffice`, access tokens last 10 minutes and a session ends 8 hours after signing in: refreshing never extends it (`Authentication:Jwt:ExtendSessionOnRefresh` is `false`). The tables live in the `backoffice` schema (`BackofficeDbContext`), migrated by `scripts/migrate-database.mjs` with the rest.

## Signing in

1. The web app opens `GET /api/auth/signin/google`, which redirects to Google.
2. Google returns to `/signin-google`; the handler keeps the identity for 5 minutes in a temporary cookie and continues to `/api/auth/signin/google/complete`.
3. With both conditions, the API sets the `laztty-backoffice-refresh-token` cookie (`HttpOnly`, `Secure`, `SameSite=Strict`, `Path=/api/auth`) and redirects to `Backoffice:WebAppUrl`. Otherwise it redirects to `{WebAppUrl}/sign-in?accessRequestCode=K7-3QF`.
4. The web app calls `POST /api/auth/refresh` with `X-Refresh-Token-Cookie: true` and keeps the access token in memory. `POST /api/auth/signout` revokes the session and deletes the cookie.

Every other endpoint requires a backoffice access token (`kind` `backoffice`). The app's tokens are refused (different key, issuer and audience), and the app's API refuses a backoffice refresh token.

## The first operator

1. Sign up Laztty in the app like any business ([your own business](changing-a-subscription.md#your-own-business)) and set `Backoffice:PlatformOrganizationId` to its organization id.
2. The person opens the backoffice, signs in with Google and gets the code.
3. In the Azure portal (SQL database → Query editor), fill in `scripts/enable-first-operator.sql` (the code, the email of their app account, the organization id and the role) and run it. Their branch role becomes the platform role (a brand owner keeps the whole brand in the app through `BrandOwner`), the identity is enabled and the request is removed.
4. They sign in again.

The script refuses an unknown code, an unknown account, and an account that isn't a team member of exactly one Laztty branch. From the second operator on, an operator enables people from the backoffice's **Operadores** screen (not built yet: until then, the script).

## Disabling someone, or a lost phone

Until the Operadores screen exists, in the Query editor:

```sql
DECLARE @AccountEmail nvarchar(256) = N'person@example.com';
DECLARE @AccountId uniqueidentifier = (SELECT Id FROM [identity].[AspNetUsers] WHERE NormalizedEmail = UPPER(@AccountEmail));
DELETE FROM backoffice.BackofficeLogins WHERE AccountId = @AccountId;
UPDATE [identity].RefreshTokens SET RevokedAt = SYSUTCDATETIME() WHERE UserId = @AccountId AND Kind = N'backoffice' AND RevokedAt IS NULL;
```

The session ends at the next refresh (10 minutes at most). After a lost phone, also sign out every Google session of that account from Google's security page.
