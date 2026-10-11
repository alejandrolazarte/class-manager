# Unused code

The app keeps no file, export, dependency or translation that nothing uses. Two checks fail CI when something is left behind, so removing it does not depend on someone searching by hand (rule 5 of the [Calma redesign](20261011-calma-redesign/plan.md#5-nothing-of-the-old-design-is-left-and-nothing-unused)).

## knip

[knip](https://knip.dev) reads the project from its entry points (Expo Router routes, `jest.config.js`, `jest.setup.ts`, the tests, the scripts in `package.json`) and reports the files, exports and dependencies nobody reaches.

```powershell
cd app; pnpm knip
```

It runs in the frontend CI job. When it reports something, delete it; don't add it to the configuration to get green.

The configuration (`app/knip.json`) only says what knip cannot see on its own:

| Setting | Why |
|---|---|
| `entry`: `public/*.js` | The service worker and its registration are served as static files and loaded by the browser, not imported. |
| `entry`: `src/testing/countryFactory.ts`, `src/testing/expoRouterMock.ts` | `jest.setup.ts` loads them with `jest.requireActual("…")` inside `jest.mock` factories, which knip does not follow. A plain `require` would be followed but breaks ESLint, and an `import` cannot be used inside a hoisted `jest.mock` factory. |
| `ignoreExportsUsedInFile` | A type or constant exported and used in its own file is still in use; knip only reports exports nobody uses at all. |
| `ignoreDependencies`: `expo-system-ui`, `expo-updates` | knip's Expo plugin expects them from `app.json` (`userInterfaceStyle`, and updates that are on unless disabled). The app does not use OTA updates, and installing `expo-system-ui` would change how the Android build applies `userInterfaceStyle`: that is a decision of its own, not a cleanup. |

## Translation keys with no user

`src/i18n/__tests__/When_translations_are_loaded/Then_every_key_has_a_user.test.ts` fails when a key in `es.json` is not written anywhere in `src/` or `app/` (tests and `src/testing` don't count: a text only a test reads is never shown).

- A plural key (`x.one`, `x.other`) counts as used when its base key `x` is written, because `translateCount("x", n)` builds the form.
- Keys built at runtime are listed by prefix in the test:
  - `keysBuiltFromOneValue`: a prefix plus one value, like `` `orders.filter.${filter}` `` or `` `roles.${role}` ``. Only one more segment counts, so `roles.editor.deleteQuestion` is still checked.
  - `keysBuiltFromApiErrorCodes`: messages for error codes the API returns (`apiErrors.`, `importExport.rowError.`, `importExport.fileError.`). The app can't know which codes exist, so every key under these prefixes is kept.

When a new key is built at runtime, add its prefix to the right list. When a feature is removed, remove its keys with it.
