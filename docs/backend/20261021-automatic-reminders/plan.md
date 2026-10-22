# Automatic reminders

Date: 2026-10-21

Status: proposal, nothing built.

## Why

Reviews of the apps closest to ours (Glofox, Mindbody, Jackrabbit, iClassPro) keep asking for **automatic reminders**: one before a class, and one when a fee is unpaid. Today the team can only send an announcement of kind `Reminder` by hand ([better announcements](../../frontend/20261014-better-announcements/plan.md)), and families only find out they owe when they open Inicio. The [MVP plan](../../mvp-plan.md#not-included-in-the-mvp) left reminders out. Most of the pieces already exist: the background tasks table with due times ([background tasks](../20261007-background-tasks/plan.md)), web push for families and the team, branded emails in each language, the "no voy" notice and makeups, and the fee and class balances shown in the family app.

Two reminders matter most:

1. **Classes**: a push some hours before a class the family may forget, so the student comes, or sends "no voy" and frees the spot for a makeup ("no voy" is accepted until the class starts).
2. **Payments**: on the business's due day, families with an unpaid or partly paid fee get a push and an email. Families that pay per class are told when a pack is about to expire with classes left, or when they owe classes.

WhatsApp and SMS are out of scope: both cost money per message and need a provider (Meta Business API, Twilio) and templates approved in advance. They can be added later as another channel for the same reminders.

## Decisions

### Designed for an API that is always on

- Each class gets its reminder **N hours before it starts**, and N is chosen by the business. That needs the API running at any hour. The [hosting plan](../../hosting-plan.md#when-to-leave-the-free-setup) already plans the move to always on (`minReplicas 1` and the database on the Basic tier, [runbook](../../database-basic-tier.md)) once a business depends on the app, and reminders are built for that setup from the start.
- **While the API still scales to zero, nothing breaks.** Reminders wait in the background tasks table with their due time, like scheduled announcements. A reminder whose time comes while the container is stopped goes out when the container next starts, or is dropped if it went stale (see [Stale reminders are dropped](#stale-reminders-are-dropped)). In the pilot some reminders go out on time and some don't go out at all, but none arrives after its class started. Moving to always on needs no code change.

### Which classes are reminded

| Kind of class | Reminded |
|---|---|
| Makeup booking, pack booking, private lesson, trial class | Yes, when class reminders are on |
| A weekly class the student is enrolled in | Only when the business also turns on **Recordar también las clases fijas** (off by default) |

- The classes families forget are the ones outside their routine. A family that goes every Tuesday and Thursday doesn't need a push every Tuesday and Thursday, and that much noise is what makes families turn notifications off, which also stops announcements and cancellations. A business whose families do want it turns it on.
- Never for a cancelled session, an ended enrollment, a booking that was cancelled, or a date with a "no voy" notice.
- The time is the effective one: a rescheduled session is reminded N hours before its new time.

### How long before

- The business chooses **2, 3, 6, 12 or 24 hours** before. **Default 3 hours**: close enough not to be forgotten, and still time to send "no voy" so the spot can go to a makeup.
- **No reminder between 22:00 and 8:00** in the business's time zone. A reminder that would fall in that window goes out at 21:00 the day before instead (a class at 8:00 with 3 hours is reminded at 21:00 the evening before).
- **No reminder for a booking made after its reminder time** (a makeup booked an hour before the class): the family just booked it.

### What the push says

- **One push per account and class time**, not one per student: two siblings in the same 18:00 class get one push.
- "Hoy tenés clase a las {hora}" or "Mañana tenés clase a las {hora}", with the class name, and the student's name when the family has more than one student. A private lesson says "Clase particular", and a trial class "Clase de prueba".
- It opens Mis clases, where "No voy" is.
- **A student's own account** (`ClientAccount.StudentId` set) only gets reminders for their own classes. The family accounts get every student of the family.
- **Families without the app** get no class reminder: a reminder by email for every class would reach the SMTP limit (Gmail allows about 500 a day, see [pilot deployment](../../pilot-deployment.md#email)) and is what families mute first.

### Payment reminders

| Reminder | Sent to | Content | Channel |
|---|---|---|---|
| Fee due | Families on a monthly plan whose current month is `Unpaid` or `Partial` on the due day | "La cuota de {mes} vence hoy" with the amount left (`Balance`) | Push and email |
| Fee overdue | The same families, 7 days after the due day, if still `Unpaid` or `Partial` | "La cuota de {mes} sigue pendiente" with the amount left | Push and email |
| Pack about to expire | Families on `ClassPacks` with a purchase that expires in 7 days and has classes left | "Tu pack {nombre} vence el {fecha} y te quedan {n} clases" | Push |
| Classes owed | Families on `ClassPacks` with unpaid classes, once a week (on the due day's weekday) | "Debés {n} clases" | Push |

- Sent at **10:00** in the business's time zone. Payment reminders aren't tied to a class, so one hour of the day for every business is enough, and 10:00 is inside working hours in both countries.
- Only to the family accounts (the person who pays), and by email to the client's address, also when the family doesn't use the app. Clients without an account get the email in the business country's language (Spanish for the countries supported today).
- **At most two fee reminders per month per family**, plus the one the team sends by hand. Being nagged is the most common complaint about payment reminders.
- A late reminder still goes out: the rule is "due day or later, not sent yet", so a reminder missed because the API was stopped goes out the next time it runs, but never between 22:00 and 8:00.

### No reminders for an inactive business

A brand without an active subscription is read-only ([subscriptions](../20261003-subscriptions/plan.md)) and doesn't message its families.

### Who can turn them off

- **The business**, in Ajustes → Recordatorios: class reminders on or off, also the weekly classes, how many hours before, fee reminders on or off, the **due day** (1–28, default 10), and pack reminders on or off. New businesses start with class and payment reminders on (weekly classes off). Existing businesses start with everything **off**, and the owner turns them on, so no family gets a surprise push the day this is deployed.
- **Each family account** can turn off **class reminders** in its Ajustes, below the notifications switch. Payment reminders can't be turned off by the family: they are about their own account, like an order email.
- Turning push off on a device (already in Ajustes) stops every push, reminders included.

### Sent at most once

- `SentReminders` (tenant-owned): `ClientId`, `Kind` (`Class`, `FeeDue`, `FeeOverdue`, `PackExpiring`, `ClassesOwed`, `FeeManual`), `Key` (the class's date and time and its class group, booking or private lesson; the billing month; or the pack purchase), `SentAt`, `SentByUserId` (`null` when automatic). A unique index on `(TenantId, ClientId, Kind, Key)` that leaves out `FeeManual` keeps a task that runs twice (delivery is at least once, a restart in the middle) from sending a reminder twice. The row and the push or email tasks are saved in the same transaction.
- Rows have no value to bring back, so they aren't soft deleted. They are kept as the history the team sees ("Recordado el 10/10"). They are small: one row per family and reminder.

## Design

### The planner

Sessions are created on demand, not ahead ([domain model](../domain-model.md)), so there is no row to hang a reminder on in advance. A planner finds what is due:

- `PlanRemindersBackgroundTaskCommand` (no tenant) runs **every hour**: its handler queues itself again for the next hour, so it lives in the background tasks table like any other task, with no new timer.
- For each business with reminders on, it finds the classes whose reminder time falls within the next two hours and that have no reminder sent or pending, and queues one `SendClassReminderBackgroundTaskCommand(TenantId, ClassKey, StartsAt)` per class with its first attempt at the reminder time (`IBackgroundTaskOutbox.EnqueueAsync(command, runOn)`). Looking two hours ahead with an hourly run means a missed run doesn't lose a reminder, and the check against pending tasks (`IBackgroundTaskOutbox.HasPendingAsync(command)`, next to the existing `CancelPendingAsync`) keeps the overlap from queuing it twice.
- Once a day per business, on the first run at or after 10:00 local, it queues `SendPaymentRemindersBackgroundTaskCommand(TenantId, Date)`.
- **There is always one pending planner.** When the worker starts it queues one if none is pending. A planner that was due while the container was stopped runs on start, which covers what is due right then.

### Sending a class reminder

`SendClassReminderBackgroundTaskHandler` establishes the tenant and **decides again at send time**, with the Core rules (`ReminderRules`), because things change after the reminder was queued:

- The session was cancelled, the booking cancelled, or "no voy" sent: no push for that student.
- The session was rescheduled: if its new reminder time is later, the task queues itself for that time. If it is earlier, it sends now unless the reminder is stale.
- The business turned reminders off, or the account turned class reminders off: nothing for them.

Then it saves the `SentReminders` rows and queues the pushes (`SendPushBackgroundTaskCommand`) in one transaction.

### Stale reminders are dropped

A class reminder that runs **less than 30 minutes before its class, or after it started**, is not sent and a warning is logged. This is what happens when the container was stopped at the reminder time and starts later, and in the pilot it is normal. Once the API is always on, these warnings mean the worker is falling behind.

### Data

| Change | What |
|---|---|
| `Businesses` | `ClassRemindersEnabled`, `WeeklyClassRemindersEnabled`, `FeeRemindersEnabled`, `PackRemindersEnabled` (`bit`), `ClassReminderHoursBefore` (`tinyint`: 2, 3, 6, 12 or 24, default 3), `FeeDueDay` (`tinyint`, 1–28, default 10). The migration sets the switches to 0 for existing businesses |
| `ClientAccounts` | `ReceivesClassReminders` (`bit`, default 1) |
| `SentReminders` | New, tenant-owned, `IAuditable`, see [Sent at most once](#sent-at-most-once) |

### Texts

- The push texts are added to `INotificationTexts` (Spanish and English) and follow each account's language, like every family push.
- The fee emails are added to `IEmailTexts` in every language and use the branded layout (`src/Notifications/Email`): the business's brand, the month, the fee, what was paid, the amount left, and a button that opens the app.
- Reminders never include free text written by the team (announcements, notes): only the class name, the time and the student's name.

### Sending a fee reminder by hand

Cobros → Deben gets **Recordar a todos** and a **Recordar** action on each row. It uses the same push and email as the automatic reminder, records a `FeeManual` row, and the row shows "Recordado el {fecha}". Sending to everyone asks first with `ConfirmDialog` ("¿Enviar el recordatorio a 12 familias?", **Volver** / **Enviar**) because it reaches other people. A family reminded in the last 24 hours is skipped and counted in the toast ("Se envió a 10 familias. 2 ya tenían un recordatorio de hoy").

## API

| Operation | Endpoint | Permission |
|---|---|---|
| Read and save the business's reminder settings | `GET` / `PUT /api/business/reminders` | `business.view` / `business.manage` |
| Remind every family that owes the month | `POST /api/fees/reminders` with `{ month }` → `{ remindedCount, skippedCount }` | `payments.record` |
| Remind one family | `POST /api/fees/reminders/{clientId}` with `{ month }` | `payments.record` |
| The family account's class reminder switch | `GET` / `PUT /api/student-app/settings/reminders` | Family session |

The month's fees list (`GET /api/fees`) gains `lastRemindedAt` per client.

## App

- **Team, Ajustes → Recordatorios** (a new row in Ajustes, shown with `business.view`, editable with `business.manage`): the class reminder switch with how many hours before, the weekly classes switch, the fee switch with the due day, the pack switch, and a **Vista previa** sheet with the class and fee pushes, reusing the announcement preview. The form follows [forms](../../frontend/forms.md).
- **Team, Cobros → Deben**: Recordar a todos (floating bar), Recordar per row (`small` button), "Recordado el …" under the status.
- **Family, Ajustes**: a "Recordatorio de clases" switch below the notifications card.
- Every new text goes in `es.json` and `en.json`.
- Before/after screenshots of Ajustes, Cobros → Deben and the family Ajustes go to the owner before the PR ([UI changes](../../../CLAUDE.md#ui-changes)).

## Steps

1. **Class reminders**: the planner, the class reminder task, `SentReminders`, the business and family settings. The pilot owner turns them on.
2. **Payment reminders**: due day, the fee push and email, the pack and owed-classes pushes.
3. **Reminding by hand** from Cobros → Deben.
4. **Always on** (operations, no code): `minReplicas 1` and the database on Basic, when the first business pays. Until then reminders go out only while the container is running.

Each code step is its own PR.

## Tests

Unit tests (`ClassManager.Core.U.Tests`, in-memory fakes), one rule per test:

- A makeup booking, a pack booking, a private lesson and a trial class get a reminder N hours before.
- A weekly class gets one only when the business turned on weekly classes.
- No reminder for a cancelled session, a cancelled booking, an ended enrollment, or a date with a "no voy" notice, also when they happen after the reminder was queued.
- A rescheduled session is reminded N hours before its new time.
- A reminder that would fall between 22:00 and 8:00 goes out at 21:00 the day before.
- No reminder for a booking made after its reminder time.
- Two students of the same family in the same class time get one push. A student's own account only gets their own classes.
- An account that turned class reminders off gets none, and the other accounts of the family still do.
- A business with reminders off, or without an active subscription, sends nothing.
- A reminder less than 30 minutes before its class, or after it started, is dropped.
- Fee reminders: only `Unpaid` and `Partial` on a monthly plan, on the due day and 7 days later. Never `Paid`, `NoFee` or `ClassPacks` families, and never twice for the same month and kind. A missed one goes out on the next run, but not between 22:00 and 8:00.
- Pack reminders: 7 days before expiry with classes left. Owed classes: once a week.
- Manual reminders skip a family reminded in the last 24 hours.

Integration tests (`ClassManager.Api.I.Tests`, Testcontainers):

- The planner queues one task per class due in the next two hours, queues itself for the next hour, and a second run doesn't queue the same reminder again.
- A class reminder task that runs twice sends the reminder once (unique index).
- When the worker starts with no pending planner it queues one, and never a second one.
- Business A's reminder settings, sent reminders and manual reminders can't be read or triggered by business B (tenant isolation).
- The endpoints' permissions: an instructor can't change the settings or send a fee reminder.
- The fee email renders in Spanish and English with the business's brand.

App tests (jest): the settings form saves the switches, the hours and the due day; Recordar a todos does nothing until the dialog is confirmed; a reminded row shows the date; the family switch saves.

## Later

- **WhatsApp** as a channel for the same reminders, for families without the app: the most used channel in Argentina and Spain, at a cost per message.
- A morning summary for instructors ("Hoy: 4 clases, 2 no vienen").
- Reminders for a class pack guarantee or a medical certificate that is about to expire ([pilot plan](../../pilot-df-swimming.md), Phase 3).
- A family choosing how long before, or turning off payment reminders, if the pilot asks for it.
