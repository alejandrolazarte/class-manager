# Legal texts and acceptance

Date: 2026-10-13

The app publishes three legal texts and records that every account accepted the current version. Not legal advice: a lawyer reviews the texts before the app opens to more businesses.

## The texts

| Text | Route | What it covers |
|---|---|---|
| Aviso legal | `/legal/notice` | Who runs Laztty (name, tax id, address, contact), intellectual property, liability, governing law. Required in Spain by the LSSI. |
| Términos de uso | `/legal/terms` | The service, accounts and minimum ages, data a business loads, allowed use, price, availability, closing an account or a business, changes, and the data processing agreement (Laztty as processor of the business's data, GDPR art. 28). |
| Política de privacidad | `/legal/privacy` | Who is controller of what, which data, purposes and legal bases, minors, where data lives and the providers, retention, rights and the authorities (AEPD, AAIP), security, cookies and local storage. |

- The bodies live in `app/src/features/legal/legalDocuments.ts`, not in `es.json`: they are long, versioned documents, and splitting them into translation keys would make them hard to review as a whole. Titles and the rest of the UI copy are in `es.json` (`legal.*`).
- The holder's data is in one place, `app/src/features/legal/legalHolder.ts`. **It still has placeholders** (`[NOMBRE DEL TITULAR]`, `[NIF / CUIT]`, `[DOMICILIO]`, `[EMAIL DE CONTACTO]`, `[LEY Y TRIBUNALES APLICABLES]`): fill them before the texts are shown to anyone outside the pilot.
- They are public (outside the signed-in layouts), linked from the welcome screen, from Mi perfil (team and student app) and from every consent checkbox.

## Acceptance

Accepting is required wherever an account is created, and the server rejects the request otherwise (`auth.legal_terms_not_accepted`):

| Where | Request field |
|---|---|
| Owner sign-up (step 2) | `RequestSignUpCommand.AcceptsLegalTerms`, kept on `PendingSignUps` until the email link completes the sign-up |
| Accepting a team invitation without an account | `AcceptInvitationCommand.AcceptsLegalTerms` |
| Accepting a student app invitation without an account | `AcceptStudentAppInvitationCommand.AcceptsLegalTerms` |

An existing account accepting an invitation is not asked again.

Each acceptance is a row in `LegalTermsAcceptances` (`UserId`, `Version`, `AcceptedOn`), the proof of what each person accepted and when (GDPR art. 7.1 asks to be able to show the consent):

- **Append only.** Accepting a new version adds a row; old rows stay, so the history is never lost. A unique index on `UserId` + `Version` keeps one row per version, and accepting a version already accepted adds nothing.
- **In the app database, not in `identity.AspNetUsers`.** Which texts exist and which version is current is a Laztty rule, and `src/Security` holds no business concepts (ARCH003). The account itself stays in `Security`; the row only points to it by `UserId`.
- **Not tenant owned.** The terms are Laztty's, not a business's, and an account can belong to several businesses or none ([Student accounts](student-accounts.md)), so one acceptance covers all of them.
- **Created in the same transaction as the account.** Sign-up keeps the acceptance on the `PendingSignUps` row (`AcceptedLegalTermsVersion`, `AcceptedLegalTermsOn`) until the email link completes it; invitations add the row when they create the account.

## Changing the texts

1. Edit the texts and `legalTermsUpdatedOn` (`app/src/features/legal/legalHolder.ts`).
2. Set `LegalTerms.CurrentVersion` (`src/Core/Domain/Accounts/LegalTerms.cs`) to the same date.

`GET /api/me/account` answers `hasAcceptedCurrentLegalTerms`. When it is `false` (accounts created before this change, or after a new version), `LegalTermsGate` shows "Actualizamos los términos" instead of the app, for the team and the student app, until the person accepts (`POST /api/me/account/legal-terms-acceptance`) or signs out. Only a change people must agree to needs a new version; fixing a typo does not.

## Still pending

- Real holder data in `legalHolder.ts`, and a lawyer's review.
- Closing a business inside the app; today the texts say it is requested by email. "Eliminar mi cuenta" is in Mi perfil, and the terms and the privacy policy say so (see [Student accounts](student-accounts.md)). Deleting an account keeps its `LegalTermsAcceptances` rows, the proof of the consent it gave.
- Anonymizing soft deleted students after a retention period ([Deleting data](deleting-data.md) keeps them forever, which is not erasure under the GDPR).
