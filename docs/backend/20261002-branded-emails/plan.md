# Plan — Branded emails

Date: 2026-10-02

Follow-up to the Claude Design exploration "Mail Invitacion" and "Mail Pedido". Every email the app sends now uses one HTML layout with the business brand (logo or initials, main color, accent) and keeps a plain-text alternative. Brand settings come from [brand plan](../../frontend/20260930-brand/plan.md).

## Before

Emails were plain text built by string concatenation in C# (`EmailBody(...)` in each use case and `OrderEmails` in Infrastructure), sent by MailKit as `text/plain`. No Razor views and no HTML files.

## Emails and their content

| Email | Sent by | Brand | Layout |
|---|---|---|---|
| Team invitation (and resend) | `InviteMemberUseCase`, `ResendInvitationUseCase` | Business | Button first, note "El link sirve una sola vez…", link fallback |
| Family invitation | `InviteFamilyUseCase` | Business | Same |
| Password reset (not in the design) | `RequestPasswordResetUseCase` | App (`Class Manager`, default color) because the request has no business | Same |
| New order (team) | `OrderNotificationService.OrderPlacedAsync` | Business | Items and total, "Entrega" note, button "Abrir pedidos" |
| Payment confirmed, order ready, order cancelled (unpaid or by the branch) (family) | `OrderNotificationService` | Business | Items and total, note, button "Ver pedido en la app" |

Order emails show the order number: "Pedido n.º 1043" above the title for families, "Nuevo pedido · n.º 1043" and the subject "Nuevo pedido n.º 1043 de …" for the team (see [domain model](../domain-model.md#order-and-orderline-added-with-the-shop)).

## Decisions

- **Content is structured, not text.** Core builds an `EmailContent` (eyebrow, title, intro, items, total, note, action, footer note) and sets `EmailMessage.BusinessId` when the email belongs to a business. `EmailContent.ToPlainText()` produces the `text/plain` part, with each link on its own line.
- **Rendering lives in the [notifications library](../../notifications.md)** since 2026-10-02 (`EmailContent`, `BrandedEmailHtml`, `EmailPalette`, `IEmailTransport`); the app keeps the brand lookup. `BrandedEmailSender` (the `IEmailSender`) reads the brand with `EmailBrandReader`, renders HTML with `BrandedEmailHtml` and hands an `OutgoingEmail` to the `IEmailTransport` (SMTP, or logging when SMTP is not configured). Core never sees HTML.
- **The layout is an HTML file**, `src/Notifications/Email/Templates/BrandedEmail.html`, embedded in the assembly. Repeated or optional blocks (button, note, item rows, link fallback, logo or initials) are small fragments in `BrandedEmailHtml`. Placeholders `{{Name}}` are filled in a single pass and every value is HTML-encoded, so a business name can't inject markup or another placeholder.
- **Why not Razor:** the emails are a single layout with a few optional blocks; Razor (`HtmlRenderer` or RazorLight) adds a dependency and a rendering pipeline for no gain. Revisit if emails grow loops and conditionals beyond the current blocks.
- **Colors follow the app's derivation.** `EmailPalette` ports `deriveBrandTheme`: the main color is darkened until white text passes WCAG AA (4.5:1), the soft note background is a tint of the main color, and the eyebrow uses the accent (or the main color without one). Without a brand color it uses the default `#0076b4` (lagoon600).
- **The logo is an inline attachment** (`cid:brand-logo@class-manager`), not a URL: the logo endpoint needs the access token and there is no public storage. Without a logo the header shows the brand initials on a white tile, as in the design.
- **The sender name is the brand name** (`From: Df Swimming Valencia <configured address>`). The address stays `Email:Smtp:FromAddress`; the app brand uses `Email:Smtp:FromName`.
- The brand is read with `IgnoreQueryFilters` by the explicit business id, because the password reset and background jobs may run without a tenant; the id always comes from the server.

## Tests

- Integration: the brand color reaches the header, the logo is embedded, the sender is the brand name, the password reset uses the app brand, and every existing email test now goes through the real renderer (the test factory replaces `IEmailTransport`, not `IEmailSender`).
- Rendering: initials without logo, HTML encoding, a too-light color is darkened until white text reads, items come before the total.
- Unit: the plain-text body keeps each link on its own line.
