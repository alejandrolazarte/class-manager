# Content Security Policy (web app)

Date: 2026-10-03

The web app (and the installed PWA) sends a Content Security Policy that tells the browser where scripts, styles, images and requests may come from. Its main job is to protect the refresh token, which the web app keeps in `localStorage` (see [running the app](running-the-app.md)): if a script were ever injected into the page, the policy keeps it from running and from sending anything to another server.

It is deployed in **report-only** mode: the browser only reports what it would block, in the developer console, and never blocks anything.

## Where it comes from

`pnpm export:web` runs `app/scripts/writeWebHeaders.js` after the export. It writes `dist/_headers`, which Cloudflare Pages applies to every path. The API origin comes from `EXPO_PUBLIC_API_BASE_URL`, the same variable the app uses, so no host is hardcoded.

## The policy

| Directive | Value | Why |
|---|---|---|
| `default-src` | `'self'` | Anything not listed below only from the app's own domain. |
| `script-src` | `'self'` | Only the app's own script files. No inline scripts and no `eval`. |
| `style-src` | `'self' 'unsafe-inline'` | `react-native-web` injects `<style>` tags at runtime. |
| `img-src` | `'self' data: blob:` | The business logo is fetched from the API and shown as a `data:` image. |
| `font-src` | `'self' data:` | Nunito and the icon fonts are bundled with the app. |
| `connect-src` | `'self'` + the API origin | The app only talks to its API. A stolen token can't be sent anywhere else. |
| `worker-src`, `manifest-src` | `'self'` | The service worker and the PWA manifest. |
| `object-src` | `'none'` | No plugins. |
| `base-uri`, `form-action` | `'self'` | Nobody can redirect relative links or form posts to another site. |
| `frame-ancestors` | `'none'` | The app can't be embedded in a frame (only applies once enforced). |

For this policy, the service worker registration moved from an inline `<script>` in `app/public/index.html` to `app/public/register-service-worker.js`. Its behavior is unchanged.

## What the console shows (expected)

One report is expected and harmless: **Zod** (form validation) checks once whether it may use `new Function` to compile faster validators. Under report-only it is allowed, so Zod compiles and the console shows `script-src 'eval'` reports from the main bundle. Once enforced, the check fails, Zod catches the error and validates without `eval`, with the same results. Zod's `jitless` option would remove the check, but it only works if it runs before any schema is created, which would mean changing the app's entry point for Android and iOS too.

Anything else in the console that mentions Content Security Policy is new and worth looking at.

## How it was tested

On 2026-10-03, with the policy **enforced** (not report-only) on the e2e web server and a local collector for violation reports:

- the e2e suite (`e2e/tests`) passed;
- the screenshot tour of every team, brand and family screen (`e2e/screenshots`) passed, including forms that are submitted;
- the service worker registered;
- the only violation reported was Zod's check above.

## Switching to enforcement

After the pilot has used the app for a while with no unexpected reports in the console:

1. In `app/scripts/writeWebHeaders.js`, change the header name from `Content-Security-Policy-Report-Only` to `Content-Security-Policy` and update its tests.
2. Deploy and open the app on Android and iPhone (installed PWA): sign in, open a few screens, submit a form.

To roll back, change the header name back. The policy itself doesn't change.

## Refresh token in a cookie (not done)

The stronger fix is to keep the web refresh token in an `HttpOnly` cookie that scripts can't read. It needs the app and the API on the same site (for example `app.example.com` and `api.example.com`): today they are on `pages.dev` and the Azure Container Apps domain, and Safari on iPhone blocks cross-site cookies, which would break the installed PWA. Do it together with a custom domain.
