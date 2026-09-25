# Running the Expo app

The app in `app/` (Expo + Expo Router + NativeWind) runs directly on the host, like the API.

## Commands

```powershell
cd app
pnpm install
pnpm expo start            # Metro + web on http://localhost:8081; scan the QR code with Expo Go
pnpm expo start --tunnel   # phone on another network
pnpm typecheck
pnpm test
pnpm lint                  # ESLint (eslint-config-expo + project rules), fails on warnings
pnpm format:check          # Prettier; pnpm format to fix
```

The API must be running (`dotnet run --project src/Api/Api.csproj`), see [Local development](../backend/local-development.md).

## Configuration

`app/.env.example` lists the public variables. Copy it to `app/.env.local` (git-ignored) to override them.

| Variable | Default | Purpose |
|---|---|---|
| `EXPO_PUBLIC_API_BASE_URL` | `http://localhost:5000` | Base URL of the API. On a phone use the host's LAN IP, for example `http://192.168.1.20:5000` |

The business id, time zone and currency no longer come from configuration: the business comes from the access token, and `BusinessProvider` loads the time zone, currency and name from `GET /api/business` after sign in.

## Signing in

Sign up from the app (it creates the owner and the business), or sign in with the demo owner `owner@demo.local` created by the seed script, see [Local development](../backend/local-development.md).

Session handling:

- The access token lives in memory only (`src/api/authenticationSession.ts`).
- The refresh token is persisted by `src/features/authentication/sessionStorage.ts`: `expo-secure-store` on Android and iOS, `localStorage` on web (`sessionStorage.web.ts`). On web any injected script can read it; that is accepted because access tokens are short-lived and refresh tokens rotate.
- `httpClient` sends `Authorization: Bearer <accessToken>`. On `401` it refreshes once (concurrent `401`s share one refresh) and retries; if the refresh is rejected the session ends and the app goes back to sign in.
- Sign out revokes the refresh token (best effort), clears storage and the TanStack Query cache.

## Tests

Behavior tests use `jest-expo` + `@testing-library/react-native`. They live next to each feature, in `src/features/<feature>/__tests__/When_<condition>/Then_<result>.test.tsx`, and mock the API at the `*Api.ts` module boundary. `expo-router` is mocked globally in `jest.setup.ts`. `httpClient` tests mock `fetch` directly; authentication tests also mock `sessionStorage` (see `src/testing/refreshTokenStorageMock.ts`).

`jest.config.js` sets `testTimeout` to 20 seconds: CI always starts with a cold Jest cache, and the first render of a screen transforms the React Native modules it loads lazily inside the test, which took the booking screen tests past the 5-second default.

## Lint and format

- ESLint uses the flat config in `app/eslint.config.js`: `eslint-config-expo` plus `eslint-config-prettier`, with `@typescript-eslint/no-explicit-any`, `@typescript-eslint/no-unused-vars`, `prefer-const` and `react-hooks/exhaustive-deps` as errors. `pnpm lint` runs with `--max-warnings 0`, so warnings fail too.
- Prettier uses `app/.prettierrc.json`: 2 spaces, double quotes, semicolons, trailing commas, LF, `printWidth` 100. `app/.prettierignore` skips `node_modules`, `dist`, `.expo` and `pnpm-lock.yaml`.

CI runs `pnpm install --frozen-lockfile`, `pnpm lint`, `pnpm format:check`, `pnpm typecheck` and `pnpm test --ci` in the `frontend` job of `.github/workflows/ci.yml`.

## Layout

```text
app/
  app/                    ← Expo Router routes, thin: they only render a feature screen
    (auth)/               ← sign in / sign up; redirects away when a session exists
    (tabs)/               ← signed-in area; redirects to sign in without a session
  src/api/                ← httpClient + ProblemDetails helpers
  src/features/<feature>/ ← types, API module, hooks, components, screens, tests
  src/i18n/es.json        ← user-facing copy (code stays in English)
  src/ui/                 ← shared presentational components (Button, TextField, Banner, Chip, Toast)
```
