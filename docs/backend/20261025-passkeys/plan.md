# Passkeys: signing in with a fingerprint or face

Status: proposal to review before building it.

## Why

Signing in today needs an email and a password. On a phone that is the slowest step of opening the app, and passwords get reused, forgotten and phished. A **passkey** lets a person sign in with what unlocks their device (fingerprint, face or device PIN), in the web app, the installed PWA and later the Android app.

The app never sees the fingerprint or the face. The device keeps a private key, protected by its own biometric check, and the API stores only the matching public key. Signing in is the device signing a one-time challenge from the API. A passkey belongs to `laztty.com`, so a fake site can't use it: it can't be phished.

## Decisions

### An extra way in, not a replacement

- Every account keeps its password. A passkey is added by the person, optionally, from **Mi perfil**, and can be removed there.
- "Olvidé mi contraseña" keeps working as before. Losing the phone never locks anyone out: they sign in with the password and remove the old passkey.
- No account is passkey-only for now (see [Later](#later)).

### Team and students alike

Passkeys belong to the **account** (`identity.AspNetUsers`), not to a business, so the same passkey signs a person into every business they belong to. After a passkey sign-in, the session starts in the same business and kind (team or student) a password sign-in would choose: the last session, else the default branch, else the default student link (`SignInUseCase` today). That choice moves to one shared class so both sign-ins always agree.

The backoffice is out of scope: operators sign in with Google, never with app accounts ([Backoffice](../20261017-backoffice/plan.md)).

### Built on ASP.NET Core Identity, without its cookie

.NET 10 Identity has passkeys built in: WebAuthn checks (`IPasskeyHandler<TUser>`), options (`IdentityPasskeyOptions`) and a table for the credentials (`AspNetUserPasskeys`, Identity schema version 3). No external WebAuthn library is needed.

Its `SignInManager` passkey methods don't fit this app: they keep the state between the two requests of each ceremony in an authentication cookie and finish by signing in with a cookie. This API signs in with JWT and refresh tokens, and the Android app can't rely on cookies (see [Web session cookie](../../web-session-cookie.md)). So `src/Security` calls `IPasskeyHandler<ApplicationUser>` directly and keeps the ceremony state itself:

- Each ceremony (registering a passkey or signing in with one) has two requests: **options** and **finish**.
- The options request stores the handler's state in a new table `identity.PasskeyCeremonies` and returns the options with a random **ceremony id**. Only the id's hash is stored, like refresh and reset tokens.
- The finish request sends the ceremony id back. The ceremony is used up on the first try, succeeded or not, and expires after 5 minutes (`SecurityOptions.PasskeyCeremonyLifetime`).
- A registration ceremony is bound to the account that started it: finishing it with another account's session is refused. That is the check Microsoft warns about when the handler is called directly ("an attacker could register a passkey on an account they don't own").
- Storing a new ceremony physically deletes the expired ones (`ExecuteDeleteAsync`, they have no value once gone), so the table stays small without a background job.

### Adding a passkey asks for the password

A stolen access token must not be enough to add a passkey, because that passkey would let the thief back in after the token expires. Adding one asks for the current password first, like changing the sign-in email. The account's email then gets a notice "Se agregó una passkey a tu cuenta", with the device name, in every language (`IEmailTexts`), so a person who didn't do it can remove it and change the password.

### Signing in without typing the email

The sign-in options ask for any passkey of `laztty.com` (discoverable credentials, `ResidentKeyRequirement = "required"`), so the person taps **Entrar con huella o cara** without typing an email. The device shows its own account picker if it has several. The API never says whether an email has passkeys, so this adds no way to find out who has an account.

A sign-in with a passkey follows the same rules as with a password:

- A locked-out account can't sign in with a passkey either until the lockout ends. A failed passkey check doesn't count as a failed password attempt, because a wrong fingerprint never reaches the API.
- An account with no team branch and no student link is refused with the same `invalid_credentials` error.
- A refused passkey sign-in is a `SignInFailed` security event, like a wrong password ([Audit log](../20261012-audit-log/plan.md)).

### Where passkeys belong: `laztty.com`

- **RP ID** (the domain a passkey belongs to): `laztty.com`, set with `IdentityPasskeyOptions.ServerDomain` from configuration (`Passkeys:ServerDomain`). It is never inferred from the request's `Host` header, which Microsoft lists as a risk. The apex domain, not `app.laztty.com`, so the Android app can use the same passkeys (it proves it belongs to `laztty.com` through `https://laztty.com/.well-known/assetlinks.json`).
- **Allowed origins**: the app runs on `app.laztty.com` and the API on `api.laztty.com`, so the default check (the request's own origin) doesn't apply. `ValidateOrigin` accepts only the origins listed in `Passkeys:AllowedOrigins`: `https://app.laztty.com`, `http://localhost:8081` in development and e2e, and, in step 3, the Android app's `android:apk-key-hash:…` origin.
- **User verification** is `required`: a passkey always means the device checked the fingerprint, face or PIN, never just a tap.

Locally, browsers allow WebAuthn on `http://localhost` without HTTPS, with `ServerDomain = localhost`.

### Removing a passkey

Removing a passkey asks first with `ConfirmDialog` ("¿Quitar esta passkey?", consequence: "No vas a poder entrar con ella en ese dispositivo.", **Volver** / **Quitar**). It is not undoable: the device may already have deleted its private key, so restoring the row could leave a passkey that no device has. The row is physically deleted, as it has no value once gone. Deleting the account deletes its passkeys (the foreign key cascades, like refresh tokens).

## Design

### Security library

| Area | Types |
|---|---|
| Passkeys | `IPasskeyService` (begin and finish registration, begin and finish sign-in, list, rename, remove), `PasskeyService` over `IPasskeyHandler<ApplicationUser>` and `UserManager.AddOrUpdatePasskeyAsync` / `GetPasskeysAsync` / `RemovePasskeyAsync` |
| Ceremonies | `PasskeyCeremony` (id, hash of the ceremony id, kind `registration` or `sign-in`, `UserId` for registrations, protected state, expiry), `PasskeyCeremonyKind` |
| Options | `PasskeyOptions` (`ServerDomain`, `AllowedOrigins`), bound from `Passkeys` and validated on start; `SecurityOptions.PasskeyCeremonyLifetime` |
| Persistence | `options.Stores.SchemaVersion = IdentitySchemaVersions.Version3`; migration `AddPasskeys` (`AspNetUserPasskeys`, `PasskeyCeremonies`) |

The handler's state is stored as it comes and never leaves the database: the app only ever holds the ceremony id. It is not encrypted with Data Protection, whose keys would need persisting outside the container for a ceremony to survive a restart, and anyone who can write to the database could change the passkeys directly anyway. Nothing in it is business-specific (ARCH003 still holds).

### Core and Infrastructure

- Core port `IPasskeyIdentityService` with the five operations the use cases need, adapted in Infrastructure over `IPasskeyService`, like `IIdentityService` over `IUserAccountService`.
- `SessionUserChooser` (Core): the "last session, else default branch, else default student link" choice, moved out of `SignInUseCase` and used by both sign-in use cases.
- Use cases, each registered with `AddUseCase`:

| Use case | Input | Kind |
|---|---|---|
| `BeginPasskeyRegistrationUseCase` | `BeginPasskeyRegistrationCommand([AuditSecret] Password)` | command |
| `FinishPasskeyRegistrationUseCase` | `FinishPasskeyRegistrationCommand([AuditPlain] CeremonyId, [AuditSecret] Credential, [AuditPersonal] Name)` | command |
| `ListMyPasskeysUseCase` | `ListMyPasskeysQuery` | query |
| `RemoveMyPasskeyUseCase` | `RemoveMyPasskeyCommand([AuditPlain] PasskeyId)` | command |
| `BeginPasskeySignInUseCase` | `BeginPasskeySignInCommand` | command (stores a ceremony) |
| `SignInWithPasskeyUseCase` | `SignInWithPasskeyCommand([AuditPlain] CeremonyId, [AuditSecret] Credential)` | command |

The passkey name defaults to the device the browser reports (for example "iPhone" or "Chrome en Windows") and the person can change it while adding it. `ListMyPasskeysQuery` returns the name, when it was added and when it was last used, never key material.

### Texts

New keys in `es.json` and `en.json` (sign-in button, Mi perfil section, add sheet, confirm dialog, errors) and the new email in every `IEmailTexts` language. The word **passkey** stays in both languages, as Apple, Google and Microsoft use it; the button explains it: "Entrar con huella o cara".

## API

| Method and route | Auth | What it does |
|---|---|---|
| `POST /api/me/passkeys/registration-options` | any account, rate limited | Checks the password, returns `{ ceremonyId, options }` |
| `POST /api/me/passkeys` | any account, rate limited | Finishes the registration, saves the passkey, sends the notice email |
| `GET /api/me/passkeys` | any account | Lists the account's passkeys |
| `DELETE /api/me/passkeys/{id}` | any account | Removes one of the account's passkeys |
| `POST /api/auth/passkey/sign-in-options` | anonymous, `authentication` rate limit | Returns `{ ceremonyId, options }` |
| `POST /api/auth/passkey/sign-in` | anonymous, `authentication` rate limit | Checks the passkey and starts a session: same responses as `/api/auth/sign-in`, including the refresh token cookie on the web |

`options` is the standard WebAuthn JSON (`PublicKeyCredentialCreationOptionsJSON` / `PublicKeyCredentialRequestOptionsJSON`) and `credential` is the JSON of the browser's `PublicKeyCredential`, so the app passes them through without reshaping them. Routes go in `ApiRoutes`.

## App

- **Support check**: `isPasskeySupported()` in `app/src/features/passkeys`, with a `.web.ts` version (`window.PublicKeyCredential` and `isUserVerifyingPlatformAuthenticatorAvailable()`) and a `.native.ts` version that returns `false` until step 3. Nothing about passkeys shows where they don't work.
- **WebAuthn calls**: `@simplewebauthn/browser` (`startRegistration`, `startAuthentication`). It converts the JSON options and credentials, which `PublicKeyCredential.parseCreationOptionsFromJSON` only does natively on recent browsers (Safari 18+), and works around password managers whose `toJSON` throws "Illegal invocation" (a known issue in Microsoft's docs). No install script, so nothing to add to `pnpm.onlyBuiltDependencies`.
- **Sign-in screen**: below the password form, a `secondary` `Button` **Entrar con huella o cara**, only when supported. It runs the two requests and then follows the same path as a password sign-in. Cancelling the device prompt does nothing; any other failure shows a toast.
- **Mi perfil** (team and student): a **Passkeys** group listing each passkey (name, added, last used), a `dashed` button **Agregar passkey** that opens a sheet with the password (`isRequired`) and the name, and a `dangerGhost` small button per row that opens the `ConfirmDialog`. Empty list: one line explaining what a passkey is, no button inside it.
- **Content Security Policy**: no change: WebAuthn needs no new source, and the app is never framed.

## Steps

1. **API and Security**: schema version 3 and migration, ceremonies, `PasskeyService`, use cases, endpoints, notice email, configuration (`Passkeys:ServerDomain`, `Passkeys:AllowedOrigins` in `appsettings`, the pilot deployment and [Pilot deployment](../../pilot-deployment.md)), integration tests with a software authenticator.
2. **Web app and PWA**: sign-in button, Mi perfil group, texts, jest tests, and one e2e test of the full flow with Chromium's virtual authenticator. Before/after screenshots of the sign-in screen and Mi perfil for the owner.
3. **Android app**: `react-native-passkey` (a native module: it works in the APK built by `eas build`, not in Expo Go), `assetlinks.json` served at `https://laztty.com/.well-known/assetlinks.json` with the APK's signing certificate fingerprint, and the `android:apk-key-hash` origin. iOS when there is an iOS app (`apple-app-site-association`, associated domains).

## Tests

Integration tests (`ClassManager.Api.I.Tests`) use a software authenticator in the test project that creates real WebAuthn attestations and assertions (ES256), so the API runs Identity's real checks:

- `When_a_passkey_is_added_with_the_right_password/Then_it_signs_in`
- `When_a_passkey_is_added_with_a_wrong_password/Then_no_ceremony_starts`
- `When_a_registration_is_finished_by_another_account/Then_it_is_refused`
- `When_a_ceremony_is_used_twice/Then_the_second_is_refused`
- `When_a_ceremony_expired/Then_it_is_refused`
- `When_the_origin_is_not_allowed/Then_the_sign_in_is_refused`
- `When_the_account_is_locked_out/Then_the_passkey_sign_in_is_refused`
- `When_a_team_member_signs_in_with_a_passkey/Then_the_session_starts_in_the_last_branch`
- `When_a_student_signs_in_with_a_passkey/Then_the_session_is_a_student_session`
- `When_signing_in_from_the_web/Then_the_refresh_token_is_a_cookie`
- `When_a_passkey_is_removed/Then_it_no_longer_signs_in`
- `When_a_passkey_of_another_account_is_removed/Then_it_is_not_found`
- `When_a_passkey_is_added/Then_the_account_gets_a_notice_email`
- `When_the_account_is_deleted/Then_its_passkeys_are_deleted`
- `When_a_passkey_sign_in_fails/Then_a_security_event_is_written`

Unit tests (`ClassManager.Core.U.Tests`) for `SessionUserChooser` and the use cases with fakes. Jest: the sign-in button only shows when supported, cancelling the prompt does nothing, removing asks first and nothing happens until confirmed. E2E: add a passkey from Mi perfil, sign out, sign in with it (Playwright's CDP `WebAuthn.addVirtualAuthenticator` with user verification on).

## Later

- Offer to add a passkey right after a password sign-in on a device that supports it.
- Passkeys in the browser's email autofill (conditional UI, `autocomplete="username webauthn"`).
- Accounts created with a passkey and no password, once recovery without a password is designed.
