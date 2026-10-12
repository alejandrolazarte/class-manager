# Sign-up confirmed by email

Status: built.

## The decision

Creating a business proves the owner's email before anything exists. The sign-up form asks for the owner's name, birth date, email and the business; it has no password. The email gets a link, and opening it is where the owner chooses the password and the business is created. This is the same flow invitations already follow: whoever opens the link owns the inbox.

Why:

- **The owner's email is the key to the account.** "¿Olvidaste tu contraseña?" sends the reset link there. A typo (`gmial.com`) used to go unnoticed until the owner needed it, with students and payments already loaded in an account nobody could recover.
- **Nobody can take someone else's email.** Before, anyone could create a business with `juan@…`, and Juan got "this email already has an account" when he tried.
- **The sign-up no longer tells who has an account.** The form answers the same for a new email and for a registered one; the registered one gets an email offering to sign in instead.
- **Google sign-in later needs it.** Linking a Google login to an existing account is only safe when that account proved its email; otherwise an attacker who signed up first with the victim's email keeps a password into the account the victim later uses ("pre-account hijacking").

There is no "unconfirmed account" state to handle: an account only exists after its email was confirmed, so nothing in the app is blocked waiting for a confirmation.

## The flow

1. `POST /api/auth/sign-up` (`RequestSignUpUseCase`) validates everything a sign-up can get wrong (name, adult birth date, email, business name, country, time zone, currency) so the form shows errors right away, and answers `202 Accepted`.
   - New email: it stores a `PendingSignUp` with the form data and a single-use token (stored hashed, like invitations and password reset tokens), revokes the earlier pending sign-ups of that email, and emails the link `/complete-sign-up?token=…`. The link expires in 24 hours (`PendingSignUp.Lifetime`).
   - Registered email: nothing is stored; the email says the account exists and links to sign in.
2. The app shows "Revisá tu email" with the address and a button to change it.
3. Opening the link shows "Elegí tu contraseña". `POST /api/auth/sign-up/complete` (`CompleteSignUpUseCase`) checks the token, creates the user (with `EmailConfirmed`), the organization, the branch, the owner as first instructor and the sign-up plan subscription in one transaction, uses up the link and starts the session (`201` with tokens).
   - Expired, revoked or used link: `400 auth.invalid_sign_up_token`; the app offers to sign up again.
   - The email got an account between the request and the link: `409 auth.email_taken`; the app offers to sign in.

Nothing is created until step 3, so abandoned sign-ups leave no users or businesses behind, only `PendingSignUps` rows that expire.

## Accounts created before this change

They keep working as they are: they have a password and their owners use them. The `ConfirmExistingAccountEmails` migration (Security) marks every existing account as confirmed, so `EmailConfirmed` means the same for every account: every way to create one (this sign-up, team invitations, student app invitations) proves the email first, and `UserAccountService.CreateAsync` sets it.

## Tests

- Unit (`ClassManager.Core.U.Tests`): requesting stores a pending sign-up and emails the link without creating an account; a registered email gets the "account exists" email; a second request revokes the first link; a minor or an unknown time zone is refused; completing creates the organization, branch, owner instructor and subscription and uses up the link; an expired or unknown link is refused.
- Integration (`ClassManager.Api.I.Tests`): the full request + link flow through the API, the account's email is confirmed, a used link answers 400, nothing exists before the link is opened, a taken email gets the "account exists" email, and a failure while creating the business leaves no user behind.
- Frontend: the form shows "Revisá tu email" and starts no session; a valid link signs in and opens Inicio; a used link offers to sign up again; a registered email offers to sign in.
- E2E: the owner signs up, opens the link from the email and lands on Inicio. Every e2e test that seeds a business goes through the same email link: `e2e/support/globalSetup.ts` starts one SMTP sink for the whole run that saves each email to `e2e/.mailbox/`, and `e2e/support/mailbox.ts` reads the links from there.
