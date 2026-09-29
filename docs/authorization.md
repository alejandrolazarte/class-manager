# Authorization: organizations, roles and permissions

How the API decides what a signed-in user may do. The full design, including the parts not built yet (coaches collecting only from their own students), is in the [roles and permissions plan](backend/20260928-roles-and-permissions/plan.md).

## Model

- An **organization** is the brand. It groups **businesses** (branches). The tenant is still the business: `tenant_id` in the access token, `TenantId` on every tenant-owned row ([tenancy](tenancy.md)).
- `OrganizationMembers` holds brand-level roles. Today: `BrandOwner`.
- `BusinessMembers` holds the role inside one business (`BranchOwner`, `Coach`, `Viewer`, or `Custom` with a `CustomRoleId`) and, for roles that only see their own classes, the linked `InstructorId`.
- Sign-up creates an organization, its first business, and makes the user `BrandOwner` of the organization and `BranchOwner` of the business.

## System roles

| Role | Can |
|---|---|
| `BrandOwner` | Every permission in every business of the organization, including giving or taking the `BranchOwner` role |
| `BranchOwner` | Everything inside the business except the `BranchOwner` role itself, including custom roles |
| `Coach` | Their own class groups, sessions and private lessons; attendance in them; their students; register clients and students. No money, settings or team |
| `Viewer` | Read everything in the business (money included), change nothing |

A role that has an `.own` permission without the matching `.all` one needs a linked instructor (`member.instructor_required`).

## Custom roles

A branch can have its own roles (`CustomRoles`: name, permission codes, the role it was copied from). They are a snapshot: permissions added to the app later are not added to existing custom roles.

| Operation | Endpoint | Permission |
|---|---|---|
| System and custom roles of the branch, with member counts | `GET /api/roles` | `members.view` |
| Create a role (empty or copied: the app sends the permissions) | `POST /api/roles` `{ name, permissions, copiedFrom }` | `roles.manage` |
| Rename it and set its permissions | `PUT /api/roles/{id}` | `roles.manage` |
| Delete it | `DELETE /api/roles/{id}` | `roles.manage` |

- Names are 2–60 characters, unique per branch, and can't be a system role name (`role.name_reserved`).
- Brand permissions (`Permissions.BrandOnly`) and unknown codes are rejected (`role.permission_not_assignable`).
- **Nobody hands out permissions they don't have**: creating or editing a role, inviting with a role and changing someone's role fail with `403 role.exceeds_own` when the role has a permission the current member lacks.
- Nobody edits the role they hold (`403 role.own_role`).
- A role with an `.own` permission and without the matching `.all` needs a linked coach: editing a role into that shape fails with `409 role.instructor_required` while a member or pending invitation with that role has no coach.
- A role used by members or pending invitations can't be deleted (`409 role.in_use`); deleting it removes its old, used or revoked invitations.
- Invitations and member changes send `role: "Custom"` with `customRoleId`. Team, invitation and `GET /api/me` responses return `customRoleId`; `GET /api/me/branches` returns `customRoleName`.

## Permissions

- The catalog is `Permissions` in `src/Core/Domain/Authorization`. Codes are strings such as `payments.record`.
- Modules tied to a coach's agenda come in pairs: `sessions.view.all` / `sessions.view.own`, `attendance.record.all` / `.own`, and so on. The endpoint accepts either; the use case narrows the result.
- `SystemRolePermissions` maps each system role to its permissions.
- Every endpoint declares its permission with `RequirePermission(...)`, or is anonymous. `GET /api/me` only requires access to the business (`RequireMember()`). The test `When_endpoints_are_mapped/Then_every_endpoint_requires_a_permission_or_is_anonymous` fails the build when an endpoint has neither.

## How a request is checked

1. JWT bearer authentication validates the token (`src/Security`).
2. `PermissionPolicyProvider` turns a `permission:<codes>` policy into a `PermissionRequirement`.
3. `PermissionAuthorizationHandler` asks `ICurrentMember` for the user's access to the business in the token:
   - `BrandOwner` of the business's organization: every permission.
   - Otherwise the permissions of their `BusinessMembers` role: a system role from `SystemRolePermissions`, or the stored permissions of their custom role (brand permissions filtered out).
   - Neither: no access, the request gets `403`.
4. Use cases ask `IAccessScopes` what the member may reach:
   - `ForInstructorsAsync(<all permission>)`: every instructor, or only the member's linked instructor.
   - `ForClientsAsync()`: every client, or only the clients the member registered and the families of students in the member's class groups or private lessons.
   - Reading something outside the scope returns `404`; changing it returns `403` (`member.not_yours`).
   - A class session also belongs to its substitute instructor for that date ([session substitutes](backend/20260929-session-substitutes/plan.md)).
5. Access is read from the database once per request, not from the token, so removing someone or changing a role applies on their next request.

The `role` claim in the token is informational; authorization never reads it.

## Team

| Operation | Endpoint | Permission |
|---|---|---|
| Members and pending invitations | `GET /api/members` | `members.view` |
| Invite by email (role, optional instructor) | `POST /api/members/invitations` | `members.manage` |
| Revoke an invitation | `DELETE /api/members/invitations/{id}` | `members.manage` |
| Change a member's role or instructor | `PUT /api/members/{id}` | `members.manage` |
| Remove a member | `DELETE /api/members/{id}` | `members.manage` |
| Accept an invitation | `POST /api/auth/invitations/accept` | anonymous |

- Giving, changing or taking away `BranchOwner` also needs `branchOwners.manage` (brand owners only).
- Nobody changes or removes their own membership.
- Invitations are single-use links valid 7 days, sent by email (`/accept-invitation?token=`). Only a SHA-256 hash of the token is stored. Inviting the same email again revokes the previous pending invitation.
- Accepting creates the account when the email has none (full name and password required). When the email already has an account, the link is enough: whoever can read that inbox could also reset its password.

## Branches

| Operation | Endpoint | Permission |
|---|---|---|
| Branches the user can open, with the current one | `GET /api/me/branches` | access to the current branch |
| Switch branch (new token pair) | `POST /api/auth/branch` with `refreshToken` and `businessId` | anonymous; the refresh token proves who it is |
| Create a branch in the current brand | `POST /api/organization/branches` | `branches.create` |
| Make a member of this branch brand owner, or undo it | `PUT` / `DELETE /api/members/{id}/brand-owner` | `brandOwners.manage` |

- `branches.create`, `brandOwners.manage` and `branchOwners.manage` belong only to brand owners (`Permissions.BrandOnly`).
- `IBranchDirectory` answers every branch question from the user id: the branches they can open (their `BusinessMembers` rows plus every business of an organization where they are `BrandOwner`), one branch, and the default one. Sign-in, refresh and switching all go through it.
- Refresh tokens store the business they were issued for. Refreshing keeps that business; switching asks for another one and fails with `403` (`branch.unavailable`) when the user has no access. A session whose branch was taken away ends on its next refresh.
- The brand always keeps at least one brand owner (`brand_owner.last`), and nobody changes their own brand owner status.

## In the app

- `MemberProvider` loads `GET /api/me` next to the business; `useCan(...)` hides tabs, rows, buttons and sections the member can't use ([frontend plan](frontend/20260928-roles-and-team/plan.md)).
- Ajustes → Equipo lists members and pending invitations, invites, changes roles and removes members.
- Ajustes → Roles lists the branch's roles and the app's; a system role can be copied into a custom one, and custom roles are edited by module with "No / Solo lo suyo / Todo" for the modules tied to a coach.
- The email link opens `/accept-invitation?token=`, which creates the account if needed and signs in.

## Adding an endpoint

1. Pick an existing permission from `Permissions`, or add one there (and to `Permissions.All`).
2. Add `.RequirePermission(Permissions.<Module>.<Action>)` to the mapping. For coach-scoped modules, pass the `.all` code and then the `.own` code, and apply `IAccessScopes` in the use case.
3. If a new permission should not belong to every system role, update `SystemRolePermissions` and its tests.

## Audit

```powershell
dotnet test tests/ClassManager.Api.I.Tests --filter "FullyQualifiedName~Authorization|FullyQualifiedName~Coaches|FullyQualifiedName~Viewers|FullyQualifiedName~Members|FullyQualifiedName~Roles"
```

```sql
SELECT o.Name, om.UserId, om.Role FROM OrganizationMembers om JOIN Organizations o ON o.Id = om.OrganizationId;
SELECT b.Name, bm.UserId, bm.Role, bm.CustomRoleId, bm.InstructorId FROM BusinessMembers bm JOIN Businesses b ON b.Id = bm.TenantId;
SELECT TenantId, Email, Role, CustomRoleId, ExpiresAt, AcceptedAt, RevokedAt FROM MemberInvitations;
SELECT TenantId, Name, Permissions, CopiedFrom FROM CustomRoles;
```
