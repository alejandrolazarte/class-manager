# Languages

The app's UI is available in Spanish (`es`, the default) and English (`en`). Each person picks the language in **Ajustes → Tu teléfono → Idioma**, in the team app and in the student app, and it is saved to their account. A business with English-speaking students doesn't need to change anything: the students choose English in their own app.

## Where the texts live

- `app/src/i18n/es.json` and `app/src/i18n/en.json` hold every user-facing text, with the same keys.
- `translate(key)` reads the file of the current language. Code never picks a file itself.
- `languages.ts` lists the languages and types `en.json` as a `Record` of every Spanish key, so a key missing in English breaks `pnpm typecheck`.
- A key built at runtime (`products.stock.kind.${kind}`) can still miss: `translate` then returns the key itself, so the screen shows the key instead of crashing. Seeing a key on screen means a translation is missing.
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

## Emails

- Every email text lives in code, in `src/Core/Emails`: `IEmailTexts` has one method per email (subject and content, with typed parameters), implemented by `SpanishEmailTexts` and `EnglishEmailTexts`. A language missing an email doesn't compile, and `EmailTexts.All` must cover `AccountLanguage.Supported` (unit test).
- They are in code and not in the database because nobody edits them without a deploy: they change in the same pull request as the code that fills them, are reviewed and tested, and are rendered by `tools/EmailPreviews` in both languages (English files end in `-en`).
- The layout's own texts (the link fallback under the button, "Total", `<html lang>`) come from `EmailLayoutTexts`, which each language sets on the content.
- Who gets which language: an email to an account uses that account's `PreferredLanguage` (`RecipientEmailTexts`): password reset, the "you already have an account" email, email change confirmation and notice, and every order email (each team member or student account gets its own language). Unknown codes fall back to Spanish.
- Still in Spanish, on purpose for now: invitations (team, student app, guardian consent) and the sign-up confirmation. They go to people who may not have an account yet; once they choose English, every later email reaches them in English.

## Push notifications and team notices

- The texts the system writes around a notification live in `src/Infrastructure/Notifications`: `INotificationTexts`, implemented by `SpanishNotificationTexts` and `EnglishNotificationTexts` ("Se suspende {clase}" / "{clase} is cancelled", "{alumno} avisó que no viene", weekdays and dates). Links stay in `StudentAppPushLinks` and `TeamNotificationLinks`.
- What the team writes is shown as written, in whatever language they wrote it: announcements (title and body), class comments and the reason for a cancelled class. Nothing is machine translated: the team doesn't see what reaches the student, and a wrong translation of a time or a place is worse than the original. A business with English-speaking students can write those texts in English, or in both languages.
- Each notification is built in every language (`LocalizedPushMessage`). The push job carries all of them (`PushJob.MessagesByLanguage`, with `Message` as the Spanish default for jobs queued before this change), and `PushDispatcher` sends each subscription the message in its account's `PreferredLanguage`.
- Team notices ("Avisos") are stored as text, one row per person, so `TeamNotifier` writes each row in that person's language. A notice already stored keeps the language it was written in.

## What stays in Spanish

- **Legal texts** (`legalDocuments.ts`): the terms of use and the privacy policy are legal documents for Argentina and Spain; translating them is a legal decision, not a UI one. Their titles are translated.
- **Import and export column names**: the API defines them, so hints keep naming the column `Responsable`.
- **Numbers and money** keep the `es-AR` format (`1.234,50`) in both languages.
- Data typed by the business (class names, products, announcements) is shown as typed.

## Adding a language

1. Add the code to `languages` and its file to `translationsByLanguage` in `languages.ts`.
2. Create `<code>.json` with every key (typecheck lists the missing ones).
3. Add the code to `AccountLanguage.Supported` in `src/Core/Domain/Accounts/AccountLanguage.cs`.
4. Add a `<Language>EmailTexts` class implementing `IEmailTexts` and list it in `EmailTexts.All`.
5. Add `settings.language.<code>` to every file, written in that language's own name ("Português"), and to `languageLabelKeys` in `languageValue.ts`.
