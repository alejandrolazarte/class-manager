# Backend plan — Roles, permissions and coach accounts

See [pilot plan](../../pilot-df-swimming.md), Phase 2. Coaches need their own accounts, and later each business wants to decide what each person in the app can do.

## Goal

- Every endpoint is protected by a **permission**, never by a role name.
- The app ships **system roles** (Owner, Coach) that nobody can edit, but a business can **copy** one into a **custom role** and change its permissions.
- A coach is a `BusinessMember` linked to their `Instructor`, and sees only their own agenda unless their role says otherwise.

## Decisions

- **Permissions live in code, roles live in data.** Code checks permissions, so a business can't invent one: the catalog is a constants class in `Core` (`Permissions`). A role is a named set of permission codes. The database stores codes as strings (`"payments.record"`), so adding a permission is a code change with no migration.
- **Granularity: one permission per action group of a module, not per use case.** About 60 use cases would make a role editor nobody understands. Each module has `view` and one or a few actions (`manage`, `record`, `sell`...). Every endpoint declares exactly one permission. When a use case later needs its own permission, it's split out without touching roles in the database, because a data migration can add the new code to every role that had the old one.
- **Scope "own" vs "all"** for the modules tied to a coach's agenda (class groups, sessions and attendance, private lessons, students). A member linked to an instructor with `sessions.view.own` sees only sessions they teach; `sessions.view.all` sees every session. "Own" is a permission, not a special role, so a custom role can mix both.
- **System roles are defined in code**, not in the database. They can't be edited or deleted, and they pick up new permissions on the next release automatically. `Owner` always has every permission, including roles and members, and a business always keeps at least one owner.
- **Custom roles are tenant-owned rows** (`Roles`, `RolePermissions`). "Copy" creates a custom role prefilled with the permissions of the source role, system or custom. A custom role is a snapshot: new permissions shipped later are not added to it; the role editor shows them as new so the owner can decide.
- **A member has exactly one role**: either a system role or a custom role of the same business.
- **Permissions are resolved on the server on every request, not stored in the JWT.** The token keeps `tenant_id`, `sub` and the member id. An `IPermissionResolver` loads the member's permissions with a short in-memory cache, invalidated when a role or member changes. Removing a coach or a permission takes effect at once instead of when the 15-minute access token expires, and the token stays small.
- **The app asks what it can show.** `GET /api/me` returns the member, role and permission codes; the app hides tabs and buttons from it. The server stays the only enforcement.
- **Invitations reuse the password reset mechanism**: a single-use link by email, valid 7 days. Accepting it creates the Identity user if needed and the `BusinessMember`.

## Permission catalog (first version)

| Module | Permissions |
|---|---|
| Business | `business.view`, `business.manage` (settings, default fee) |
| Members and roles | `members.view`, `members.manage` (invite, change role, remove), `roles.manage` |
| Instructors | `instructors.view`, `instructors.manage` |
| Class groups | `classGroups.view.own`, `classGroups.view.all`, `classGroups.manage` |
| Enrollments | `enrollments.view`, `enrollments.manage` |
| Sessions and attendance | `sessions.view.own`, `sessions.view.all`, `attendance.record.own`, `attendance.record.all`, `sessions.manage` (cancel, reschedule) |
| Private lessons | `privateLessons.view.own`, `privateLessons.view.all`, `privateLessons.manage.own`, `privateLessons.manage.all` |
| Clients and students | `students.view.own`, `students.view.all`, `students.manage` (register client, add student) |
| Fees and payments | `payments.view` (fees list, client payments), `payments.record` (record, delete, billing plan) |
| Class packs | `classPacks.view`, `classPacks.manage` (catalog), `classPacks.sell` (sell, delete purchase, balance) |
| Import and export | `importExport.run` |

`*.view.own` means: the member's linked instructor teaches it (students: enrolled in a group they teach or in one of their private lessons).

## System roles

| Role | Permissions |
|---|---|
| `Owner` | All, always |
| `Coach` | `business.view`, `instructors.view`, `classGroups.view.own`, `enrollments.view`, `sessions.view.own`, `attendance.record.own`, `privateLessons.view.own`, `privateLessons.manage.own`, `students.view.own` |

A "front desk" role (everything except members, roles and business settings) is easy to add later as a third system role if pilots ask for it; until then an owner can build it as a custom role.

## Data

- `BusinessMembers` gains `SystemRole` (nullable, string), `RoleId` (nullable, FK to `Roles`), `InstructorId` (nullable, FK to `Instructors`, unique per business). A check constraint requires exactly one of `SystemRole` and `RoleId`. The existing `Role` column migrates to `SystemRole` (`Owner`).
- `Roles` (`TenantId`, `Id`, `Name` unique per business, `CopiedFrom` nullable for reference).
- `RolePermissions` (`TenantId`, `RoleId`, `Permission` string), unique per role and permission. Unknown codes are rejected when saving a role.
- `MemberInvitations` (`TenantId`, `Id`, `Email`, `SystemRole` or `RoleId`, `InstructorId`, `TokenHash`, `ExpiresAt`, `AcceptedAt`, `RevokedAt`).
- The Identity role tables (`AspNetRoles`, `AspNetUserRoles`) stay unused: roles depend on the business, and Identity roles are global per user.

## Enforcement

- `RequirePermission(Permissions.Payments.Record)` on each endpoint, backed by an `IAuthorizationPolicyProvider` that turns `permission:<code>` policies into a `PermissionRequirement`. `AuthorizationPolicies.OwnerOnly` disappears.
- "Own" scopes are applied in the use cases through an `ICurrentMember` abstraction in `Core` (member id, instructor id, `HasPermission(code)`), never by the endpoint alone: a coach asking for `GET /api/sessions?date=` gets only their sessions.
- A test enumerates every endpoint in the API and fails if one has neither a permission nor `AllowAnonymous`. New endpoints can't forget it.
- Mechanics that are not business concepts (permission requirement, policy provider) could move to `Security` later, if salon-manager needs them; for now they live in `Api` and `Infrastructure`.

## Contract

| Operation | Endpoint | Permission |
|---|---|---|
| Current member, role and permissions | `GET /api/me` | authenticated |
| Permission catalog (grouped by module, for the role editor) | `GET /api/permissions` | `roles.manage` |
| List roles (system and custom) | `GET /api/roles` | `members.view` |
| Create custom role (empty or copied from `copyFromRoleId` / `copyFromSystemRole`) | `POST /api/roles` | `roles.manage` |
| Rename and set permissions | `PUT /api/roles/{id}` | `roles.manage` |
| Delete custom role (only if no member or pending invitation uses it) | `DELETE /api/roles/{id}` | `roles.manage` |
| List members and pending invitations | `GET /api/members` | `members.view` |
| Invite (email, role, optional instructor) | `POST /api/members/invitations` | `members.manage` |
| Resend / revoke invitation | `POST` / `DELETE /api/members/invitations/{id}` | `members.manage` |
| Accept invitation (token, password if new user) | `POST /api/authentication/invitations/accept` | anonymous |
| Change a member's role or linked instructor | `PUT /api/members/{id}` | `members.manage` |
| Remove a member | `DELETE /api/members/{id}` | `members.manage` |

## Business rules

- A business always has at least one member with `SystemRole = Owner`: the last owner can't be removed or given another role.
- A member can't change their own role.
- A member with any `*.own` permission must be linked to an instructor; otherwise saving the role or member fails with `member.instructorRequired`.
- An instructor is linked to at most one member.
- A custom role name is 2–60 characters and unique per business; system role names are reserved.
- An invitation to an email that is already a member of the business fails with `member.alreadyMember`.
- Removing a member revokes their refresh tokens; the next request with their access token is rejected because the permission resolver finds no member.

## Delivery

1. **Permissions without behavior change.** Catalog, `RequirePermission` on every endpoint, `ICurrentMember`, `GET /api/me`, the "every endpoint has a permission" test, and `Role` → `SystemRole`. Owners keep full access. The GET endpoints that today are open to any member (payments, fees, class balance) get their permission.
2. **Coach accounts.** `Coach` system role, `InstructorId`, invitations and accepting them, "own" scopes in sessions, class groups, private lessons and students. This is what the pilot needs.
3. **Custom roles.** `Roles`, `RolePermissions`, copy, the role editor endpoints. Built when a pilot asks for it; nothing in steps 1–2 has to change for it.

## Tests (write first)

```text
When_endpoints_are_mapped/Then_every_endpoint_requires_a_permission_or_is_anonymous
When_member_lacks_permission/Then_request_is_forbidden
When_coach_lists_day_sessions/Then_only_their_sessions_are_returned
When_coach_records_attendance_in_another_coach_session/Then_request_is_forbidden
When_coach_lists_students/Then_only_students_of_their_classes_are_returned
When_coach_requests_payments/Then_request_is_forbidden
When_last_owner_is_removed/Then_request_fails
When_member_is_removed/Then_next_request_is_unauthorized
When_invitation_is_accepted_twice/Then_second_attempt_fails
When_system_role_is_copied/Then_custom_role_has_its_permissions
When_system_role_is_edited/Then_request_fails
When_role_has_unknown_permission/Then_request_fails
When_business_reads_roles/Then_other_business_roles_are_not_visible   (tenant isolation)
When_member_is_given_a_role_of_another_business/Then_request_fails    (tenant isolation)
```

## App (frontend plan to follow)

- Session loads `GET /api/me`; a `useCan(permission)` hook hides tabs, buttons and screens.
- Ajustes → Equipo: members, pending invitations, invite form (email, role, coach).
- Ajustes → Roles (step 3): system roles read-only with "Copiar", custom roles editable with permissions grouped by module and "Ver solo lo suyo / Ver todo" for scoped modules.
- Accept invitation screen from the email link.

## Open decisions

| Decision | Proposal |
|---|---|
| Can a coach register new clients and students? | No in the `Coach` role; an owner can grant `students.manage` with a custom role |
| One user in several businesses | Supported by the data (unique `TenantId` + `UserId`), but sign-in picks one business; a business switcher comes when someone needs it |
| Family accounts (`Client` role, linked to `ClientId`) | Out of scope; the same `BusinessMembers` shape leaves room for it |
