# Local development (backend)

How to get a database with demo data, configure authentication and call the API.

## Database schema

The schema is managed by EF Core migrations in `src/Infrastructure/Persistence/Migrations` (application data) and `src/Security/Persistence/Migrations` (user accounts and refresh tokens).

Install the tool once:

```powershell
dotnet tool install --global dotnet-ef
```

There are two `DbContext`s in the same database, each with its own migrations:

| Context | Contents | Migrations folder | History table |
|---|---|---|---|
| `AppDbContext` | Businesses and tenant-owned data (schema `dbo`) | `src/Infrastructure/Persistence/Migrations` | `dbo.__EFMigrationsHistory` |
| `SecurityDbContext` | ASP.NET Core Identity users and refresh tokens (schema `identity`) | `src/Security/Persistence/Migrations` | `identity.__EFMigrationsHistory` |

Apply both to the database the API uses:

```powershell
dotnet ef database update --project src/Infrastructure --startup-project src/Api --context AppDbContext --connection "<BusinessDatabase connection string>"
dotnet ef database update --project src/Security --startup-project src/Api --context SecurityDbContext --connection "<BusinessDatabase connection string>"
```

Add a migration after changing an entity configuration:

```powershell
dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/Api --context AppDbContext --output-dir Persistence/Migrations
dotnet ef migrations add <Name> --project src/Security --startup-project src/Api --context SecurityDbContext --output-dir Persistence/Migrations
```

Integration tests don't need any of this: `ApiFixture` starts its own SQL Server container and migrates both contexts.

## JWT signing key

The API refuses to start without `Authentication:Jwt:SigningKey` (at least 32 bytes, validated at startup). It never goes in `appsettings.json`. Locally, store it with user secrets (the `Api` project has a `UserSecretsId`):

```powershell
dotnet user-secrets set "Authentication:Jwt:SigningKey" "<at least 32 random characters>" --project src/Api
```

One way to generate a key: `[Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))`.

Issuer, audience and token lifetimes have defaults in `appsettings.json` under `Authentication:Jwt`. Integration tests set their own key in `BusinessApiFactory`.

## Demo data

For a ready-made demo, `scripts/seed-demo-business.cs` inserts one demo business and its owner account `owner@demo.local`. It is idempotent (fixed ids, `IF NOT EXISTS`); running it again resets the owner's password and lockout.

The owner password is always passed in (argument or `DEMO_OWNER_PASSWORD`); there is no default password. Apply both migrations first.

```powershell
dotnet run scripts/seed-demo-business.cs -- "<BusinessDatabase connection string>" "<demo owner password, 10+ characters>"
```

## Calling the API

Every endpoint requires `Authorization: Bearer <access token>` except `/api/auth/*` and `/health`. The business comes from the token's `tenant_id` claim. A missing, expired or badly signed token answers `401`.

```http
POST /api/auth/sign-in
Content-Type: application/json

{ "email": "owner@demo.local", "password": "<demo owner password>" }
```

```http
POST /api/clients
Authorization: Bearer <accessToken>
Content-Type: application/json

{ "fullName": "Ana Pérez", "phoneNumber": "11 2233-4455" }
```

| Endpoint | Purpose |
|---|---|
| `POST /api/auth/sign-up` | Create an owner account and their business; `201` with tokens |
| `POST /api/auth/sign-in` | `200` with tokens; `401 auth.invalid_credentials`; `423 auth.locked_out` after 5 failures (15 minutes) |
| `POST /api/auth/refresh` | Exchange a refresh token for new tokens (single use, rotated) |
| `POST /api/auth/sign-out` | Revoke a refresh token; `204`, idempotent |
| `GET /api/business` | Current business settings |
| `POST /api/clients` | Register a client |
| `GET /api/clients/{clientId}` | Get a client |
| `GET /api/clients?search=ana&limit=20` | Search by name or phone prefix |
| `PUT /api/business` | Owner only. Edit name, time zone, currency and calling code; the slug never changes |

Owner only endpoints use the `OwnerOnly` authorization policy (`role` claim `Owner`) and answer `403` to any other role. An id that belongs to another business answers `404`.

`/api/auth/*` is rate limited per client IP (`Authentication:RateLimit`, 10 requests per minute by default; `429` when exceeded). The API honors one `X-Forwarded-For` hop so the limit sees the real client behind the Container Apps ingress.

## Globalization

`Api.csproj` must not enable `InvariantGlobalization`: `Microsoft.Data.SqlClient` refuses to connect in invariant mode ("Globalization Invariant Mode is not supported").
