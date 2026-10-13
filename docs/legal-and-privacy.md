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

An existing account accepting an invitation is not asked again. The account stores `AcceptedLegalTermsVersion` and `AcceptedLegalTermsOn` (`identity.AspNetUsers`), which is the proof of what was accepted and when.

## Changing the texts

1. Edit the texts and `legalTermsUpdatedOn` (`app/src/features/legal/legalHolder.ts`).
2. Set `LegalTerms.CurrentVersion` (`src/Core/Domain/Accounts/LegalTerms.cs`) to the same date.

`GET /api/me/account` answers `hasAcceptedCurrentLegalTerms`. When it is `false` (accounts created before this change, or after a new version), `LegalTermsGate` shows "Actualizamos los términos" instead of the app, for the team and the student app, until the person accepts (`POST /api/me/account/legal-terms-acceptance`) or signs out. Only a change people must agree to needs a new version; fixing a typo does not.

## Still pending

- Real holder data in `legalHolder.ts`, and a lawyer's review.
- "Eliminar mi cuenta" inside the app and closing a business (see [Student accounts](student-accounts.md)); today the texts say both are requested by email.
- Anonymizing soft deleted students after a retention period ([Deleting data](deleting-data.md) keeps them forever, which is not erasure under the GDPR).
