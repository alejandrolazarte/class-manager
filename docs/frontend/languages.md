# Languages

The app's UI is available in Spanish (`es`, the default) and English (`en`). Each person picks the language in **Ajustes → Tu teléfono → Idioma**, in the team app and in the student app, and it is saved to their account. A business with English-speaking students doesn't need to change anything: the students choose English in their own app.

## Where the texts live

- `app/src/i18n/es.json` and `app/src/i18n/en.json` hold every user-facing text, with the same keys.
- `translate(key)` reads the file of the current language. Code never picks a file itself.
- `languages.ts` lists the languages and types `en.json` as a `Record` of every Spanish key, so a key missing in English breaks `pnpm typecheck`.
- `src/i18n/__tests__/When_a_language_is_added/` fails when a language has a key Spanish doesn't, or a text with different `{placeholders}`.
- The translation keys test ([unused code](unused-code.md)) still runs on `es.json`; since every language has the same keys, removing a key from Spanish means removing it from every language.

Adding or changing a text means writing it in both files in the same pull request.

## The choice belongs to the account

- The language is a column of the account: `identity.AspNetUsers.PreferredLanguage` (`nvarchar(10)`, not null, default `es`, migration `AddUserPreferredLanguage`). It lives there, next to `FullName` and `BirthDate`, because it belongs to the person, not to a business: the same account can be team in one business and student in another, and reads both in one language. It isn't a business concept, so it doesn't break ARCH003.
- `GET /api/me/account` returns it as `preferredLanguage`; `PUT /api/me/account/language` with `{ "preferredLanguage": "en" }` changes it (`UpdateMyLanguageUseCase`). The supported codes are `AccountLanguage.Supported` in `src/Core/Domain/Accounts`; anything else is a validation error.
- Choosing a language in Ajustes saves it to the account first and only then switches the app. If the request fails, the app stays in its language and shows an error toast.
- `AccountLanguageSync` (in the team and student layouts) applies the account's language when it differs from the device's, so signing in on a new phone shows the person's language.
- The device also keeps a copy (`localStorage` on web, `SecureStore` on Android and iOS, key `class-manager.language`, `languagePreferenceStorage*.ts`), because the screens before signing in (welcome, sign in, password reset) have no account to read. Signing in replaces it with the account's.
- `LanguageProvider` (in `app/_layout.tsx`) reads the device copy before the navigator renders, so the splash screen covers the restore and nothing flashes in Spanish first.
- Changing the language remounts everything under `LanguageProvider`, so every screen renders again with the new texts. On web the URL keeps the current screen; it was checked by switching on `/forgot-password` and reloading. On web it also sets `<html lang>` for screen readers.

## What stays in Spanish

- **Emails** (invitations, password reset, receipts): they are rendered by the API from `src/Notifications` with Spanish templates only. The account's `PreferredLanguage` is the input for translating them later; invitations to people without an account will also need a language (from the invitation or a business default). The language hint in the app says so.
- **Legal texts** (`legalDocuments.ts`): the terms of use and the privacy policy are legal documents for Argentina and Spain; translating them is a legal decision, not a UI one. Their titles are translated.
- **Import and export column names**: the API defines them, so hints keep naming the column `Responsable`.
- **Numbers and money** keep the `es-AR` format (`1.234,50`) in both languages.
- Data typed by the business (class names, products, announcements) is shown as typed.

## Adding a language

1. Add the code to `languages` and its file to `translationsByLanguage` in `languages.ts`.
2. Create `<code>.json` with every key (typecheck lists the missing ones).
3. Add the code to `AccountLanguage.Supported` in `src/Core/Domain/Accounts/AccountLanguage.cs`.
4. Add `settings.language.<code>` to every file, written in that language's own name ("Português"), and to `languageLabelKeys` in `languageValue.ts`.
