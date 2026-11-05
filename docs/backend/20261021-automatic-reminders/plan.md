# Automatic reminders

Date: 2026-10-21. Updated 2026-11-05 for the [plans catalog](../20261031-plans-catalog/plan.md): monthly fees no longer exist, families owe what their students' monthly plans add up to.

Status: proposal, nothing built. Comes after [notification preferences](../20261105-notification-preferences/plan.md): every reminder belongs to one of its categories.

## Why

Reviews of the apps closest to ours (Glofox, Mindbody, Jackrabbit, iClassPro) keep asking for **automatic reminders**: one before a class, and one when the month is unpaid. Today the team can only send an announcement of kind `Reminder` by hand ([better announcements](../../frontend/20261014-better-announcements/plan.md)), and families only find out they owe when they open Inicio. The [MVP plan](../../mvp-plan.md#not-included-in-the-mvp) left reminders out. Most of the pieces already exist: the background tasks table with due times ([background tasks](../20261007-background-tasks/plan.md)), web push for families and the team, branded emails in each language, the "no voy" notice and makeups, and what a family owes (its students' monthly plans, through `IFamilyBillingService`) and its class balance, both shown in the family app.

Two reminders matter most:

1. **Classes**: a push some hours before a class the family may forget, so the student comes, or sends "no voy" and frees the spot for a makeup ("no voy" is accepted until the class starts).
2. **Payments**: on the business's due day, families with at least one student on a monthly plan whose month is unpaid or partly paid get a push and an email. Families without a monthly plan, who pay with class packs, are told when a pack is about to expire with classes left, or when they owe classes.

WhatsApp and SMS are out of scope: both cost money per message and need a provider (Meta Business API, Twilio) and templates approved in advance. They can be added later as another channel for the same reminders.

## Decisions

### Designed for an API that is always on

- Each class gets its reminder **N hours before it starts**, and N is chosen by the business. That needs the API running at any hour, which it already is: the database moved to the Basic tier on 2026-10-24 and the API to `minReplicas 1` on 2026-11-03 ([hosting plan](../../hosting-plan.md#when-to-leave-the-free-setup), [runbook](../../database-basic-tier.md)).
- **A restart or a deploy loses nothing.** Reminders wait in the background tasks table with their due time, like scheduled announcements. A reminder whose time came while the worker was down goes out when it next starts, or is dropped if it went stale (see [Stale reminders are dropped](#stale-reminders-are-dropped)), so none arrives after its class started.

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
- **Families without the app** get no class reminder: a reminder by email for every class would reach the email provider's daily limit (Resend's free plan allows 100 a day, see [email domain](../../email-domain.md)) and is what families mute first.

### Payment reminders

| Reminder | Sent to | Content | Channel |
|---|---|---|---|
| Plan due | Families with a monthly plan that month (`HasMonthlyPlan`) whose month is `Unpaid` or `Partial` on the due day | "La cuota de {mes} vence hoy" with the amount left (`Balance`) | Push and email |
| Plan overdue | The same families, 7 days after the due day, if still `Unpaid` or `Partial` | "La cuota de {mes} sigue pendiente" with the amount left | Push and email |
| Pack about to expire | Any family with a pack purchase that expires in 7 days and has classes left (also families with a monthly plan that bought packs for extra classes) | "Tu pack {nombre} vence el {fecha} y te quedan {n} clases" | Push |
| Classes owed | Families without a monthly plan that month with unpaid classes (`UnpaidClasses` of the class balance), once a week (on the due day's weekday) | "Debés {n} clases" | Push |

- **What is owed comes from the existing billing**, never computed again: the family's month from `IFamilyBillingService` (the sum of its students' plans, with each student's own price, the same amount Cobros and the family card show), what was paid from the month's payments, and `FeeStatus` from `FeeRules`. A family whose plans add up to 0 (every student "incluido") is `NoFee` and is never reminded.
- **Plans chosen or cancelled from the shop** ([plans in the shop](../20261103-plans-in-the-shop/plan.md)) start or end on the first of a month, so the rule needs nothing special: a student who chose a plan "desde diciembre" adds to December's reminder, not November's, and one who cancelled stops counting from the month the plan ends.
- **The text keeps "cuota"**: the family app already calls what it owes "Cuota del mes" (`student.fee.title`), and the reminder uses the same word. The email lists one line per student with their plan and price, like the family card, then what was paid and what is left.
- Order lines (packs and products ordered and not paid yet) are not reminded in this plan: an order is followed up by the team when it is handed over.

- Sent at **10:00** in the business's time zone. Payment reminders aren't tied to a class, so one hour of the day for every business is enough, and 10:00 is inside working hours in both countries.
- Only to the family accounts of adults, who are the ones who can pay ("only adults buy", the same rule as the shop); a minor student's own account never gets one. By email to the client's address, also when the family doesn't use the app. Clients without an account get the email in the business country's language (Spanish for the countries supported today).
- **Email volume**: on the due day every family that owes gets an email at once. Before turning automatic payment reminders on for a business, move Resend to a paid plan or check that the families that owe fit the daily limit ([email domain](../../email-domain.md)).
- **At most two automatic payment reminders per month per family**, plus the ones the team sends by hand. Being nagged is the most common complaint about payment reminders.
- A late reminder still goes out: the rule is "due day or later, not sent yet", so a reminder missed because the API was stopped goes out the next time it runs, but never between 22:00 and 8:00.

### No reminders for an inactive business

A brand without an active subscription is read-only ([subscriptions](../20261003-subscriptions/plan.md)) and doesn't message its families.

### Who can turn them off

- **The business**, in Ajustes → Recordatorios: class reminders on or off, also the weekly classes, how many hours before, payment reminders on or off, the **due day** (1–28, default 10), and pack reminders on or off. New businesses start with class and payment reminders on (weekly classes off). Existing businesses start with everything **off**, and the owner turns them on, so no family gets a surprise push the day this is deployed.
- **Each person** decides in Mi perfil → Notificaciones ([notification preferences](../20261105-notification-preferences/plan.md)): class reminders are the category **Recordatorios de clases** (push), and payment reminders, pack and owed-classes reminders and the ones sent by hand are **Recordatorios de pago** (email and push). Every reminder email carries the one-click unsubscribe link. A family that turned off the email still sees what it owes in the app.
- Turning push off on a device (already in Ajustes) stops every push, reminders included.

### Sent at most once

- `SentReminders` (tenant-owned): `ClientId`, `Kind` (`Class`, `PlanDue`, `PlanOverdue`, `PackExpiring`, `ClassesOwed`, `PlanManual`), `Key` (the class's date and time and its class group, booking or private lesson; the billing month; or the pack purchase), `SentAt`, `SentByUserId` (`null` when automatic). A unique index on `(TenantId, ClientId, Kind, Key)` that leaves out `PlanManual` keeps a task that runs twice (delivery is at least once, a restart in the middle) from sending a reminder twice. The row and the push or email tasks are saved in the same transaction.
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
| `Businesses` | `ClassRemindersEnabled`, `WeeklyClassRemindersEnabled`, `PaymentRemindersEnabled`, `PackRemindersEnabled` (`bit`), `ClassReminderHoursBefore` (`tinyint`: 2, 3, 6, 12 or 24, default 3), `PaymentDueDay` (`tinyint`, 1–28, default 10). The migration sets the switches to 0 for existing businesses |
| `SentReminders` | New, tenant-owned, `IAuditable`, see [Sent at most once](#sent-at-most-once) |

### Texts

- The push texts are added to `INotificationTexts` (Spanish and English) and follow each account's language, like every family push.
- The payment emails are added to `IEmailTexts` in every language and use the branded layout (`src/Notifications/Email`): the business's brand, the month, one line per student with their plan and price, the total, what was paid, the amount left, and a button that opens the app.
- Reminders never include free text written by the team (announcements, notes): only the class name, the time and the student's name.

### Sending a payment reminder by hand

Cobros is one view of the month ([Cobros in one view](../../frontend/20261103-unified-collections/plan.md)): the summary says how many owe and how much, the **Deben** chip filters the list, and the floating button opens a counter sale. The reminder goes there without a second floating control:

- Each monthly plan row (`ClientFeeRow`) that is `Unpaid` or `Partial` gets a **Recordar** action (`small` button), and shows "Recordado el {fecha}" under its status once reminded. Order rows don't get it.
- With the **Deben** chip on, the list header shows **Recordar a todos** (`Button` `secondary`, `small`). It asks first with `ConfirmDialog` ("¿Enviar el recordatorio a 12 familias?", **Volver** / **Enviar**) because it reaches other people.
- Both use the same push and email as the automatic reminder and record a `PlanManual` row. A family reminded in the last 24 hours is skipped and counted in the toast ("Se envió a 10 familias. 2 ya tenían un recordatorio de hoy").
- Families that owe classes (Alumnos → Saldo de clases) are not reminded by hand in this plan; see [Later](#later).

## API

| Operation | Endpoint | Permission |
|---|---|---|
| Read and save the business's reminder settings | `GET` / `PUT /api/business/reminders` | `business.view` / `business.manage` |
| Remind every family that owes the month (monthly plans) | `POST /api/fees/reminders` with `{ month }` → `{ remindedCount, skippedCount }` | `payments.record` |
| Remind one family | `POST /api/fees/reminders/{clientId}` with `{ month }` | `payments.record` |

The routes stay under `/api/fees` because that is where the month's monthly plan list lives (`GET /api/fees`, `ListMonthlyFeesUseCase`).

The month's list (`GET /api/fees`) gains `lastRemindedAt` on each monthly plan client (`ClientFeeResponse`). Both reminder endpoints respect the member's scope (`payments.view.own` only reaches their own students' families), like the list.

## App

- **Team, Ajustes → Recordatorios** (a new row in Ajustes, shown with `business.view`, editable with `business.manage`): the class reminder switch with how many hours before, the weekly classes switch, the payment switch with the due day, the pack switch, and a **Vista previa** sheet with the class and payment pushes, reusing `PushNotificationPreview`. The form follows [forms](../../frontend/forms.md).
- **Team, Cobros**: Recordar per monthly plan row that owes, Recordar a todos in the list header with the Deben chip on, "Recordado el …" under the status (see [Sending a payment reminder by hand](#sending-a-payment-reminder-by-hand)).
- **Family**: nothing new in Ajustes; the reminder categories show in Mi perfil → Notificaciones, which ships first.
- Every new text goes in `es.json` and `en.json`.
- Before/after screenshots of Ajustes, Cobros with the Deben chip and the family Ajustes go to the owner before the PR ([UI changes](../../../CLAUDE.md#ui-changes)).

## Steps

Ordered from least to most work, so each one ships something families and the team notice:

1. **Reminding by hand** from Cobros: `SentReminders`, the payment push and email in every language, the two endpoints, `lastRemindedAt`, and the Recordar actions. No planner and no settings yet: it is the smallest step, and it builds the texts, the email and the "sent at most once" table the next steps reuse.
2. **Class reminders**: the planner, the class reminder task, the business settings (class switches and hours). The pilot owner turns them on.
3. **Automatic payment reminders**: the due day and payment switch in the settings, plan due and overdue, and the pack and owed-classes pushes, queued by the planner at 10:00.
4. **Always on** (operations, no code): done, the database on Basic on 2026-10-24 and the API with `minReplicas 1` on 2026-11-03 ([hosting plan](../../hosting-plan.md#when-to-leave-the-free-setup)).

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
- An account that turned off Recordatorios de clases gets none, and the other accounts of the family still do. An account that turned off the Recordatorios de pago email still gets the push, and the other way round; the reminder sent by hand respects the same choice.
- A business with reminders off, or without an active subscription, sends nothing.
- A reminder less than 30 minutes before its class, or after it started, is dropped.
- Payment reminders: only families with a monthly plan that month and `Unpaid` or `Partial`, on the due day and 7 days later, with the amount left from `IFamilyBillingService` and the month's payments. Never `Paid`, `NoFee` (plans adding up to 0) or families without a monthly plan, and never twice for the same month and kind. A missed one goes out on the next run, but not between 22:00 and 8:00.
- A student whose plan starts next month (chosen in the shop) doesn't count in this month's reminder; one whose plan ended doesn't count from the month it ended.
- Payment reminders reach the family accounts of adults, never a minor student's own account.
- Pack reminders: 7 days before expiry with classes left, also for a family with a monthly plan. Owed classes: once a week, only families without a monthly plan.
- Manual reminders skip a family reminded in the last 24 hours, and a member with `payments.view.own` can only remind families of their own students.

Integration tests (`ClassManager.Api.I.Tests`, Testcontainers):

- The planner queues one task per class due in the next two hours, queues itself for the next hour, and a second run doesn't queue the same reminder again.
- A class reminder task that runs twice sends the reminder once (unique index).
- When the worker starts with no pending planner it queues one, and never a second one.
- Business A's reminder settings, sent reminders and manual reminders can't be read or triggered by business B (tenant isolation).
- The endpoints' permissions: an instructor can't change the settings or send a payment reminder.
- The payment email renders in Spanish and English with the business's brand and one line per student and plan.

App tests (jest): the settings form saves the switches, the hours and the due day; Recordar a todos only shows with the Deben chip on and does nothing until the dialog is confirmed; a reminded row shows the date.

## Later

- **A Pagar button in the payment reminder** once families pay online ([online payments](../20260929-online-shop-and-payments/plan.md), Stripe Connect): the monthly plan becomes a Stripe subscription and most of these reminders stop being needed.
- Reminding by hand the families that owe classes, from Alumnos → Saldo de clases, and orders not paid yet.
- **WhatsApp** as a channel for the same reminders, for families without the app: the most used channel in Argentina and Spain, at a cost per message.
- A morning summary for instructors ("Hoy: 4 clases, 2 no vienen").
- Reminders for a class pack guarantee or a medical certificate that is about to expire ([pilot plan](../../pilot-df-swimming.md), Phase 3).
- A family choosing how long before, or turning off payment reminders, if the pilot asks for it.
