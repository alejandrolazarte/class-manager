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

- **Every new account** asks for it: the owner when signing up, and anyone accepting an invitation without an account (client, student, coach, team member). It is stored on the account (`AspNetUsers.BirthDate`) and the person can correct it from Editar perfil.
- **Students** also have a birth date typed by the team (`Students.BirthDate`). Inviting a student to the app requires it, and the invitation screen comes with it filled in so the person confirms or corrects it.
- **Accounts created before this change** have no birth date; they add it from Editar perfil (the form requires it to save).

## Rules

All of them live in `PersonAge` (`src/Core/Domain/Accounts`).

| Rule | Where | Error |
|---|---|---|
| The owner of a business is an adult (18) | Sign-up | `auth.owner_must_be_adult` |
| A person has an account of their own only from the minimum age of the branch's country: 13 in Argentina (`54`), 14 in Spain (`34`), 16 anywhere else (the GDPR maximum, the safe default) | Inviting a student, and accepting any invitation | `auth.too_young_for_own_account` |
| Younger children use the app through the account of the client who pays, who sees all the family | — | — |

The country comes from the branch's default calling code; add a country to `PersonAge` when the app reaches it.

### Next: orders of minors

Orders placed from the account of someone under 18 will wait for the approval of the client who pays (purchases need the parents in both countries). That decision uses the **most protective** of the two birth dates it can find: the one on the account and the one the team typed on the student. If a 14-year-old types a date that makes them 20, the team's date still makes them a minor, so lying does not skip the approval.
