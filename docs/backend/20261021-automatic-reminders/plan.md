# Automatic reminders

Date: 2026-10-21

Status: proposal, nothing built.

## Why

Reviews of the apps closest to ours (Glofox, Mindbody, Jackrabbit, iClassPro) keep asking for **automatic reminders**: one before a class, and one when a fee is unpaid. Today the team can only send an announcement of kind `Reminder` by hand ([better announcements](../../frontend/20261014-better-announcements/plan.md)), and families only find out they owe when they open Inicio. The [MVP plan](../../mvp-plan.md#not-included-in-the-mvp) left reminders out. Most of the pieces already exist: the background tasks table with due times ([background tasks](../20261007-background-tasks/plan.md)), web push for families and the team, branded emails in each language, the "no voy" notice and makeups, and the fee and class balances shown in the family app.

Two reminders matter most:

1. **Classes**: the evening before, families get a push listing tomorrow's classes. This lowers no-shows and leads to more "no voy" notices sent in time, which frees spots for makeups.
2. **Payments**: on the business's due day, families with an unpaid or partly paid fee get a push and an email. Families that pay per class are told when a pack is about to expire with classes left, or when they owe classes.

WhatsApp and SMS are out of scope: both cost money per message and need a provider (Meta Business API, Twilio) and templates approved in advance. They can be added later as another channel for the same reminders.

## Decisions

### One evening round per day, at 19:00 in the business's time zone

- Every reminder is sent in a **daily round** at a fixed time: **19:00 the day before** for classes. The fee and pack reminders go out in the same round.
- 19:00 is early enough for a family to send "no voy" before a 7:00 class, late enough to cover the day's changes (cancellations and reschedules already send their own push), and outside quiet hours.
- **The time is fixed, not chosen by the business, and there is no "N hours before".** Hosting is the reason: see [Waking the API](#waking-the-api-for-the-round). Once the API is always on, a business setting can pick the hour and "2 h before" becomes possible ([Later](#later)).

### Who gets what

| Reminder | Sent to | Content | Channel |
|---|---|---|---|
| Tomorrow's classes | Each app account of a family with a class tomorrow: group class (current enrollment, session not cancelled, no "no voy" notice), makeup booking, pack booking, private lesson or trial class | **One push per account and day**, not one per class: "Mañana tenés clase" with "{clase} a las {hora}", or "Mañana: 2 clases" with the list. The student's name is added when the family has more than one student. The time is the effective one (rescheduled sessions) | Push |
| Fee due | Families on a monthly plan whose current month is `Unpaid` or `Partial` on the due day | "La cuota de {mes} vence hoy" with the amount left (`Balance`) | Push and email |
| Fee overdue | The same families, 7 days after the due day, if still `Unpaid` or `Partial` | "La cuota de {mes} sigue pendiente" with the amount left | Push and email |
| Pack about to expire | Families on `ClassPacks` with a purchase that expires in 7 days and has classes left | "Tu pack {nombre} vence el {fecha} y te quedan {n} clases" | Push |
| Classes owed | Families on `ClassPacks` with unpaid classes, once a week (the round on the due day's weekday) | "Debés {n} clases" | Push |

- **A student's own account** (`ClientAccount.StudentId` set) only gets reminders for their own classes. The family accounts get every student of the family. Payment reminders go only to the family accounts: the person who pays.
- **Families without the app** get no class reminder. They do get the fee emails when the client has an email address. A daily email per class would hit the SMTP limit (Gmail allows about 500 a day, see [pilot deployment](../../pilot-deployment.md#email)) and is what families mute first.
- **Push opens the screen to act on**: the class reminder opens Mis clases (where "No voy" is), and the payment reminders open Inicio (where the balance is).
- **No reminder for a business without an active subscription.** A read-only brand ([subscriptions](../20261003-subscriptions/plan.md)) doesn't message its families.
- **At most two fee reminders per month per family**, plus the one the team sends by hand. Being nagged is the most common complaint about payment reminders.

### Who can turn them off

- **The business** has one switch per reminder (classes, fees, packs) in Ajustes → Recordatorios, and the **due day** (1–28, default 10). New businesses start with all of them on. Existing businesses start with them **off**, and the pilot owner turns them on, so no family gets a surprise push the day this is deployed.
- **Each family account** can turn off **class reminders** in its Ajustes, below the notifications switch. Payment reminders can't be turned off by the family: they are about their own account, like an order email.
- Turning push off on a device (already in Ajustes) stops every push, reminders included.

### Sent at most once

- `SentReminders` (tenant-owned): `ClientId`, `Kind` (`ClassesTomorrow`, `FeeDue`, `FeeOverdue`, `PackExpiring`, `ClassesOwed`, `FeeManual`), `Key` (the class date, the billing month or the purchase id), `SentAt`, `SentByUserId` (`null` for the round). A unique index on `(TenantId, ClientId, Kind, Key)` without `FeeManual` stops a round that runs twice (at-least-once delivery, a restart in the middle) from sending a reminder twice. The row and the push or email tasks are saved in the same transaction.
- Rows have no value to bring back, so they aren't soft deleted. They are kept as the history the team sees ("Recordado el 10/10"). They are small: one row per family and reminder.

## Design

### The round

- `PlanRemindersBackgroundTaskCommand(TimeZoneId)` (no tenant) is queued with its first attempt at **19:00 in that time zone**. Its handler:
  1. Queues one `SendBusinessRemindersBackgroundTaskCommand(TenantId, RoundDate)` per business in that time zone that has any reminder on. Each business is its own task, so one failing business is retried alone and doesn't hold up the others.
  2. Queues itself again for 19:00 the next day. The next run is computed with `TimeZoneInfo` on the local date, so daylight saving changes keep 19:00 local.
- `SendBusinessRemindersBackgroundTaskHandler` establishes the tenant, works out the reminders due for `RoundDate` with Core rules (`ReminderRules`: who has a class tomorrow, which fees and packs are due), skips the ones already in `SentReminders` and the accounts that turned class reminders off, saves the rows and queues the pushes and emails (`SendPushBackgroundTaskCommand`, `SendEmailBackgroundTaskCommand`) in one transaction.
- **A late round sends no class reminders.** If the round runs more than 3 hours after 19:00 (the API didn't wake up), class reminders for tomorrow are skipped and a warning is logged: a reminder at 2:00 is worse than none. Payment reminders aren't tied to a time: if a round was missed, the next round sends them, since the rule is "due day or later, not sent yet".
- **There is always one planned round per time zone in use.** `IReminderSchedule.EnsurePlannedAsync(timeZoneId)` queues the plan task when there is no pending one for that time zone. It is called when the worker starts (for every distinct `Businesses.TimeZoneId`), at sign-up, and when a business changes its time zone. This needs `IBackgroundTaskOutbox.HasPendingAsync(command)`, next to the existing `CancelPendingAsync`.

### Waking the API for the round

The API runs with `minReplicas 0` and the database is serverless with auto-pause ([hosting plan](../../hosting-plan.md)). At 19:00 nothing is running, so a task due then waits until someone opens the app. The design covers two setups:

- **Pilot (scale to zero):** one **KEDA cron scale rule** per time zone in use, on the API's container app: `desiredReplicas 1` from 18:58 to 19:10 local, with the rule's own `timezone` (Container Apps accepts any ScaledObject-based KEDA scaler as a custom rule, see [scaling rules](https://learn.microsoft.com/azure/container-apps/scale-app#custom)). The worker does a pass when it starts, so the round runs. Cost: about 15 minutes of one replica a day per time zone, inside the free monthly grant, and one database resume a day per time zone. A runbook in [pilot deployment](../../pilot-deployment.md) adds the rules and lists the time zones in use (`SELECT DISTINCT TimeZoneId FROM Businesses`). A business in a new time zone needs a new rule. The late-round warning shows a forgotten rule.
- **Always on (`minReplicas 1`, Basic database):** no rules needed. The worker's existing "wait until the next due task" wakes it at 19:00. The hosting plan already names this as the step to take when a paying business depends on the app.

Recommendation: start the pilot with the cron rules (the pilot's businesses share two time zones, Madrid and Canarias), and move to always-on when the first business pays. Reminders are a good reason to make that move.

### Data

| Change | What |
|---|---|
| `Businesses` | `ClassRemindersEnabled`, `FeeRemindersEnabled`, `PackRemindersEnabled` (`bit`), `FeeDueDay` (`tinyint`, 1–28, default 10). The migration sets the switches to 0 for existing businesses. New businesses get 1 |
| `ClientAccounts` | `ReceivesClassReminders` (`bit`, default 1) |
| `SentReminders` | New, tenant-owned, `IAuditable`, see [Sent at most once](#sent-at-most-once) |

### Texts

- The push texts are added to `INotificationTexts` (Spanish and English) and follow each account's language, like every family push.
- The fee emails are added to `IEmailTexts` in every language and use the branded layout (`src/Notifications/Email`): the business's brand, the month, the fee, what was paid, the amount left, and a button that opens the app. Clients without an account get it in the business country's language (Spanish for the countries supported today).
- The class reminder never includes the business's free text (announcements, notes). It only uses the class name, the time and the student's name.

### Sending a fee reminder by hand

Cobros → Deben gets **Recordar a todos** and a **Recordar** action on each row. It uses the same push and email as the round, records a `FeeManual` row, and the row shows "Recordado el {fecha}". Sending to everyone asks first with `ConfirmDialog` ("¿Enviar el recordatorio a 12 familias?", **Volver** / **Enviar**) because it reaches other people. A family reminded in the last 24 hours is skipped and counted in the toast ("Se envió a 10 familias. 2 ya tenían un recordatorio de hoy").

## API

| Operation | Endpoint | Permission |
|---|---|---|
| Read and save the business's reminder settings | `GET` / `PUT /api/business/reminders` | `business.view` / `business.manage` |
| Remind every family that owes the month | `POST /api/fees/reminders` with `{ month }` → `{ remindedCount, skippedCount }` | `payments.record` |
| Remind one family | `POST /api/fees/reminders/{clientId}` with `{ month }` | `payments.record` |
| The family account's class reminder switch | `GET` / `PUT /api/student-app/settings/reminders` | Family session |

The month's fees list (`GET /api/fees`) gains `lastRemindedAt` per client.

## App

- **Team, Ajustes → Recordatorios** (new row in Ajustes, shown to `business.view`, editable with `business.manage`): three switches with a one-line explanation each ("Se envían a las 19:00 del día anterior"), the due day, and a **Vista previa** sheet showing the class and fee pushes, reusing the announcement preview. The form follows [forms](../../frontend/forms.md).
- **Team, Cobros → Deben**: Recordar a todos (floating bar), Recordar per row (`small` button), "Recordado el …" under the status.
- **Family, Ajustes**: "Recordatorio de clases" switch below the notifications card.
- Every new text goes in `es.json` and `en.json`.
- Before/after screenshots of Ajustes, Cobros → Deben and the family Ajustes go to the owner before the PR ([UI changes](../../../CLAUDE.md#ui-changes)).

## Steps

1. **Class reminders**: the round, its scheduling and `EnsurePlannedAsync`, `SentReminders`, the class rule, the business and family switches, the cron rules runbook. The pilot owner turns them on.
2. **Fee and pack reminders**: due day, the fee push and email, the pack and owed-classes pushes, the late-round rule for payments.
3. **Reminding by hand** from Cobros → Deben.

Each step is its own PR.

## Tests

Unit tests (`ClassManager.Core.U.Tests`, in-memory fakes), one rule per test:

- A family with a group class tomorrow gets one reminder. A family with two classes or two students also gets one reminder, listing them all.
- No reminder for a cancelled session, an ended enrollment, or a date with a "no voy" notice. Makeup bookings, pack bookings, private lessons and trial classes get one.
- The reminder uses the rescheduled time.
- A student's own account only lists their own classes.
- An account that turned class reminders off gets none, and the other accounts of the family still do.
- A business with class reminders off, or without an active subscription, sends nothing.
- A round more than 3 hours late sends no class reminders, but does send payment reminders.
- The next round is at 19:00 local across a daylight saving change (Europe/Madrid, last Sunday of March and of October).
- Fee reminders: only `Unpaid` and `Partial` on a monthly plan, on the due day and 7 days later. Never `Paid`, `NoFee` or `ClassPacks` families, and never twice for the same month and kind.
- Pack reminders: 7 days before expiry with classes left. Owed classes: once a week.
- Manual reminders skip a family reminded in the last 24 hours.

Integration tests (`ClassManager.Api.I.Tests`, Testcontainers):

- The round queues one business task per business in that time zone, and plans the next round.
- A round that runs twice sends each reminder once (unique index).
- `EnsurePlannedAsync` queues one plan per time zone and never a second one.
- Business A's reminder settings, sent reminders and manual reminders can't be read or triggered by business B (tenant isolation).
- The endpoints' permissions: an instructor can't change the settings or send a fee reminder.
- The fee email renders in Spanish and English with the business's brand.

App tests (jest): the settings form saves the switches and the due day, Recordar a todos does nothing until the dialog is confirmed, a reminded row shows the date, and the family switch saves.

## Later

- **The business chooses the hour, or "N hours before"**: once the API is always on, the round can run at any time, or one task per class can be queued.
- **WhatsApp** as a channel for the same reminders, for families without the app: the most used channel in Argentina and Spain, at a cost per message.
- A morning summary for instructors ("Hoy: 4 clases, 2 no vienen").
- Reminders for a class pack guarantee or a medical certificate that is about to expire ([pilot plan](../../pilot-df-swimming.md), Phase 3).
- A family choosing the hour, or turning off payment reminders, if the pilot asks for it.
