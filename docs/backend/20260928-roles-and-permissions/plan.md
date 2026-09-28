# Backend plan — Organizations, branches, roles and permissions

See [pilot plan](../../pilot-df-swimming.md), Phase 2. DF Swimming Team is a brand with branches ("sedes"): the owner teaches in Tenerife, his sister runs Valencia with her own students and her own money, a partner runs Barcelona with students the owner doesn't know. Each branch needs its own coaches, and the brand owner decides who runs or sees each branch.

## Goal

- An **organization** (the brand) groups **businesses** (the branches). Each business keeps its own students, money, settings and time zone.
- Every endpoint is protected by a **permission**, never by a role name.
- The app ships **system roles** that nobody can edit, and a business can **copy** one into a **custom role** and change its permissions.
- The brand owner enters any branch, creates coaches and branch owners there, gives a partner access to one specific branch, or promotes someone to brand owner.

## Decisions

### Organizations and branches

- **The tenant stays the `Business`.** A branch is a business, so tenant isolation (`TenantId`, global query filters) doesn't change. The `Organization` sits above: it owns no students and no payments.
- **Every business belongs to an organization.** Sign-up creates an organization and its first business, and the user becomes brand owner of both. A solo instructor never sees the word "organization": the app shows brand screens only when there is more than one branch or more than one brand member. One model, no special case for franchises. The migration creates one organization per existing business.
- **Two levels of membership:**
  - `OrganizationMembers`: brand-level role. Today only `BrandOwner`.
  - `BusinessMembers`: role inside one branch (`BranchOwner`, `Coach`, `Viewer` or a custom role).
- **A brand owner can act in every branch of the organization** with full permissions, without a `BusinessMembers` row: that's how he creates coaches and branch owners in a new branch. A person can also be a plain member of one branch (the partner who may only see Barcelona).
- **Choosing a branch.** The access token carries one `tenant_id`, the current branch. `POST /api/authentication/branch` issues new tokens for another branch the user can access, and the refresh token remembers the branch it was issued for. `GET /api/me/branches` lists them for the switcher.

### Permissions and roles

- **Permissions live in code, roles live in data.** The catalog is a constants class in `Core` (`Permissions`). A role is a named set of permission codes stored as strings (`"payments.record"`), so adding a permission needs no migration.
- **One permission per action group of a module, not per use case.** Each module has `view` and one or a few actions. Every endpoint declares exactly one permission. A permission can be split later with a data migration that gives the new code to every role that had the old one.
- **Scope "own" vs "all"** for the modules tied to a coach (class groups, sessions and attendance, private lessons, students, payments). "Own" means the member's linked instructor teaches it. It's a permission, not a special role, so a custom role can mix both.
- **System roles are defined in code**: they can't be edited or deleted, and they get new permissions with each release.
- **Custom roles are rows of one business** (`Roles`, `RolePermissions`). "Copy" creates a custom role prefilled with the permissions of any role. A custom role is a snapshot: permissions shipped later are shown as new in the role editor, not added by themselves.
- **A branch member has exactly one role** in that branch: a system role or a custom role of the same business.
- **Permissions are resolved on the server on every request**, with a short in-memory cache invalidated when a role or member changes, not stored in the JWT. Removing someone takes effect at once instead of when the 15-minute access token expires.
- **The app asks what it can show.** `GET /api/me` returns the current branch, the role and the permission codes; the app hides tabs and buttons from it. The server stays the only enforcement.
- **Invitations reuse the password reset mechanism**: a single-use link by email, valid 7 days. Accepting creates the Identity user if needed and the membership.

## System roles

| Level | Role | Can |
|---|---|---|
| Brand | `BrandOwner` | Everything in every branch, plus: create branches, invite and remove brand owners, give anyone a role in any branch. The organization always keeps at least one |
| Branch | `BranchOwner` | Everything inside the branch: students, classes, money, settings, invite coaches and viewers, custom roles. Can't touch other branches, can't make anyone `BranchOwner` or `BrandOwner`, can't delete the branch |
| Branch | `Coach` | Their own agenda and attendance, register clients and students, sell packs and record payments, see payments they recorded or of their own students. Doesn't see the rest of the branch's money, settings or members |
| Branch | `Viewer` | Read everything in the branch, change nothing. For a partner who follows a branch |

How the pilot maps to it:

| Person | Membership |
|---|---|
| DF owner | `BrandOwner` of DF Swimming Team; teaches in Tenerife through that role, linked to his instructor there |
| His sister | `BranchOwner` of DF Valencia. If he wants her to run everything, he makes her `BrandOwner` too |
| The Barcelona partner | `BranchOwner` of DF Barcelona |
| Someone who should only follow Barcelona | `Viewer` of DF Barcelona |
| A coach hired in Valencia | `Coach` of DF Valencia, linked to their instructor |

## Permission catalog (first version)

| Module | Permissions |
|---|---|
| Business | `business.view`, `business.manage` (settings, default fee) |
| Members and roles | `members.view`, `members.manage` (invite coaches and viewers, change role, remove), `roles.manage` |
| Instructors | `instructors.view`, `instructors.manage` |
| Class groups | `classGroups.view.own`, `classGroups.view.all`, `classGroups.manage` |
| Enrollments | `enrollments.view`, `enrollments.manage` |
| Sessions and attendance | `sessions.view.own`, `sessions.view.all`, `attendance.record.own`, `attendance.record.all`, `sessions.manage` (cancel, reschedule) |
| Private lessons | `privateLessons.view.own`, `privateLessons.view.all`, `privateLessons.manage.own`, `privateLessons.manage.all` |
| Clients and students | `students.view.own`, `students.view.all`, `students.manage` (register client, add student) |
| Fees and payments | `payments.view.own`, `payments.view.all`, `payments.record` (record, delete own, billing plan) |
| Class packs | `classPacks.view`, `classPacks.manage` (catalog), `classPacks.sell` (sell, delete own sale) |
| Import and export | `importExport.run` |

Brand-level permissions (`organization.manage`, `branches.create`, `brandOwners.manage`, `branchOwners.manage`) belong only to `BrandOwner` and are not assignable to custom roles.

`*.own` for students and payments: students enrolled in a group the member teaches or in one of their private lessons, or registered by the member; payments recorded by the member or of those students.

## Data

- `Organizations` (`Id`, `Name`).
- `Businesses` gains `OrganizationId` (required).
- `OrganizationMembers` (`OrganizationId`, `UserId`, `Role`), unique per organization and user.
- `BusinessMembers` gains `SystemRole` (nullable, string), `RoleId` (nullable, FK to `Roles`), `InstructorId` (nullable, unique per business). A check constraint requires exactly one of `SystemRole` and `RoleId`. The existing `Role` column (`Owner`) becomes an `OrganizationMembers` row with `BrandOwner`, and the business member row is removed or kept as `BranchOwner`.
- `Roles` (`TenantId`, `Id`, `Name` unique per business, `CopiedFrom` nullable).
- `RolePermissions` (`TenantId`, `RoleId`, `Permission`), unique per role and permission. Unknown codes and brand-level codes are rejected.
- `Invitations` (`Id`, `OrganizationId`, `TenantId` nullable for brand invitations, `Email`, role, `InstructorId`, `TokenHash`, `ExpiresAt`, `AcceptedAt`, `RevokedAt`).
- Payments and pack sales gain `RecordedByMemberId`, for the `payments.view.own` scope.
- The Identity role tables stay unused: roles depend on the branch, and Identity roles are global per user.

## Enforcement

- `RequirePermission(Permissions.Payments.Record)` on each endpoint, backed by an `IAuthorizationPolicyProvider` that turns `permission:<code>` policies into a `PermissionRequirement`. `AuthorizationPolicies.OwnerOnly` disappears.
- `IPermissionResolver`: for the user and the current branch, the permissions of their branch role; if they are `BrandOwner` of the branch's organization, all permissions. No membership and not brand owner means `401`, even with a valid token.
- "Own" scopes are applied in the use cases through `ICurrentMember` in `Core` (member id, instructor id, `HasPermission(code)`), never by the endpoint alone.
- Brand endpoints check `OrganizationMembers`, not the current branch. Listing branches reads `Businesses` by `OrganizationId`, never branch data.
- A test enumerates every endpoint and fails if one has neither a permission nor `AllowAnonymous`.

## Contract

| Operation | Endpoint | Permission |
|---|---|---|
| Current branch, role and permissions | `GET /api/me` | authenticated |
| Branches the user can access | `GET /api/me/branches` | authenticated |
| Switch branch (new token pair) | `POST /api/authentication/branch` | authenticated, access to that branch |
| Create branch (name, time zone, currency, optional branch owner email) | `POST /api/organization/branches` | `branches.create` |
| List brand owners / invite / remove | `GET` / `POST` / `DELETE /api/organization/members` | `brandOwners.manage` |
| Permission catalog, grouped by module | `GET /api/permissions` | `roles.manage` |
| List roles of the branch (system and custom) | `GET /api/roles` | `members.view` |
| Create custom role (empty or copied) | `POST /api/roles` | `roles.manage` |
| Rename and set permissions | `PUT /api/roles/{id}` | `roles.manage` |
| Delete custom role (unused only) | `DELETE /api/roles/{id}` | `roles.manage` |
| List branch members and pending invitations | `GET /api/members` | `members.view` |
| Invite to the branch (email, role, optional instructor) | `POST /api/members/invitations` | `members.manage`; `BranchOwner` role needs `branchOwners.manage` |
| Resend / revoke invitation | `POST` / `DELETE /api/members/invitations/{id}` | `members.manage` |
| Accept invitation | `POST /api/authentication/invitations/accept` | anonymous |
| Change a member's role or linked instructor | `PUT /api/members/{id}` | `members.manage`; to or from `BranchOwner` needs `branchOwners.manage` |
| Remove a member | `DELETE /api/members/{id}` | `members.manage`; a `BranchOwner` needs `branchOwners.manage` |

## Business rules

- An organization always keeps at least one `BrandOwner`.
- Nobody changes their own role.
- A member with any `*.own` permission must be linked to an instructor of that branch (`member.instructorRequired`). An instructor is linked to at most one member.
- A custom role name is 2–60 characters, unique per branch; system role names are reserved.
- Inviting an email that already has access to the branch fails with `member.alreadyMember`.
- Removing a member, or a brand owner, revokes their refresh tokens; their next request is rejected.
- A branch's time zone and currency are its own; the Canary Islands branch uses `Atlantic/Canary`.

## Delivery

1. **Permissions without behavior change.** Catalog, `RequirePermission` on every endpoint, `ICurrentMember`, `IPermissionResolver`, `GET /api/me`, the "every endpoint has a permission" test. `Organizations`, `OrganizationMembers` and the migration that makes every current owner `BrandOwner` of their own organization. Owners keep full access.
2. **Branch members.** `BranchOwner`, `Coach` and `Viewer`, `InstructorId`, invitations, "own" scopes, `RecordedByMemberId`.
3. **Branches.** Create a branch, `GET /api/me/branches`, switch branch, brand owners management. After this, DF runs Tenerife, Valencia and Barcelona under one brand.
4. **Custom roles.** `Roles`, `RolePermissions`, copy, role editor.
5. **Later, when asked:** shared course catalog from the brand, brand reports with totals only (no personal data), public brand page that routes leads to branches.

## Tests (write first)

```text
When_endpoints_are_mapped/Then_every_endpoint_requires_a_permission_or_is_anonymous
When_member_lacks_permission/Then_request_is_forbidden
When_brand_owner_switches_to_a_branch/Then_they_have_every_permission_there
When_brand_owner_switches_to_a_branch_of_another_organization/Then_request_is_forbidden
When_branch_owner_invites_a_branch_owner/Then_request_is_forbidden
When_branch_owner_reads_another_branch/Then_request_is_forbidden
When_viewer_records_a_payment/Then_request_is_forbidden
When_coach_lists_day_sessions/Then_only_their_sessions_are_returned
When_coach_records_attendance_in_another_coach_session/Then_request_is_forbidden
When_coach_lists_payments/Then_only_their_payments_are_returned
When_last_brand_owner_is_removed/Then_request_fails
When_member_is_removed/Then_next_request_is_unauthorized
When_invitation_is_accepted_twice/Then_second_attempt_fails
When_system_role_is_copied/Then_custom_role_has_its_permissions
When_system_role_is_edited/Then_request_fails
When_custom_role_has_a_brand_permission/Then_request_fails
When_business_reads_roles/Then_other_business_roles_are_not_visible   (tenant isolation)
When_member_is_given_a_role_of_another_business/Then_request_fails    (tenant isolation)
```

## App (frontend plan to follow)

- Session loads `GET /api/me`; a `useCan(permission)` hook hides tabs, buttons and screens.
- Branch switcher in the header when the user has more than one branch.
- Ajustes → Equipo: members, pending invitations, invite form (email, role, coach).
- Ajustes → Marca (brand owners only, when relevant): branches, create branch, brand owners.
- Ajustes → Roles: system roles read-only with "Copiar", custom roles editable, permissions grouped by module with "Solo lo suyo / Todo" for scoped modules.
- Accept invitation screen from the email link.

## Open decisions

| Decision | Proposal |
|---|---|
| A branch that the brand owner must not see (an independent partner) | Not supported: a brand owner sees every branch of the brand. A fully independent partner uses their own organization |
| Family accounts (`Client` role, linked to `ClientId`) | Out of scope; `BusinessMembers` leaves room for it |
