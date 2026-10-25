# Passkeys: signing in with a fingerprint or face

Status: steps 1 (API and Security) and 2 (web app and PWA) are built. Step 3 (Android) is built in the app and waits on the infrastructure in [Passkeys in the Android app](../../pilot-deployment.md#passkeys-in-the-android-app) (the signing fingerprint, the `laztty.com` apex and the app's origin in the API) before it can be tried on a phone.

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

The sign-in options ask for any passkey of `laztty.com` (discoverable credentials, `ResidentKeyRequirement = "required"`), so the person taps **Ingresar con passkey** without typing an email. The device shows its own account picker if it has several. The API never says whether an email has passkeys, so this adds no way to find out who has an account.

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
| Ceremonies | `PasskeyCeremony` (id, hash of the ceremony id, kind `Registration` or `SignIn`, `UserId` for registrations, the handler's state, expiry), `PasskeyCeremonyKind`, `PasskeyCeremonyOptions` |
| Options | `PasskeyOptions` (`ServerDomain`, `AllowedOrigins`), bound from `Passkeys` and validated on start; `SecurityOptions.PasskeyCeremonyLifetime` |
| Persistence | `options.Stores.SchemaVersion = IdentitySchemaVersions.Version3`; migration `AddPasskeys` (`AspNetUserPasskeys`, `PasskeyCeremonies`) |

Identity reads the schema version from the application's `IdentityOptions`, so a `SecurityDbContext` created outside the application (the design-time factory, the test fixtures) calls `UseIdentitySchema()` to build the same model. Schema version 3 also includes version 2, which shortens the keys of `AspNetUserLogins` and `AspNetUserTokens` to 128 characters and `AspNetUsers.PhoneNumber` to 256. The app uses none of the three (both tables are empty), and SQL Server can't shorten a primary key column, so the migration drops and re-adds those two primary keys around the change.

The handler's state is stored as it comes and never leaves the database: the app only ever holds the ceremony id. It is not encrypted with Data Protection, whose keys would need persisting outside the container for a ceremony to survive a restart, and anyone who can write to the database could change the passkeys directly anyway. Nothing in it is business-specific (ARCH003 still holds).

### Core and Infrastructure

- Core port `IPasskeyIdentityService` with the operations the use cases need, adapted in Infrastructure (`PasskeyIdentityService`) over `IPasskeyService`, like `IIdentityService` over `IUserAccountService`. A passkey sign-in returns the same `CredentialVerification` as a password check.
- `ISignInSessionService` (`SignInSessionService`, Core): lockout, then the "last session, else default branch, else default student link" choice, moved out of `SignInUseCase` and used by both sign-in use cases.
- Use cases, each registered with `AddUseCase`:

| Use case | Input | Kind |
|---|---|---|
| `BeginPasskeyRegistrationUseCase` | `BeginPasskeyRegistrationCommand([AuditSecret] Password)` | command |
| `FinishPasskeyRegistrationUseCase` | `FinishPasskeyRegistrationCommand([AuditSecret] CeremonyId, [AuditSecret] Credential, [AuditPersonal] Name)` | command |
| `ListMyPasskeysUseCase` | `ListMyPasskeysQuery` | query |
| `RemoveMyPasskeyUseCase` | `RemoveMyPasskeyCommand([AuditPlain] PasskeyId)` | command |
| `BeginPasskeySignInUseCase` | `BeginPasskeySignInCommand` | command (stores a ceremony) |
| `SignInWithPasskeyUseCase` | `SignInWithPasskeyCommand([AuditSecret] CeremonyId, [AuditSecret] Credential)` | command |

The ceremony id is `[AuditSecret]`: while it lives it is the handle that finishes the ceremony. The passkey name (1 to 60 characters) is filled in by the app with the device the browser reports (for example "iPhone" or "Chrome en Windows") and the person can change it while adding it. `ListMyPasskeysQuery` returns the id (the credential id in base64url), the name and when it was added, never key material. Identity doesn't store when a passkey was last used, so the list doesn't show it.

### Texts

New keys in `es.json` and `en.json` (sign-in button, Mi perfil row, passkeys list, add form, confirm dialog, errors) and the new email in every `IEmailTexts` language. The word **passkey** stays in both languages. The buttons never name the fingerprint or the face, because a passkey can also be unlocked with the device PIN, a password manager or another phone: the sign-in button is **Ingresar con passkey** ("Sign in with a passkey"), and the explanation in Mi perfil names the ways to unlock it ("Una passkey te permite ingresar con la huella, la cara o el bloqueo de pantalla de tu dispositivo, sin escribir la contraseña.").

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

- **Support check**: `isPasskeySupported()` in `app/src/features/passkeys/webAuthn.web.ts` (`browserSupportsPasskeys()` from the library: a platform authenticator or a phone over the hybrid transport), and `webAuthn.ts` for Android and iOS, which returns `false` until step 3. `usePasskeySupport()` reads it. Nothing about passkeys shows where they don't work.
- **WebAuthn calls**: `@simplewebauthn/browser` (`startRegistration`, `startAuthentication`). It converts the JSON options and credentials, which `PublicKeyCredential.parseCreationOptionsFromJSON` only does natively on recent browsers (Safari 18+), and works around password managers whose `toJSON` throws "Illegal invocation" (a known issue in Microsoft's docs). No install script, so nothing to add to `pnpm.onlyBuiltDependencies`.
- **Sign-in screen**: below **Ingresar**, a `secondary` `Button` **Ingresar con passkey** with the fingerprint icon, only when supported. `SessionProvider.signInWithPasskey()` runs the two requests and starts the session like a password sign-in (including the account chooser for someone with a team and a student account). Cancelling the device prompt does nothing; a passkey the API refuses shows a banner ("No pudimos verificar la passkey…"), and lockout and network errors show the same banners as the password.
- **Mi perfil** (team and student): a **Passkeys** row in Acceso, only when supported, opens the passkeys screen (`/settings/passkeys`, `/student-app/passkeys`). It follows the list rules: each passkey with its name and the day it was added, a `dangerGhost` small **Quitar** per row that asks first with `ConfirmDialog`, the floating **Agregar passkey** button as the only create action, and an `EmptyState` that points to it.
- **Agregar passkey** (`/settings/add-passkey`, `/student-app/add-passkey`): a form closed with ✕, with the current password and the name (both required, the name filled in with the device the browser reports, up to 60 characters). It asks the API for the options, lets the device create the passkey and saves it; then goes back with the toast "Passkey agregada". A wrong password shows under the field, a device that already has a passkey of the account shows a warning, and cancelling the device prompt does nothing.
- **Content Security Policy**: no change: WebAuthn needs no new source, and the app is never framed.

## Steps

1. **API and Security**: schema version 3 and migration, ceremonies, `PasskeyService`, use cases, endpoints, notice email, configuration (`Passkeys:ServerDomain`, `Passkeys:AllowedOrigins` in `appsettings`, the pilot deployment and [Pilot deployment](../../pilot-deployment.md)), integration tests with a software authenticator.
2. **Web app and PWA**: sign-in button, Mi perfil row, passkeys list and add form, texts, jest tests, and one e2e test of the full flow with Chromium's virtual authenticator (`When_signing_in_with_a_passkey`). The demo screenshots capture the passkey screens with the same virtual authenticator (`Passkey_screens.capture.ts`).
3. **Android app**: `react-native-passkey` (a native module: it works in the APK built by `eas build`, not in Expo Go) in `webAuthn.android.ts`, which maps its results to the same WebAuthn JSON the web sends. Android 9 (API 28) or later; older versions hide passkeys. Errors from the device that are not a cancellation (no passkey on the phone, a refused request) show the same banner as a refused passkey. `scripts/writeAndroidAssetLinks.js` writes `/.well-known/assetlinks.json` in the web export from `ANDROID_SIGNING_CERT_SHA256`, the apex `laztty.com` serves it, and the API allows the `android:apk-key-hash:` origin. iOS stays hidden (`webAuthn.ts`) until there is an iOS app (`apple-app-site-association`, associated domains).

## Tests

Integration tests (`ClassManager.Api.I.Tests`, `Endpoints/Passkeys`) use `SoftwareAuthenticator`, a platform authenticator in memory that creates real WebAuthn attestations and assertions (ES256, `none` attestation), so the API runs Identity's real checks:

- `When_a_passkey_is_added_with_the_right_password/Then_it_signs_in`
- `When_a_passkey_is_added_with_a_wrong_password/Then_returns_400`
- `When_a_registration_is_finished_by_another_account/Then_it_is_refused`
- `When_a_passkey_of_another_account_is_registered_again/Then_it_is_refused`
- `When_a_sign_in_ceremony_is_used_twice/Then_the_second_is_refused`
- `When_a_sign_in_ceremony_expired/Then_it_is_refused`
- `When_the_origin_is_not_allowed/Then_the_sign_in_is_refused`
- `When_the_device_does_not_verify_the_user/Then_the_sign_in_is_refused`
- `When_the_account_is_locked_out/Then_the_passkey_sign_in_is_refused`
- `When_a_team_member_signs_in_with_a_passkey/Then_the_session_starts_in_their_business`
- `When_a_student_signs_in_with_a_passkey/Then_the_session_is_a_student_session`
- `When_signing_in_with_a_passkey_from_the_web/Then_the_refresh_token_is_a_cookie`
- `When_a_passkey_is_removed/Then_it_no_longer_signs_in`
- `When_a_passkey_of_another_account_is_removed/Then_returns_404`
- `When_passkeys_are_listed/Then_only_the_accounts_own_are_shown`
- `When_a_passkey_is_added/Then_the_account_gets_a_notice_email`
- `When_the_account_with_a_passkey_is_deleted/Then_its_passkeys_are_deleted`
- `When_a_passkey_sign_in_fails/Then_a_security_event_is_written`

Unit tests (`ClassManager.Core.U.Tests`): the existing sign-in tests run through `SignInSessionService`, and the passkey use cases are tested with fakes (missing password, name too long, missing credential, passkey not found, a verified passkey starts the session).

App tests (`app/src/features/passkeys/__tests__`, `authentication/__tests__`, `account/__tests__`): the sign-in button only shows when passkeys are supported, a passkey sign-in starts the session, cancelling the prompt shows nothing, a refused passkey shows the banner, Mi perfil opens the passkeys screen, the empty list points to the floating button, adding sends the password and the name, a wrong password shows under the field, the add button stays grey until the form is filled, and removing asks first and nothing happens until the person confirms.

E2E (`e2e/tests/When_signing_in_with_a_passkey`): add a passkey from Mi perfil, sign out, and sign in with it, with Playwright's CDP `WebAuthn.addVirtualAuthenticator` (user verification on).

## Later

- Offer to add a passkey right after a password sign-in on a device that supports it.
- Passkeys in the browser's email autofill (conditional UI, `autocomplete="username webauthn"`).
- Accounts created with a passkey and no password, once recovery without a password is designed.
