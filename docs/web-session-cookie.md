# Web session cookie

Date: 2026-10-15

On the web (browser and installed PWA) the refresh token lives in an `HttpOnly` cookie that the API sets. JavaScript can't read it, so a script injected into the page can't steal it and use the session from another machine. Before this, the web app kept it in `localStorage`, where any script could read it.

This is not single sign-on: the cookie only belongs to the API's authentication routes, and the session works the same as before.

## Why it needed our own domain

The browser only sends the cookie when the app and the API are on the **same site**. Safari on iPhone blocks cross-site cookies, which would have broken the installed PWA. With `app.laztty.com` and `api.laztty.com` (see [Domain and DNS](domain-and-dns.md)) both belong to the site `laztty.com`.

## Web and mobile

| Client | Where the refresh token lives | How it reaches the API |
|---|---|---|
| Web and PWA | `HttpOnly` cookie set by the API | The browser sends it |
| Android and iOS app | `expo-secure-store` (Keystore / Keychain) | In the request body, as before |

The native app is unchanged: it has no page where a script could be injected, `expo-secure-store` is already encrypted by the operating system, and cookies with `fetch` in React Native are unreliable.

## How it works

- The web app sends `X-Refresh-Token-Cookie: true` and `credentials: "include"` on every authentication request (`app/src/api/refreshTokenCookieRequest.web.ts`). The native version sends neither.
- With that header, every response that starts or rotates a session (sign in, sign up, invitations, refresh, branch switch) sets the cookie `class-manager-refresh-token` and leaves the refresh token out of the body (`CookieTokenResponse`). Without it, the API answers as before (`TokenResponse`), so the mobile app is unaffected (`src/Api/Authentication/RefreshTokenCookie.cs`).
- Refresh, branch switch and sign out read the token from the body; when the body has none and the header is present, they read the cookie. Sign out revokes the token and deletes the cookie.
- The cookie is `HttpOnly`, `Secure`, `SameSite=Strict`, limited to `Path=/api/auth`, and expires with the refresh token (30 days).
- The web app only keeps a marker in `localStorage` (`class-manager.cookie-session`) that says a session exists, so it doesn't call `/refresh` on every visit of someone who never signed in (auth routes are rate limited).
- CORS allows credentials and the `X-Refresh-Token-Cookie` header for the allowed origins only (`Cors:AllowedOrigins`).

## Cross-site requests (CSRF)

The browser sends the cookie on its own, so the API must not accept it from another site:

- `SameSite=Strict`: the browser doesn't send it on requests started by another site.
- The API only reads the cookie when the request carries the `X-Refresh-Token-Cookie` header. A form or link from another site can't add a header, and a script on another site can't send it because CORS only allows `app.laztty.com` to.
- The access token still travels as `Authorization: Bearer`, which a browser never adds by itself.

## Sessions from before the change

A web session started before this change still has its refresh token in `localStorage` under `class-manager.refresh-token`. On the next refresh the web app sends it in the body once, with the header; the API answers with the cookie and the app deletes the old key. Nobody has to sign in again.

## Local development and e2e

Browsers treat `http://localhost` as secure, so the `Secure` cookie works without HTTPS, and `localhost:8081` and `localhost:5000` are the same site (ports don't count). The e2e test `When_the_page_is_reloaded` checks that the session survives a reload and that the cookie is `HttpOnly`.
