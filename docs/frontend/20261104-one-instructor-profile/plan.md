# One profile per instructor

A profe on the team had two screens: the member screen (role, brand owner, remove) and the instructor profile, joined by "Ver ficha de profe". For the business they are one person, so the instructor profile is now the only profile, laid out like the student profile.

## Instructor profile (`/settings/instructors/{id}`)

Same order as the student profile:

1. Header: photo, "Profe", name, email, **Editar**.
2. **Email** action tile.
3. **App del equipo** card (same card shape as "App de alumno"): access status and Invitar / Reenviar / Revocar as before. When the profe has an account, the card also shows **Rol** (the branch role, or "Dueño de marca") with **Cambiar rol**, which opens the member screen. Hidden for your own membership and for platform roles, as in Equipo.
4. Same-person notice, as before.
5. **Clases**: the active class groups the profe teaches, with days and start time (`classGroups.view.own` or `.all`).

Private lessons are not listed yet: the API only lists them per day (`GET /api/private-lessons/instructor-agenda`), so a "Particulares" list needs a new endpoint first.

## Equipo

Tapping a member linked to a profe opens the instructor profile (with `instructors.manage` or `members.manage`). Members without a profe still open the member screen.

## Member screen (`/settings/team/{memberId}`)

- **Dueño de marca** is one more role chip, first in the list, only for people with `brandOwners.manage`. Each system role shows what it allows under the chips (`roles.hint.*`, `team.member.brandOwnerHint`, `team.invite.instructorHint`). The separate Hacer dueño de marca / Quitar de dueños de marca buttons are gone.
- Saving with Dueño de marca makes the member brand owner (`PUT /api/members/{id}/brand-owner`); their branch role stays as it was, since a brand owner has every permission anyway. Saving a brand owner with any other role asks first (`ConfirmDialog`), removes them from the brand owners (`DELETE .../brand-owner`) and then changes the branch role if it changed. The last brand owner banner stays.
- For a member linked to a profe the title is "Rol de {name}" with "Sigue siendo el profe de sus clases. Solo cambia lo que ve en la app.", there is no "Profe vinculado" picker, and the link is kept for every role: `PUT /api/members/{id}/role` always sends the member's `instructorId`. The API already accepted an instructor on any role; it only requires one for roles with `.own` permissions.
- Members without a profe keep the picker when they get a role that needs one.

## Brand owner and branch owner

- **Dueño de la sede** (`BranchOwner`) is a role inside one branch (`BusinessMembers`): everything in that branch except giving the branch owner role.
- **Dueño de marca** (`BrandOwner`) lives on the brand (`OrganizationMembers`): every permission in every branch, plus creating branches and naming brand and branch owners ([authorization](../../authorization.md)). In the app it is picked like a role, but it is stored apart because it covers every branch at once.

## Tests

- `src/features/members/__tests__/When_a_member_is_an_instructor`, `When_a_member_is_linked_to_an_instructor`, `When_a_linked_instructor_gets_another_role`, `When_member_is_made_brand_owner`, `When_a_brand_owner_is_removed`.
- `src/features/instructors/__tests__/When_an_instructor_is_on_the_team`, `When_an_instructor_teaches_classes`.
