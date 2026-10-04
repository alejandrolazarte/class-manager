# Notifications library

Date: 2026-10-02

`src/Notifications` and `src/Notifications.Delivery` send emails and web push messages for any app. They know email layouts, brands, SMTP, VAPID and push encryption, never businesses, orders or students, so they can be reused like the [tenancy](tenancy.md), [security](security.md) and [import and export](import-export.md) libraries. ARCH006 keeps them free of `ClassManager.Core`, `Infrastructure`, `Api`, `Security`, `Tenancy` and `ImportExport` (see [analyzers](analyzers.md)).

## Contents

### `src/Notifications` (no package dependencies)

| Folder | Types | Purpose |
|---|---|---|
| `Email` | `EmailContent`, `EmailLine`, `EmailNote`, `EmailAction` | What an email says: eyebrow, title, intro, items and total, note, button, footer note. `ToPlainText()` is the `text/plain` part |
| `Email` | `EmailBrand`, `EmailPalette` | Who it comes from: name, main and accent colors, logo. `EmailPalette` derives AA-readable colors like the app's `deriveBrandTheme` |
| `Email` | `BrandedEmailHtml`, `Templates/BrandedEmail.html` | Renders content and brand into the HTML layout (embedded resource, HTML-encoded single-pass placeholders) |
| `Email` | `IEmailTransport`, `OutgoingEmail`, `EmailInlineImage` | The boundary to whatever sends the email |
| `WebPush` | `PushMessage`, `PushTarget`, `PushDelivery`, `IWebPushSender` | One push message, one subscription, and the result (`Delivered`, `Gone`, `Failed`) |
| `WebPush` | `VapidOptions`, `VapidKeys`, `VapidAuthorization`, `WebPushEncryption`, `EcKeys` | VAPID keys and tokens (RFC 8292) and payload encryption (RFC 8291, `aes128gcm`) |

`Core` references this project for `EmailContent`, so use cases describe emails without knowing about HTML or SMTP.

### `src/Notifications.Delivery` (MailKit, ASP.NET Core shared framework)

| Type | Purpose |
|---|---|
| `SmtpEmailTransport`, `SmtpOptions` | Sends `OutgoingEmail` through SMTP with MailKit: text and HTML parts, inline images by `cid` |
| `LoggingEmailTransport` | Used when `Email:Smtp:Host` is empty: logs the email instead of sending it |
| `WebPushSender` | Encrypts and posts one push message, and reports `Gone` for expired subscriptions |
| `NotificationsServiceCollectionExtensions` | `AddEmailTransport()` (binds `Email:Smtp`, picks SMTP or logging) and `AddWebPushSender()` (binds `WebPush:Vapid`, typed `HttpClient`) |

## What stays in the app

| In `Core` | In `Infrastructure` |
|---|---|
| `IEmailSender`, `EmailMessage` (content + `BusinessId`), `IWebAppLinks`, the texts of invitation and password emails | `BrandedEmailSender` (reads the business brand, renders, hands the email to the transport), `EmailBrandReader`, `WebAppLinks`, `OrderEmails`, push jobs, outbox, dispatcher, subscriptions and who gets each notification |

The rule of thumb: the library decides **how** an email or push is built and delivered; the app decides **who** gets it, **what** it says and **which brand** it carries.

## When emails are sent

This is how emails are sent today. It reflects the current needs and can change when they do; the best option will be weighed then.

- **Emails that are one more notice** (orders: placed, paid, ready, cancelled) are sent in the background, so a slow mail server never holds up the screen. If one is lost, the same event still reaches people through the in-app notifications, a push or the order status in the app. Failures are logged (`Email not sent: {Subject}`).
- **Emails that are the only channel** (team and student invitations, password reset) are sent within the request, so the screen knows whether the email went out and the user can send it again.

## Using it in another app

1. Reference `Notifications.Delivery` (it brings `Notifications`).
2. Call `services.AddEmailTransport()` and, for push, `services.AddWebPushSender()`; configure `Email:Smtp` and `WebPush:Vapid`.
3. Build an `EmailContent` and an `EmailBrand`, render with `BrandedEmailHtml.Render`, and send an `OutgoingEmail` with `IEmailTransport`. For push, send a serialized `PushMessage` to each `PushTarget` with `IWebPushSender` and delete the subscriptions that answer `Gone`.

## Tests

`tests/ClassManager.Notifications.U.Tests`: rendering (initials, HTML encoding, items before total), color contrast, plain text, VAPID token, RFC 8291 encryption example and the `Gone` answer. Delivery through the API (brand color, logo, sender name, every email and push flow) stays in `tests/ClassManager.Api.I.Tests`.
