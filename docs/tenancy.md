# Tenancy library

Date: 2026-09-24

`src/Tenancy` and `src/Tenancy.AspNetCore` hold the multi-tenancy mechanism with no business concepts, so every app built from this template reuses it as is. Same approach as the [security library](security.md).

## Projects

| Project | Depends on | Contents |
|---|---|---|
| `Tenancy` | nothing | `ITenantOwned`, `ITenantContext`, `ITenantScope`, `TenantNotResolvedException`, `TenantClaimTypes` (`tenant_id`) |
| `Tenancy.AspNetCore` | `Tenancy`, EF Core, ASP.NET Core | `ApplyTenantQueryFilters` (global query filter named `Tenant` for every `ITenantOwned` entity), `IgnoreTenantFilter()` (reads across tenants, keeps every other filter), `TenantStampingSaveChangesInterceptor`, `ClaimsTenantContext` (reads the `tenant_id` claim), `AddClaimsTenancy`, `AddTenantStamping` |

`Core` references only `Tenancy`, so the domain stays free of EF Core and ASP.NET (ARCH001). `Tenancy.AspNetCore` is referenced by `Infrastructure`.

## How the application plugs in

```text
Core            entities implement ITenantOwned (Guid TenantId); use cases use ITenantContext / ITenantScope
  ▲
Infrastructure  AppDbContext : ITenantDbContext → modelBuilder.ApplyTenantQueryFilters(this)
  │             AddTenantStamping() + AddInterceptors(TenantStampingSaveChangesInterceptor)
  └──────────► Tenancy.AspNetCore ──► Tenancy
Api             AddClaimsTenancy()  (ITenantContext and ITenantScope from the access token)
```

- The tenant is the `Business`. Every tenant-owned entity has a `TenantId` column that points to `Businesses.Id`.
- A business belongs to an `Organization` (the brand). Organizations and their members are not tenant-owned; they only decide who can access which business ([authorization](authorization.md)).
- `ITenantDbContext.CurrentTenantId` must be a property of the `DbContext` itself: EF Core evaluates it per query, so each request sees its own tenant.
- Security issues the token with its `TenantClaimType`; the application sets it to `TenantClaimTypes.TenantId`, so both libraries agree on `tenant_id` without depending on each other.
- Business-specific rules stay in `Core`, for example `BusinessErrorCodes.CurrentBusinessNotFound`.

## Rules

- ARCH004: `Tenancy` and `Tenancy.*` cannot use `ClassManager.Core`, `ClassManager.Infrastructure`, `ClassManager.Api` or `ClassManager.Security` (see [analyzers.md](analyzers.md)).
- The tenant filter is named (`TenantModelBuilderExtensions.TenantQueryFilter`) so it can live next to other named filters, such as the soft delete filter ([deleting data](deleting-data.md)). An unnamed filter added later would replace it, so every global filter must be named.
- A query that must read across tenants uses `IgnoreTenantFilter()`. Today: `BranchDirectory` and `StudentAppDirectory` (the branches and student accounts of a user at sign-in), `ExpiredOrderDirectory` (background job over every business), the invitation repositories (an invitation code is accepted before entering the business) and `EmailBrandReader` (emails sent outside a request). It turns off only the tenant filter, so soft deleted rows stay hidden.
- `IgnoreQueryFilters()` stays forbidden outside `Infrastructure` and `Tenancy.AspNetCore` (ARCH002), and calling it without filter names is forbidden everywhere (ARCH010): it would also turn off the soft delete filter.
- Every new tenant-owned entity still needs a test proving that business A cannot read business B's data.

## Extracting to packages

Once two apps built from this template are in production, move `Tenancy`, `Tenancy.AspNetCore`, `Security` and the analyzers to NuGet packages so a fix ships once. Until then each app keeps its own copy.
