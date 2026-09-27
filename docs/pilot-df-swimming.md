# Pilot plan — DF Swimming Team

The first pilot is the **owner** of DF Swimming Team, a Spanish network of affiliated swimming coaches ("filiales"). What the app already covers and what it still needs, in the order to build it.

## How the business works

- Group classes and private lessons (one to four students), for adults and children.
- Sells **named courses** instead of single classes: *ADG Básico* (10 × 30 min), *ADG De Autor* (10 × 45 min, with PDF and video material), *EDT* (5 × 45 min per stroke). Trial classes, free or paid, are deducted from the course.
- A money-back guarantee (7 or 30 days) and cancellation policies that vary per affiliate (for example free until 48 h, 50 € penalty after).
- Today: WordPress forms and WhatsApp for leads, payment outside any tool, and Timify for bookings. Timify doesn't know which course a student bought or how many classes are left: the student has to translate that by hand.
- The owner runs his own classes and also wants to see his coaches.

## Already covered

Families with children or adult students, weekly group classes, enrollments, attendance, monthly fees, **class packs with balance and expiry** (M6), several instructors, euros and Spain, and the Hoy calendar.

## Phases

### Phase 1 — Private lessons (before the pilot)

[Backend plan](backend/20260928-private-lessons/plan.md).

- Schedule a private lesson: one to four students, coach, date, time, duration and place. It shows on Hoy and on the month calendar next to group classes.
- Marking a student as present uses a class from their family's pack, with the same balance rules as group classes.
- Packs gain a **class duration** and an optional **material link** (PDF or video hosted elsewhere, for *ADG De Autor*), so "ADG De Autor = 10 × 45 min" is a catalog item.
- **Trial class**: a private lesson marked as a trial, free or paid. When the student buys a course, the paid trial can count as its first class.

Outside the app during the pilot: Timify for public booking, WordPress and WhatsApp for leads, payments collected as today and recorded in the app.

### Phase 2 — Coaches see their own agenda (early in the pilot)

- Coach accounts: the owner invites a coach by email. The coach signs in and sees only their classes, students and attendance. Only the owner sees money.
- Hoy and the calendar gain a coach filter for the owner.
- This is the "accounts for instructors" item that the MVP left out ([MVP plan](mvp-plan.md)).

### Phase 3 — Driven by pilot feedback

Pick from what the owner asks for most:

- Guarantee end date per purchase, and a warning before it ends.
- Cancellation policy per business (free window, penalty amount), recorded when a student cancels late.
- Automatic course certificate (PDF) when a course is completed.
- Lead tracking: contact → trial class → course.
- Online booking that only offers the durations and classes the student has left, to replace Timify.

### Places

Pools and cities as a list per business ("Piscina Alboraya, Valencia", "Tenerife"), each with its own time zone (the Canary Islands are one hour behind the peninsula). Classes pick a place instead of free text, and the owner can filter by place. Needed before reminders sent at an exact time, or before online booking.

### Later — Franchise layer

Each affiliate as its own business, with a brand account that shares the course catalog and standards and sees affiliates' activity. This is a large change in tenancy ([tenancy](tenancy.md)), so it waits until the single-business pilot proves the product.

## Also needed before the pilot starts

- Deploy ([pilot deployment](pilot-deployment.md)).
- ~~"Forgot password"~~: done, by email through a Gmail account ([email](pilot-deployment.md#email)).
- Import current students from a spreadsheet (CSV), or load them with a script.
- Error reporting and a WhatsApp feedback button in Ajustes.
