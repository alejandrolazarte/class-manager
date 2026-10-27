# Ages and minors

Every person who signs in has a birth date, so the app knows who is a minor, except an adult client (contact) whose birth date nobody typed. This page records the decision, the legal background it rests on, and the rules the code applies.

## Legal background

Not legal advice; review the terms of service and privacy policy with a lawyer before a public launch.

| | Argentina | Spain |
|---|---|---|
| Consents to the processing of their own data | 13 (Civil and Commercial Code); a reform bill raises it to 16 with the parents' consent too | 14 (LOPDGDD art. 7; the GDPR allows 13 to 16); a bill raises it to 16 for social networks only |
| Buys or signs contracts alone | 18 | 18 |
| The app must know the age | Not explicit, but consent must be provable | Yes, with reasonable effort (GDPR art. 8) |
| Collect only what is needed | Yes (Law 25.326, "not excessive") | Yes (GDPR) |

Sources: Law 25.326 and the AAIP criteria (Argentina); LOPDGDD art. 7, GDPR art. 8 and the APDCAT opinion CNS 9/2019 (Spain).

## Where the birth date comes from

- **The owner and the team** type it when they create their account: the owner when signing up, an instructor or team member when accepting their invitation without an account. It is stored on the account (`AspNetUsers.BirthDate`) and the person can correct it from Editar perfil.
- **Students** have a birth date typed by the team (`Students.BirthDate`). Inviting a student to the app requires it. Accepting the app invitation never asks for it: the new account takes the one the team typed. Asking again added nothing: the guardian already authorized a minor, an adult can change it from Editar perfil anyway, and a wrong date would leave the account disagreeing with the student card.
- **The client (contact)** is never asked for it either: neither the registration form nor the app invitation has the field, and accepting the invitation doesn't ask. When the team typed one (the contact attends class and their student has a birth date) the new account takes it; otherwise the account has none until the person adds it from Editar perfil. The team's confirmation that the contact is 18 or older is what vouches for their age. A contact who already uses the app has it on their account, and when they attend class and that student has no birth date, accepting the invitation copies the account's onto it.
- **Accounts created before this change** have no birth date; they add it from Editar perfil (the form requires it to save).

## Rules

All of them live in `PersonAge` (`src/Core/Domain/Accounts`).

| Rule | Where | Error |
|---|---|---|
| The owner of a business is an adult (18) | Sign-up | `auth.owner_must_be_adult` |
| The client (contact) is an adult (18): they pay and get the reminders, and both countries need 18 to contract. A minor is registered as a student of an adult client, never as the client | Accepting the app invitation of the client, with the birth date of their existing account or, for a new one, the one the team typed; registering or inviting a client who attends class when the team already has a birth date for them; importing a student without Responsable (the student becomes the client) (`AttendingContact` in `src/Core/UseCases/Clients`) | `client.contact_must_be_adult` |
| Below the minimum age of the branch's country (`Countries.OwnAccountMinimumAge`: 13 in Argentina, 14 in Spain, 16 in the other countries, the GDPR maximum) a person still gets an account of their own, but only after the client who pays authorizes it | Inviting a student | — |
| Anyone accepting an invitation below that age needs that authorization | Accepting any invitation (for a student, with the birth date on the student card, in case the team changed it after inviting) | `auth.too_young_for_own_account` |
| The team member who invites the client (contact) to the app confirms that they are 18 or older | Inviting the client: the "Enviarle invitación a la app" switch of Nuevo alumno and Invitar on the client card | `student_invitation.contact_adult_confirmation_required` |

### The team confirms that the contact is an adult

The team never types the contact's birth date, so a minor registered as the contact could lie about their age when creating the account and nobody would know. To close that, inviting the contact to the app asks the team member to tick "Confirmo que {nombre} es mayor de 18 años". The invitation stores who ticked it and when (`ClientInvitations.ContactAdultConfirmedByUserId`, `ContactAdultConfirmedAt`): the business, who knows the person, declares the age, the way schools do in ClassDojo or Google Classroom.

- The tick only appears when the contact is invited. In Nuevo alumno, the invitation switch appears as soon as something is typed in the email, and the tick appears while the switch is on; Registrar stays disabled until it is ticked.
- Resending a pending invitation that was already confirmed keeps that confirmation and doesn't ask again.
- If the contact is a minor, the team can't tick it: the hint under the tick tells them to set a responsible adult as the contact and add the minor with "Agregar hijo/a u otra persona".
- Accepting doesn't ask the contact for their birth date, so the tick is the only check for a new account. A contact who already has an account is still checked with its birth date (`client.contact_must_be_adult`). A wrong tick or a lie is the business's responsibility; nothing short of identity documents prevents it, and no app of this kind asks for them.

### Authorization of the client who pays

Inviting a student below the minimum age does not email the student. It emails the client who pays (`Clients.Email`, required in that case: `student_invitation.guardian_email_required`) a link to `/authorize-student-app?token=`:

- **Autorizar** (`POST /api/auth/student-app-invitations/guardian-consent/accept`) stores `GuardianConsentedAt` and `GuardianEmail` on the invitation, which is the record that the parents consented, and only then emails the student their own invitation.
- **No autorizar** (`.../guardian-consent/decline`) closes the invitation and notifies the team member who sent it.
- If the client already uses the app, they also get a push notification ("Tomás quiere usar la app") and a card on the home of their app with the same two buttons (`POST /api/student-app/guardian-consents/{invitationId}/authorization` or `/refusal`). Only the client's own account sees it: the accounts of the students of the family get `404`.
- While it waits, the client card shows "Falta que su responsable autorice" and the button **Recordar al responsable**, which sends the request again.

The age comes from the branch's country (`Businesses.CountryCode`, see [Countries](countries.md)), never from a phone number: a student of a branch in Spain may have an Argentine number and still follows the rules of Spain.

### Next: orders of minors

Orders placed from the account of someone under 18 will wait for the approval of the client who pays (purchases need the parents in both countries). That decision uses the **most protective** of the two birth dates it can find: the one on the account and the one the team typed on the student. If a 14-year-old changes their account to a date that makes them 20 from Editar perfil, the team's date still makes them a minor, so lying does not skip the approval.
