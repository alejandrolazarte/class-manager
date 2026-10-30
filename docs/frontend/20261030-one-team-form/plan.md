# One team: Equipo and Profes in one screen and one form

## Why

Ajustes had two entries, **Equipo** (accounts with a role) and **Profes** (instructors), each with its own create form. Both forms asked for an email, and inviting a profe from Equipo required creating the profe first in Profes. Every profe ends up as an instructor, and a profe who uses the app is also a team member, so the split made people wonder where to add someone.

## What changed

- Ajustes shows a single **Equipo** entry, visible with `members.view` or `instructors.view`. The **Profes** entry and its list screen are gone.
- **Equipo** lists:
  - Miembros and Invitaciones pendientes (`members.view`). An invitation linked to a profe is not listed there: the profe is listed under Profes sin cuenta en la app, and its profile has **Reenviar** and **Revocar**.
  - **Profes sin cuenta en la app**: instructors not linked to a member (no account yet, or invited and not accepted). A profe with an account is listed once, as a member.
  - Without `members.view` (for example a profe), every instructor is listed under **Profes**.
  - The instructors import/export menu moved to the Equipo header.
- **Sumar al equipo** (`/settings/team/invite`, also `/classes/instructors/new` from the class form) replaces "Invitar al equipo" and "Nuevo profe":
  - **Rol** (only with `members.manage`; Profe by default).
  - Profe role (any role that needs an instructor): **Nombre y apellido** (required) and **Email** (optional: without it the profe is created and does not get an invitation). Saving creates the instructor and, with an email and `members.manage`, invites them with the chosen role.
  - Other roles: **Email** (required) and the invitation is sent.
  - While typing the name, active profes without an account whose name matches are offered ("¿Es un profe que ya cargaste?"). Choosing one links the invitation to that profe instead of creating a duplicate; the field becomes the "Profe vinculado" picker and the email is filled from the profe.
  - Without `instructors.manage` but with `members.manage`, the Profe role shows the "Profe vinculado" picker as before, since the person cannot create instructors.
  - Without `members.manage` (only `instructors.manage`) the form is name and email, as the old "Nuevo profe".
  - The team plan limit is checked when the form sends an invitation, not when it opens, so a profe without an account can still be added at the limit.
- **Editar profe** only edits; it no longer creates.
- The member screen of a profe has **Ver ficha de profe**, which opens the instructor profile (photo, email, same-person notice, edit, deactivate).

## Not changed

- The API: the form composes `POST /api/instructors` and `POST /api/members/invitations`, as "Nuevo profe" already did.
- Instructor detail, edit, import and separate routes stay under `/settings/instructors/...`.
