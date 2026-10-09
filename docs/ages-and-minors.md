# Ages and minors

Every person who signs in has a birth date, so the app knows who is a minor. This page records the decision, the legal background it rests on, and the rules the code applies.

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

- **Every new account** asks for it: the owner when signing up, and anyone accepting an invitation without an account (client, student, instructor, team member). It is stored on the account (`AspNetUsers.BirthDate`) and the person can correct it from Editar perfil.
- **Students** also have a birth date typed by the team (`Students.BirthDate`). Inviting a student to the app requires it, and the invitation screen comes with it filled in so the person confirms or corrects it.
- **A client who attends class** (the contact has a student with their own name) gives their birth date in the registration form, stored on that student. Contacts registered before this rule are asked for it when they are invited to the app.
- **Accounts created before this change** have no birth date; they add it from Editar perfil (the form requires it to save).

## Rules

All of them live in `PersonAge` (`src/Core/Domain/Accounts`).

| Rule | Where | Error |
|---|---|---|
| The owner of a business is an adult (18) | Sign-up | `auth.owner_must_be_adult` |
| The client (contact) is an adult (18): they pay and get the reminders, and both countries need 18 to contract. A minor is registered as a student of an adult client, never as the client | Registering a client who attends class; importing a student without Responsable (the student becomes the client); inviting such a client to the app (`AttendingContact` in `src/Core/UseCases/Clients`) | `client.contact_must_be_adult` |
| Below the minimum age of the branch's country (13 in Argentina (`54`), 14 in Spain (`34`), 16 anywhere else, the GDPR maximum) a person still gets an account of their own, but only after the client who pays authorizes it | Inviting a student | — |
| Anyone accepting an invitation below that age needs that authorization | Accepting any invitation | `auth.too_young_for_own_account` |

### Authorization of the client who pays

Inviting a student below the minimum age does not email the student. It emails the client who pays (`Clients.Email`, required in that case: `student_invitation.guardian_email_required`) a link to `/authorize-student-app?token=`:

- **Autorizar** (`POST /api/auth/student-app-invitations/guardian-consent/accept`) stores `GuardianConsentedAt` and `GuardianEmail` on the invitation, which is the record that the parents consented, and only then emails the student their own invitation.
- **No autorizar** (`.../guardian-consent/decline`) closes the invitation and notifies the team member who sent it.
- If the client already uses the app, they also get a push notification ("Tomás quiere usar la app") and a card on the home of their app with the same two buttons (`POST /api/student-app/guardian-consents/{invitationId}/authorization` or `/refusal`). Only the client's own account sees it: the accounts of the students of the family get `404`.
- While it waits, the client card shows "Falta que su responsable autorice" and the button **Recordar al responsable**, which sends the request again.

The country comes from the branch's default calling code; add a country to `PersonAge` when the app reaches it.

### Next: orders of minors

Orders placed from the account of someone under 18 will wait for the approval of the client who pays (purchases need the parents in both countries). That decision uses the **most protective** of the two birth dates it can find: the one on the account and the one the team typed on the student. If a 14-year-old types a date that makes them 20, the team's date still makes them a minor, so lying does not skip the approval.
