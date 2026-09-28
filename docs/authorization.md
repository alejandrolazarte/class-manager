# Authorization: organizations, roles and permissions

How the API decides what a signed-in user may do. The full design, including the parts not built yet (coaches, branches, custom roles), is in the [roles and permissions plan](backend/20260928-roles-and-permissions/plan.md).

## Model

- An **organization** is the brand. It groups **businesses** (branches). The tenant is still the business: `tenant_id` in the access token, `TenantId` on every tenant-owned row ([tenancy](tenancy.md)).
- `OrganizationMembers` holds brand-level roles. Today: `BrandOwner`.
- `BusinessMembers` holds the role inside one business. Today: `BranchOwner`.
- Sign-up creates an organization, its first business, and makes the user `BrandOwner` of the organization and `BranchOwner` of the business.

## Permissions

- The catalog is `Permissions` in `src/Core/Domain/Authorization`. Codes are strings such as `payments.record`; modules tied to a coach's agenda use a `.all` suffix, so a `.own` variant can be added later.
- `SystemRolePermissions` maps each system role to its permissions. `BrandOwner` and `BranchOwner` have every permission.
- Every endpoint declares its permission with `RequirePermission(...)`, or is anonymous. `GET /api/me` only requires access to the business (`RequireMember()`). The test `When_endpoints_are_mapped/Then_every_endpoint_requires_a_permission_or_is_anonymous` fails the build when an endpoint has neither.

## How a request is checked

1. JWT bearer authentication validates the token (`src/Security`).
2. `PermissionPolicyProvider` turns a `permission:<codes>` policy into a `PermissionRequirement`.
3. `PermissionAuthorizationHandler` asks `ICurrentMember` for the user's access to the business in the token:
   - `BrandOwner` of the business's organization: every permission.
   - Otherwise the permissions of their `BusinessMembers` role.
   - Neither: no access, the request gets `403`.
4. Access is read from the database once per request, not from the token, so removing someone or changing a role applies on their next request.

The `role` claim in the token is informational; authorization never reads it.

## Adding an endpoint

1. Pick an existing permission from `Permissions`, or add one there (and to `Permissions.All`).
2. Add `.RequirePermission(Permissions.<Module>.<Action>)` to the mapping.
3. If a new permission should not belong to every system role, update `SystemRolePermissions` and its tests.

## Audit

```powershell
dotnet test tests/ClassManager.Api.I.Tests --filter "FullyQualifiedName~Authorization"
```

```sql
SELECT o.Name, om.UserId, om.Role FROM OrganizationMembers om JOIN Organizations o ON o.Id = om.OrganizationId;
SELECT b.Name, bm.UserId, bm.Role FROM BusinessMembers bm JOIN Businesses b ON b.Id = bm.TenantId;
```
