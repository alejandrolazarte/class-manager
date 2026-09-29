# Authorization: organizations, roles and permissions

How the API decides what a signed-in user may do. The full design is in the [roles and permissions plan](backend/20260928-roles-and-permissions/plan.md).

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
| `Coach` | Their own class groups, sessions and private lessons; attendance in them; their students; register clients and students; enroll and unenroll the students they reach in their own class groups (`enrollments.manage.own`). No money, settings or team |
| `Viewer` | Read everything in the business (money, products and orders included), change nothing |

A role that has an `.own` permission without the matching `.all` one needs a linked instructor (`member.instructor_required`).

## Families and adult students

Families are a different kind of user from the team, kept apart from everything the team does:

- A **family account** (`ClientAccounts`: `ClientId`, `UserId`) belongs to whoever pays: a parent or an adult who takes the classes. Children never sign in. A user has at most one family per branch (`family.already_linked` otherwise); several users can share a family.
- The branch invites from the family card (`POST /api/clients/{id}/app-invitation`, `students.manage` and the same client scope as the rest of the team; the email defaults to the client's). The link (`/accept-family-invitation?token=`) is single-use, valid 7 days and hashed, like team invitations; inviting again revokes the previous one. Accepting (`POST /api/auth/family-invitations/accept`, anonymous) creates the account when the email has none.
- **The session kind is fixed at sign-in.** Tokens carry a `kind` claim (`team` or `family`; no claim means `team`) and the refresh token stores it, so refreshing or switching branch can never turn a family session into a team one, or the other way. Sign-in opens the team session when the user is a team member and the family session otherwise (a coach who is also a family reaches the family side once a switch exists; not built yet).
- **Two disjoint authorization paths.** Team endpoints (`permission:` policies) reject any token whose kind is not `team`. Family endpoints, all under `/api/family`, use their own `family` policy: the token kind must be `family` and the user must have a `ClientAccounts` row in the token's branch (read from the database on every request, like `ICurrentMember`). Two tests enforce it: every endpoint is anonymous, a permission or `family`, and `family` only appears under `/api/family` and covers everything there.
- Family use cases never reuse team use cases and never take a client id from the request: they read it from `IFamilyAccess`.

| Operation | Endpoint | Who |
|---|---|---|
| Invite a family or adult student | `POST /api/clients/{id}/app-invitation` `{ email? }` | `students.manage` |
| Accept the invitation | `POST /api/auth/family-invitations/accept` | anonymous |
| Home: students with next classes (14 days, cancellations, reschedules and substitutes applied), the month's fee or the class balance | `GET /api/family` | family |
| Shop: active class packs and active products shown in the app, with availability (`Available`, `OnOrder`, `SoldOut`; never the stock count) | `GET /api/family/shop` | family |
| My orders | `GET /api/family/orders` | family |
| Order to pay at the branch (catalog prices only; at most 5 unpaid orders; pickup, or a class one of its students attends) | `POST /api/family/orders` | family |
| Cancel one of my unpaid orders (another family's order is `404`) | `PUT /api/family/orders/{id}/cancellation` | family |

Token responses return `kind` (`team` or `family`) so the app opens the right shell.

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

## Collecting from a coach's own families

The system `Coach` role has no money permissions. A custom role can add them:

- `payments.view.own` (instead of `payments.view.all`) narrows the monthly fees list, a family's payments and the class pack sales total to the families the member reaches: the same client scope as `students.view.own` (clients they registered and families of students in their class groups or private lessons). Reading a family outside it returns `404`.
- With `payments.view.own`, `payments.record` and `classPacks.sell` only work for those families: recording a payment, changing the billing plan or selling a pack for another family returns `403 member.not_yours`.
- Payments and pack sales store `RecordedByUserId`. A family's payments show who recorded each one (`recordedByUserId`, `recordedByFullName`).
- Without `payments.view.all`, a member only deletes payments and pack sales they recorded themselves.
- A family's class balance (`GET /api/clients/{id}/class-balance`) follows the student scope.

## Products and orders

| Operation | Endpoint | Permission |
|---|---|---|
| Products with sizes and stock | `GET /api/products` | `products.view` or `orders.manage` (the counter sale needs the catalog) |
| Create, edit, deactivate | `POST /api/products`, `PUT /api/products/{id}`, `PUT /api/products/{id}/active` | `products.manage` |
| Load or adjust stock, history | `POST /api/products/{id}/stock`, `GET /api/products/{id}/stock-movements` | `products.manage` / `products.view` |
| Orders, newest first (`?awaitingPickup=true`, `?clientId=`) | `GET /api/orders` | `orders.view.all` or `orders.view.own` |
| Counter sale, mark delivered, refund | `POST /api/orders`, `PUT /api/orders/{id}/delivered`, `POST /api/orders/{id}/refunds` | `orders.manage` |
| Confirm the payment of a family's order (`?requested=true` lists them), or cancel it | `PUT /api/orders/{id}/payment` `{ method, paidOn?, isReady }`, `PUT /api/orders/{id}/cancellation` | `orders.manage` |
| Mark ready (emails the family), choose pickup or a class, the family's classes | `PUT /api/orders/{id}/ready`, `PUT /api/orders/{id}/delivery`, `GET /api/orders/delivery-classes?clientId=` | `orders.manage` |
| What to hand over in a class, hand it over | `GET /api/class-groups/{id}/deliveries`, `PUT /api/class-groups/{id}/deliveries/{orderId}` | `attendance.record.all` or `attendance.record.own` (own class groups only, like attendance) |

- `orders.view.own` narrows orders to the families the member reaches (the student client scope); orders without a family are only visible with `orders.view.all`. With it, selling to another family returns `403 member.not_yours`, and delivering or refunding its orders returns `404`.
- The system `Coach` role has no shop permissions; `Viewer` gets `products.view` and `orders.view.all`.

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
| Resend an invitation (new link, 7 more days) | `POST /api/members/invitations/{id}/resend` | `members.manage` |
| Change a member's role or instructor | `PUT /api/members/{id}` | `members.manage` |
| Remove a member | `DELETE /api/members/{id}` | `members.manage` |
| Accept an invitation | `POST /api/auth/invitations/accept` | anonymous |

- Giving, changing or taking away `BranchOwner` also needs `branchOwners.manage` (brand owners only).
- Nobody changes or removes their own membership.
- Invitations are single-use links valid 7 days, sent by email (`/accept-invitation?token=`). Only a SHA-256 hash of the token is stored. Inviting the same email again revokes the previous pending invitation. Resending an invitation that was not accepted or revoked (expired ones too) replaces its token, so the previous link stops working, and emails the new one.
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
SELECT TenantId, ClientId, Amount, Month, RecordedByUserId FROM Payments ORDER BY CreatedAt DESC;
SELECT TenantId, ClientId, UserId FROM ClientAccounts;
SELECT TenantId, ClientId, Email, ExpiresAt, AcceptedAt, RevokedAt FROM ClientInvitations;
```
