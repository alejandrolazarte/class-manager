# Domain model

Entities, invariants and tenancy rules. Everything here lives in `src/Core/Domain` and has no dependency on EF Core or ASP.NET.

## Principles

- **Entities protect their own invariants.** They are created through a static `Create(...)` factory that returns `Result<TEntity>`; properties have private setters. An invalid entity cannot exist in memory.
- **Identifiers are `Guid` version 7** (`Guid.CreateVersion7()`): sortable by creation time and index-friendly in SQL Server.
- **Time is stored as UTC `DateTimeOffset`.** Conversions to local time use the business's `TimeZoneId`. Current time always comes from `TimeProvider`, never `DateTimeOffset.UtcNow`, so tests control the clock.
- **Money is `decimal`**, in the business's currency, with 2 decimal places.
- **Validation limits are constants** in the entity (for example `Client.FullNameMaxLength`), reused by EF Core configuration and by use case validation.

## Entities

```mermaid
erDiagram
    Business ||--o{ BusinessMember : "is run by"
    Business ||--o{ Client : has
```

### Business (tenant root)

| Property | Type | Rules |
|---|---|---|
| `Id` | `Guid` | v7 |
| `Name` | `string` | Required, 2–120 characters |
| `Slug` | `string` | Required, unique globally, lowercase, 3–80 characters |
| `TimeZoneId` | `string` | IANA id, for example `America/Argentina/Buenos_Aires` |
| `DefaultCountryCallingCode` | `string` | 1–3 digits, for example `54`; used to normalize local phone numbers |
| `CurrencyCode` | `string` | ISO 4217, 3 letters, for example `ARS`; normalized to upper case |
| `CreatedAt` | `DateTimeOffset` | UTC |

`Business` is the only entity that does **not** implement `ITenantOwned`.

### BusinessMember

Links a user account to a business with a role. For now every business has exactly one member, the owner.

| Property | Type | Rules |
|---|---|---|
| `Id` | `Guid` | v7 |
| `TenantId` | `Guid` | Tenant (the business) |
| `UserId` | `Guid` | Identity user id (user accounts live in `SecurityDbContext`, schema `identity`) |
| `Role` | `BusinessRole` | `Owner` for now; add roles as the app needs them |

Unique `(TenantId, UserId)`. The access token carries the member's `TenantId` as the `tenant_id` claim and the role as `role`.

### Client

| Property | Type | Rules |
|---|---|---|
| `Id` | `Guid` | v7 |
| `TenantId` | `Guid` | Tenant (the business) |
| `FullName` | `string` | Required, trimmed, 2–120 characters |
| `PhoneNumber` | `PhoneNumber` | Required, normalized, **unique per business** |
| `Email` | `string?` | Optional, valid format, max 254 characters |
| `Notes` | `string?` | Optional, max 1000 characters  |
| `CreatedAt` | `DateTimeOffset` | UTC |

## Value objects

### PhoneNumber

- Created with `PhoneNumber.Create(string? rawValue, string defaultCountryCode)` returning `Result<PhoneNumber>`.
- Strips spaces, dashes, dots and parentheses; keeps an explicit `+` or `00` international prefix; otherwise drops the leading trunk `0` and adds the business's `DefaultCountryCallingCode`; stores E.164 (`11 2233-4455` → `+541122334455`).
- No country-specific rules: the Argentine mobile `9` is not inserted. Numbers entered with it (`+54 9 11 ...`) are kept as entered.
- Two phone numbers are equal when their normalized values are equal. That is what makes the uniqueness rule reliable.
- Persisted as a single `nvarchar(20)` column via an EF Core value converter.

## Multi-tenancy

One database for all businesses. Isolation is enforced centrally, never by remembering to add a `Where`.

```text
Tenancy/                    (no dependencies, referenced by Core)
  ITenantOwned.cs           → Guid TenantId { get; }
  ITenantContext.cs         → Guid TenantId { get; }  (the current request's business)

Tenancy.AspNetCore/
  Persistence/TenantModelBuilderExtensions.cs        → global query filter for every ITenantOwned entity
  Persistence/TenantStampingSaveChangesInterceptor.cs → sets TenantId on Added entities;
                                                       throws if a Modified entity belongs to another business
  Claims/ClaimsTenantContext.cs                      → reads the tenant_id claim
```

The mechanism has no business concepts so other apps can reuse it; see [tenancy.md](../tenancy.md).

Rules:

- Every tenant-owned table has a `TenantId` column (the business) and an index that **starts** with `TenantId` (for example `IX_Clients_TenantId_PhoneNumber`, unique).
- Every unique rule is scoped per business, for example `(TenantId, PhoneNumber)`.
- `IgnoreQueryFilters()` is forbidden outside `Infrastructure`. Enforced by the Roslyn analyzer ARCH002 (see [analyzers.md](../analyzers.md)).
- Every new tenant-owned entity needs an integration test in `When_<entity>_belongs_to_another_business/Then_it_is_not_returned.cs`.
- `ITenantContext` is `ClaimsTenantContext`: it reads the `tenant_id` claim of the signed-in user in every environment. HTTP tests authenticate with a real token issued for a test owner; `FixedTenantContext` is used for direct database access.
- Sign up creates a business before any token exists, so the use case calls `ITenantScope.Establish(tenantId)` for the new business; the stamping interceptor then accepts the owner's `BusinessMember`. A request can only establish one business.
- Sign in and refresh look up `BusinessMember` by user id with `IgnoreQueryFilters()` inside `Infrastructure` (`BusinessMemberRepository`), because there is no tenant yet.
- A missing or invalid tenant surfaces as `TenantNotResolvedException`, which the global exception handler maps to `401 ProblemDetails`.

## Persistence abstractions

`Core` defines what it needs; `Infrastructure` implements it with EF Core. One interface per aggregate, containing only the methods the use cases need (interface segregation), and no generic repository.

```text
Core/Abstractions/Persistence/
  IClientRepository.cs
  IBusinessRepository.cs
  IBusinessMemberRepository.cs
  IUnitOfWork.cs         → Task SaveChangesAsync(CancellationToken)
```
