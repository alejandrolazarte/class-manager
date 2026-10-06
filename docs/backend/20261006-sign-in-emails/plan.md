# Sign-in emails and app accounts per person

Status: steps 1–3 are built. Step 4 is a proposal to review before building it.

## The rule: one email per person, everywhere

A business sees one email for each person (a client or an instructor). That email is the one the app uses to invite them, and once they accept, it is the one they sign in with. The three places that store an email never disagree on screen:

| Where | What it is | Who changes it |
|---|---|---|
| `Client.Email`, `Instructor.Email` | The person's email in the business | The business, until the person signs in |
| `ClientInvitation.Email`, `MemberInvitation.Email` | Where an invitation link was sent | Nobody: it is a record of the send |
| `identity.AspNetUsers.Email` | The email used to sign in | Only the person, from **Mi perfil** |

The invitation keeps its own copy on purpose: accepting creates the account with the invitation's email, so the link proves the person owns that inbox. If the invitation read the client's current email instead, a link sent to `ana@` could create an account for whatever address someone typed later.

## 1. Clients

- Inviting a client to the app always sends to the client's email. When the person inviting types another email, it is saved as the client's email first, then the invitation goes there (`InviteStudentAppUseCase`).
- A new invitation revokes every pending invitation of that client, so the latest email is the only one with a working link.
- Inviting a client that already uses the app is refused (`student_invitation.already_uses_the_app`).
- Changing the client's email while the invitation is pending sends the invitation again to the new email (the edit screen does it after saving).
- Once the client uses the app, the email can only be the one they sign in with (`client.email_used_to_sign_in`). The app shows the sign-in email (`appAccess.signInEmail`) and locks the field with "Lo usa para entrar a la app. Solo lo puede cambiar desde su perfil."

This keeps one person with the app per client. Step 4 replaces it with one account per person.

## 2. Instructors

- Instructors have an optional email (`AddInstructorEmail` migration).
- The instructor screen shows the email and a card to invite them to the app. The invitation is the team invitation with the Coach role linked to the instructor, so accepting works as before.
- Every team invitation linked to an instructor saves its email as the instructor's email and revokes the instructor's other pending invitations (`InviteMemberUseCase`), whether it was sent from the instructor screen or from Equipo.
- Changing the email of an invited instructor sends the invitation again.
- Once a team member is linked to the instructor, the email can only be the one they sign in with (`instructor.email_used_to_sign_in`), and the form locks the field.

## 3. Changing the sign-in email (Mi perfil)

Why the business cannot change it: whoever controls the sign-in email controls the account, because "Olvidé mi contraseña" sends the reset link there. If editing a client changed their sign-in email, anyone on the team could take over that person's account, which may also be used in other businesses. A typo would lock the person out too.

The flow, available to team members and students from Ajustes → Mi perfil:

1. The person writes the new email and their current password. `POST /api/me/account/email-change` checks the password, refuses an email used by another account, and emails a single-use link to the new address. The link expires in 24 hours (`SecurityOptions.EmailChangeTokenLifetime`).
2. Opening the link calls `POST /api/auth/email-change/confirm`. In one transaction it moves the account to the new email (also the user name), uses up every pending change link of the user, and updates the email of every client and instructor linked to the account in every business.
3. The previous address gets a notice that the email changed.

Tokens are stored hashed in `identity.EmailChangeTokens` (`AddEmailChangeTokens` migration), like password reset tokens.

## 4. Proposal: one app account per person

Today an invitation belongs to the client, and whoever accepts it sees every student of that client, payments included. Students have no email. The proposal:

- **Every person can have their own account**: the client who pays (Laura) and each student (her children Tomás and Sofía). A person who is both client and student (María) has one account.
- **The family is only a grouping** for billing and contact.
- **What each account sees**:
  - The client: every student of the group, fees and payments, and orders.
  - A student: only their own classes, absences and makeups.
- **Shop orders**: adults order freely. Orders from minors wait for the client's approval before the business sees them. The app does not charge cards online today, so no minor ever pays with a card; if online payments are added, they stay adult-only, as payment providers require.
- **Ages (Spain)**: the student's birth date decides.
  - **18 or older**: own account, invited by the business or the client.
  - **14 to 17**: own account, invited by the business or the client, and shop orders approved by the client. In Spain, minors can consent to the processing of their data from 14 (LOPDGDD, Ley Orgánica 3/2018, article 7).
  - **Under 14**: own account only when the client invites them from their own account, because parental consent is required and the invitation records who gave it and when. Otherwise the client manages them from their account, as today.
  - **No birth date**: the app asks for it before inviting.
- **Each person's email follows the rule above**: saved when typed in an invitation, invitation sent again when it changes, locked once they sign in, changed by the person from Mi perfil.

To decide before building:

1. Do students see fees, or only the client?
2. Do adults in the same group order on their own, or does the client approve their orders too?
3. Under 14: own account invited by the client, or no account at all?
4. How to keep the proof of parental consent; check it with someone who knows data protection before launch.
