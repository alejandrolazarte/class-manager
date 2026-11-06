# Notification preferences

Date: 2026-11-05

Status: agreed with the owner on 2026-11-05. Step 1 built, first per account, then per business (decided by the owner the same day); step 2 (unsubscribe from the email) next. Comes before the [automatic reminders](../20261021-automatic-reminders/plan.md), so the new reminder emails and pushes can be turned off from the day they ship.

## Why

Nobody can choose which emails or pushes they get. A family can only turn push off on the device, which stops everything: announcements, cancelled classes and a ready order alike. A team member who handles orders gets an email and a push for every order and can't turn off one of them. Reminders are about to add more emails and pushes, and being nagged is what makes people mute an app entirely, or mark its emails as spam, which hurts delivery for every business on our domain.

Decided by the owner:

- **By kind**: each person chooses which kinds of email and push they get. Account and security emails can't be turned off.
- **Per business**: set in Mi perfil for the business the person is in. Someone in two businesses (an instructor in two studios, a parent with children in two schools) chooses in each; most people belong to one and see no difference. First built per account; changed the same day, because each email comes from one business and an unsubscribe link should stop that business's emails only.
- **One-click unsubscribe** at the foot of every email that can be turned off, plus the `List-Unsubscribe` headers.
- **Push by kind** too, on the same screen.

## Categories

What can be turned off is a category, not each single message. Categories group what a person thinks of as one thing.

### Always sent (not a category)

| Message | Why it can't be turned off |
|---|---|
| Sign-up confirmation, account already exists, password reset, email change confirmation, email changed notice, passkey added | Security and access to the account |
| Team and student app invitations | Sent to someone who has no account yet |
| Guardian consent request (email and push), photo approval request (push) | They ask an adult to decide something about a minor; without them the minor stays blocked |

### Family and student accounts

| Category | Email | Push | Today | Later |
|---|---|---|---|---|
| **Pedidos** | Paid, ready, cancelled | Ready | ✓ | |
| **Novedades** | | Announcement published, announcement reminder | ✓ | |
| **Cambios en las clases** | | Class cancelled, class changed (time, substitute) | ✓ | |
| **Comentarios del profe** | | Feedback left | ✓ | |
| **Recordatorios de clases** | | Class reminder | | Reminders step 2 |
| **Recordatorios de pago** | Plan due, plan overdue, reminder sent by hand | The same, plus pack about to expire and classes owed | | Reminders steps 1 and 3 |

### Team accounts

| Category | Email | Push (and aviso) | Today |
|---|---|---|---|
| **Pedidos nuevos** | Order placed | Order placed | ✓ |
| **Asistencia** | | A student won't come, a makeup booked, a class booked with a pack | ✓ |
| **Planes de alumnos** | | A student chose or cancelled a plan from the app | ✓ |
| **Invitaciones** | | An invitation was declined, a guardian refused consent | ✓ |

- **Avisos (the team's in-app list) always keep everything.** A team preference turns off the push and the email, not the record: the aviso still shows in Avisos, so turning a push off never hides that something happened. Avisos are a list the person opens, not an interruption.
- **The family app has no such list today**, so for family categories turning off the push means not hearing about it until the app is opened, where the class, the order or the announcement is shown anyway.

### Defaults

Everything is **on** for every account, existing and new, so nothing changes the day this ships. Only what a person turns off is stored.

## Decisions

### Stored per account and business

- Table `NotificationOptOuts`: `TenantId` (the business), `AccountId` (the `identity.AspNetUsers` id), `Category`, `Channel` (`Email`, `Push`) and the audit columns. Primary key `(TenantId, AccountId, Category, Channel)`.
- **A row means "off"**. Turning a category back on deletes the row (physical delete: the row has no value once gone, like push subscriptions, see [deleting data](../../deleting-data.md)).
- **Tenant-owned** (`ITenantOwned`, `IAuditable`), so the tenant query filter keeps each business's choices apart; the tenancy test proves business A can't read business B's rows. Within a business, the endpoints only read and write the signed-in account's rows, and senders read the rows of the accounts they are about to send to (pushes and emails already run in the business they come from).
- Migration `NotificationOptOutsPerBusiness` copied each choice made per account to every business where it applies: team categories to the businesses where the person is on the team, family categories to those where they have family access; choices of people in no business were dropped. Checked on SQL Server with rows written by the previous version.
- It lives in the app (`Infrastructure`, default schema), not in `src/Security`: categories are business concepts, and `Security` stays free of them (ARCH003).
- Deleting the account deletes its rows in every business.

### One place decides

- `Category` and `Channel` are enums in `Core` (`Core/Domain/Notifications`), with the "always sent" messages having no category at all, so a new message has to pick a category or be explicitly always sent.
- `INotificationPreferencesService.WhoWantsAsync(accountIds, category, channel)` returns the accounts that still want it. Nothing filters by hand:
  - **Emails** are filtered when they are queued: `OrderNotificationService` asks before building the recipients of the order emails (Pedidos for the family, Pedidos nuevos for the team).
  - **Pushes** carry their category in the push task (`PushJob.Category`, set by every `PushPublisher` and `TeamNotifier` call), and `PushDispatcher` drops the subscriptions of accounts that turned it off when the task runs. That is where the recipients of a push are resolved anyway (an announcement for everyone only knows its accounts then). A push with no category (guardian consent, photo approval) always goes out.
  - `TeamNotifier` always saves the aviso; only its push is filtered.

### Unsubscribe from the email

- Every email with a category ends with a line in the footer, **"Dejar de recibir estos emails"** ("Stop getting these emails"), and carries `List-Unsubscribe` and `List-Unsubscribe-Post: List-Unsubscribe=One-Click` headers (RFC 8058), which Gmail and Outlook show as their own unsubscribe button and expect from senders of repeated emails.
- The link carries a token protected with ASP.NET Data Protection (purpose `email-unsubscribe`, payload: business, account id and category, no expiry: an old email must still work). No sign-in needed.
- `/unsubscribe?token=…` in the web app names the category and asks with one button, **Dejar de recibirlos**. The page never turns anything off by just loading, because mail scanners open links. After the button: "Ya no vas a recibir emails de {categoría}", with **Volver a recibirlos** (undo, as every reversible action) and a link to Mi perfil → Notificaciones.
- `POST /api/notification-preferences/unsubscribe` with the token turns it off. The one-click header and the page's button both post here.
- `src/Notifications` gains an optional unsubscribe link in `EmailContent` (rendered in the branded layout's footer and in `ToPlainText()`) and optional headers in `OutgoingEmail`. The library still knows nothing about categories: the app builds the link.

### Recipients without an account

Every email that can be turned off today goes to an account. Payment reminders will also email clients who never opened the app ([reminders](../20261021-automatic-reminders/plan.md#payment-reminders)). For them the token carries the client's email address instead of an account id, and the opt-out is stored by normalized email address in the same table (a nullable `EmailAddress` with a check that exactly one of `AccountId` and `EmailAddress` is set). If that person later creates an account with that address, their address opt-outs move to the account. This is built with the reminders step that first emails clients without an account, not before.

## API

| Operation | Endpoint | Who |
|---|---|---|
| Read the signed-in account's preferences in the current business: its name and the categories of the current session (team or family), each with its channels and on/off | `GET /api/me/notification-preferences` | Any signed-in account |
| Turn one category and channel on or off | `PUT /api/me/notification-preferences` with `{ category, channel, isEnabled }`; a channel the category doesn't use is refused (`notifications.channel_not_in_category`) | Any signed-in account |
| Unsubscribe from an email link | `POST /api/notification-preferences/unsubscribe` with `{ token }`, and `POST …/resubscribe` for the undo | Anonymous, the token is the proof |

Which categories apply: the team categories in a team session, the family categories in a student app session. An owner who is also a parent sets the family ones from the student app of the business where they are a parent.

## App

- **Ajustes → Notificaciones** in both apps (decided by the owner on 2026-11-06; it first lived in Mi perfil). The row keeps showing whether push is on in this device, is always visible (emails can be chosen even where push is not available) and opens one screen: **En este dispositivo** first (the push switch and its hint, the blocked or unsupported message, nothing when push can't be offered), then the business's categories. The device push sheet is gone. The screen says which business it is for ("En Escuela Brazada"), adds "Lo que apagues igual queda en Avisos." in a team session, and each category is a row with its name, one line saying what it covers, and a `ToggleSwitch` for **Email** and for **Push** (only the channels the category uses). Saving is immediate on each switch, like the device notifications switch.
- The texts of the device switch hints (`student.notifications.hint`, `teamNotifications.hint`) stay as they are; the new screen has its own texts.
- The unsubscribe page is a public web route like the invitation and password pages, with the business brand of the email it came from when the token carries it, and the app's otherwise.
- Every new text goes in `es.json` and `en.json`; the footer line and page texts go in every email language.
- Screenshots of Ajustes, the new screen and an email footer before and after go to the owner before the PR.

## Steps

1. **Preferences**: the enums, `NotificationOptOuts`, `INotificationPreferencesService`, every sender filtering through it, the two account endpoints, and Mi perfil → Notificaciones.
2. **Unsubscribe from the email**: the footer link, the headers, the token, the public page and its endpoints.
3. Then the [automatic reminders](../20261021-automatic-reminders/plan.md) start, using the categories **Recordatorios de clases** and **Recordatorios de pago** from their first step.

Each step is its own PR.

## Tests

Unit tests (`ClassManager.Core.U.Tests`):

- Every category is on for an account with no rows; a row turns off only its category and channel.
- A team session lists the team categories and a student app session the family ones; someone outside the business is refused.
- An "always sent" message has no category and is never filtered.

Integration tests (`ClassManager.Api.I.Tests`, Testcontainers):

- For each category, an account that turned off the email gets no email and still gets the push, and the other way round; another account in the same business still gets both.
- A team member who turned off the push of Pedidos nuevos still gets the aviso in Avisos.
- A password reset, an invitation and a guardian consent request are sent even when every category is off.
- An account can't read or change another account's preferences, and business A can't read business B's.
- The unsubscribe token turns off exactly its account and category, a tampered token is refused, loading the page alone changes nothing, and resubscribe undoes it.
- The email carries the footer link and both `List-Unsubscribe` headers only when it has a category.

App tests (jest): the screen names the business; a switch saves at once and shows the saved state; the unsubscribe page doesn't turn anything off until its button is pressed, and **Volver a recibirlos** undoes it.

## Later

- An in-app list of notifications for families, like the team's Avisos, so turning a push off never means missing something.
- A daily digest email for the team instead of one email per order, if owners ask for it.
- WhatsApp as another channel per category, when the reminders add it.
