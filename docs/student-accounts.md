# Student accounts outlive their businesses

Status: decided. "Eliminar mi cuenta" is built; the rest of the pending work is listed at the end.

## Decision

An app account belongs to the person, not to a business. The two are kept apart:

- **The account** (`Security`: email, password, name) is the person's sign-in. No business owns it.
- **The link** (`ClientAccounts`, one row per business) gives that account access to one client of one business. It belongs to the business and lives in its tenant, together with the student data (classes, payments, orders).

When a business removes a student or stops working with them, only the link goes. The account stays, even when it has no link left. Accounts are **not** deleted just because the person is not enrolled anywhere.

## Why

- **The same person comes back.** People change schools and teachers. With the account kept, the next business only links it: the invitation already works with an existing account ("si ya usás la app con este email, entrás con tu cuenta de siempre"). A deleted account would make the person sign up again and would look like the app lost their data.
- **It is where the product is going.** Later, a person could sign up on their own, find a business inside the app and ask to join. An account with no link is the starting point of that flow, not something to clean up.
- **There is little to keep.** An account with no link holds only email, name and password. The data with business meaning stays in each business's tenant and follows that business's rules ([deleting data](deleting-data.md)).
- **The student app is in every plan** (Free, Lite, Pro, Enterprise), so any business can invite students and accounts are expected to move between businesses.

## When an account is deleted

1. **The person asks for it.** This is a legal right (GDPR in Spain; Ley 25.326 in Argentina) and Google Play and the App Store require apps that create accounts to offer it inside the app.
2. **Optionally, after long inactivity.** For example, no sign-in for 2 years and no link left, with an email warning before. Data hygiene only, not required.

Never because the person has no current enrollment.

## Minors

A guardian's consent is given for one invitation to one business ([ages and minors](ages-and-minors.md)). It does not cover the account on its own. A minor whose account has no link left keeps the account but can't do anything alone with it (for example, search for businesses or ask to join) without a new consent.

## Pending

- [x] **"Eliminar mi cuenta"** in Mi perfil (student app and team app): asks for the current password and confirms with `ConfirmDialog`, deletes the account in `Security`, soft deletes its `ClientAccounts` links and team memberships, deletes its push subscriptions, and keeps the student data each business owns, with the person's user unlinked from the account ([Users per business, step 6](backend/20261017-users/plan.md#steps)). The only brand owner of a brand can't delete their account until someone else is a brand owner.
- [x] **Signing in with no link:** a right password for an account with no business left (its student was deleted, or it was removed from every team) gets `403 auth.no_business_access` and "Tu cuenta ya no tiene acceso a ninguna escuela. Si creés que es un error, hablá con tu escuela." instead of "Email o contraseña incorrectos". A wrong password still gets the generic message, so nobody learns whether an email exists. An open session ends at the next request: the API refuses it and refreshing the token fails too, since the refresh checks the link.
- [ ] **Home with no link:** a screen for a signed-in person with no business (for example, to find one) instead of the sign-in message above.
- [ ] **Find a business and ask to join:** the self sign-up and search flow. Proposal only; needs its own plan.
- [ ] **Minors with no link:** block actions that need a guardian's consent until a new one is given.
- [ ] **Inactive accounts (optional):** decide whether to delete accounts with no link and no sign-in for a long time, and the warning email.
