# Frontend plan — Roles, team and invitations

The app shows each member only what their role allows, owners manage their team from Ajustes, and invited people accept from the email link.

Backend contract: [backend/20260928-roles-and-permissions](../../backend/20260928-roles-and-permissions/plan.md) and [authorization](../../authorization.md).

## Current member

- `GET /api/me` loads next to the business when the tabs open (`MemberProvider`), with the same loading and retry states as `BusinessProvider`.
- `useCurrentMember()` returns the member; `useCan(...permissions)` is true when the member has any of them.
- Permission codes live in `src/features/members/permissions.ts`, mirroring `Permissions` in the backend.
- The server stays the only enforcement: hiding is for clarity, not security.

## What each permission shows

| Where | Shown when |
|---|---|
| Tab Cuotas | `payments.view.all` |
| Hoy: "Nueva clase particular" | `privateLessons.manage.all` or `.own` |
| Sesión: marcar asistencia | `attendance.record.all` or `.own` |
| Sesión: cancelar, cambiar horario | `sessions.manage` |
| Clase particular: marcar asistencia | `attendance.record.all` or `.own` |
| Clase particular: cancelar, mover, borrar | `privateLessons.manage.all` or `.own` |
| Formulario de clase particular: coach | Only the member's own coach when they lack `privateLessons.manage.all` |
| Clases: "Nueva clase" | `classGroups.manage` |
| Detalle de clase: editar | `classGroups.manage` |
| Detalle de clase: inscribir, dar de baja | `enrollments.manage` |
| Alumnos: "Nuevo alumno", "Agregar alumno" | `students.manage` |
| Ficha de familia: cuota y pagos | `payments.view.all` |
| Ficha de familia: registrar pago, cambiar cuota, borrar pago | `payments.record` |
| Ficha de familia: saldo de clases | `classPacks.view` |
| Ficha de familia: vender pack, borrar venta | `classPacks.sell` |
| Ajustes: negocio, cuota mensual | `business.manage` |
| Ajustes: profes | `instructors.view` (nuevo profe: `instructors.manage`) |
| Ajustes: packs | `classPacks.view` (nuevo pack: `classPacks.manage`) |
| Ajustes: importar y exportar | `importExport.run` |
| Ajustes: equipo | `members.view` |

## Navigation

```text
app/
  accept-invitation.tsx                → AcceptInvitationScreen (?token=), outside the tabs
  (tabs)/settings/team/index.tsx       → TeamScreen
  (tabs)/settings/team/invite.tsx      → InviteMemberScreen
  (tabs)/settings/team/[memberId].tsx  → MemberScreen
```

## Screens

### Equipo — `settings/team/index.tsx`

- Members: name, email, role ("Dueño de la sede", "Coach · Marcos Díaz", "Solo lectura"), "Vos" on the current user. Tap → member.
- Pending invitations: email, role, "Vence el dd/mm", "Revocar".
- "Invitar" (`members.manage`).

### Invitar — `settings/team/invite.tsx`

Email, role chips (Coach, Solo lectura, and Dueño de la sede only for `branchOwners.manage`), coach picker when the role is Coach (active coaches not linked to anyone yet). Sent → toast "Invitación enviada", back to Equipo.

### Miembro — `settings/team/[memberId].tsx`

Role chips and coach picker as in Invitar, "Guardar", "Quitar del equipo" with confirmation. Not shown for the current user.

### Aceptar invitación — `accept-invitation.tsx`

Full name and password, "Unirme". A person who already has an account with that email can leave the password empty. Errors: invalid or expired link (with "Ir a ingresar"), coach already linked. Accepted → signed in, Hoy.

## Tests (write first)

```text
src/features/members/__tests__/
  When_member_lacks_payments_permission/Then_fees_tab_is_hidden.test.tsx
  When_coach_opens_settings/Then_only_allowed_rows_are_shown.test.tsx
  When_coach_opens_a_session/Then_cancel_and_reschedule_are_hidden.test.tsx
  When_coach_opens_a_client/Then_fees_and_packs_are_hidden.test.tsx
  When_viewer_opens_a_client/Then_payment_actions_are_hidden.test.tsx
  When_coach_schedules_a_private_lesson/Then_only_their_coach_is_offered.test.tsx
  When_team_is_listed/Then_members_and_invitations_are_shown.test.tsx
  When_coach_is_invited/Then_request_has_email_role_and_coach.test.tsx
  When_member_role_is_changed/Then_request_has_new_role.test.tsx
  When_invitation_is_accepted/Then_session_starts.test.tsx
  When_invitation_link_is_invalid/Then_error_is_shown.test.tsx
```

## Implementation notes

- Status: done. Tests in `src/features/members/__tests__`.
- `renderWithProviders` takes a `member` option; it defaults to a brand owner with every permission, so existing screen tests keep their behavior. `buildCoach()` and `buildViewer()` in `src/testing/memberFactory.ts` mirror the backend system roles.
- The tab rules live in `src/navigation/tabDefinitions.ts` (`isTabVisible`), so they are tested without rendering the router.
- Lists that a member can read but not change (profes, packs) show their items without the chevron and without "Nuevo".
- The coach picker in Invitar and Miembro only lists active coaches not linked to another member (`freeInstructors`).
- Accepting an invitation starts the session with `startedBy: "invitation"` and opens Hoy.
- Pending: manual check on Android (Expo Go) and on the installed web app.

## Out of scope

- Custom roles editor (step 4 of the backend plan).
- Branch switcher (step 3).
- Resending an invitation (revoke and invite again).
