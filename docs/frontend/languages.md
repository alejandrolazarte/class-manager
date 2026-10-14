# Languages

The app's UI is available in Spanish (`es`, the default) and English (`en`). Each person picks the language in **Ajustes → Tu teléfono → Idioma**, in the team app and in the student app. A business with English-speaking students doesn't need to change anything: the students choose English on their own phones.

## Where the texts live

- `app/src/i18n/es.json` and `app/src/i18n/en.json` hold every user-facing text, with the same keys.
- `translate(key)` reads the file of the current language. Code never picks a file itself.
- `languages.ts` lists the languages and types `en.json` as a `Record` of every Spanish key, so a key missing in English breaks `pnpm typecheck`.
- `src/i18n/__tests__/When_a_language_is_added/` fails when a language has a key Spanish doesn't, or a text with different `{placeholders}`.
- The translation keys test ([unused code](unused-code.md)) still runs on `es.json`; since every language has the same keys, removing a key from Spanish means removing it from every language.

Adding or changing a text means writing it in both files in the same pull request.

## The choice is per device

- The language is stored on the device like the light/dark appearance: `localStorage` on web, `SecureStore` on Android and iOS (`languagePreferenceStorage*.ts`, key `class-manager.language`). It isn't sent to the API, so it doesn't follow the person to another phone.
- `LanguageProvider` (in `app/_layout.tsx`) reads it before the navigator renders, so the splash screen covers the restore and nothing flashes in Spanish first.
- Changing the language remounts everything under `LanguageProvider`, so every screen renders again with the new texts. On web the URL keeps the current screen; it was checked by switching on `/forgot-password` and reloading. On web it also sets `<html lang>` for screen readers.

## What stays in Spanish

- **Emails** (invitations, password reset, receipts): they are rendered by the API from `src/Notifications`, which doesn't know the reader's language. The language hint in the app says so.
- **Legal texts** (`legalDocuments.ts`): the terms of use and the privacy policy are legal documents for Argentina and Spain; translating them is a legal decision, not a UI one. Their titles are translated.
- **Import and export column names**: the API defines them, so hints keep naming the column `Responsable`.
- **Numbers and money** keep the `es-AR` format (`1.234,50`) in both languages.
- Data typed by the business (class names, products, announcements) is shown as typed.

## Adding a language

1. Add the code to `languages` and its file to `translationsByLanguage` in `languages.ts`.
2. Create `<code>.json` with every key (typecheck lists the missing ones).
3. Add `settings.language.<code>` to every file, written in that language's own name ("Português"), and to `languageLabelKeys` in `languageValue.ts`.
