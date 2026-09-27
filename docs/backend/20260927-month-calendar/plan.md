# Month calendar

The **Hoy** tab can expand its week strip into a month view. Each day shows whether there are classes, whether any class was cancelled, and whether a past class still has attendance to take.

## Endpoint

`GET /api/sessions/calendar?month=YYYY-MM` (month optional, defaults to the business's current month)

```json
{
  "month": "2026-09",
  "days": [
    { "date": "2026-09-10", "classCount": 2, "cancelledCount": 0, "pendingAttendanceCount": 1 }
  ]
}
```

- Only days on which at least one active class group meets are listed.
- `cancelledCount`: class groups whose session that day is cancelled.
- `pendingAttendanceCount`: non-cancelled class groups on a day **before today** (business time zone) with fewer marks (present + absent) than students enrolled on that date. Today is never pending, because the class may not have happened yet.
- The month is parsed with `BillingMonth`, the same `YYYY-MM` rules as `/api/fees`. Invalid values return 400.

## Implementation

- `ListMonthCalendarUseCase` loads, once per month, the active class groups, the enrollment periods overlapping the month (`IEnrollmentRepository.ListActiveInPeriodAsync`), the sessions in the month (`IClassSessionRepository.ListBetweenAsync`) and their attendance counts. It then computes each day in memory: at most 31 days × the number of class groups.
- Like the day view, it uses the class groups that are active today. A class deactivated later no longer appears in past months.
- No new tables, so no new tenancy test. The queries go through the existing tenant filters.

## App

- `useMonthCalendar(month, isEnabled)` is fetched only while the month view is open. Recording attendance and cancelling, restoring or rescheduling a session invalidate every cached month.
- `MonthCalendarGrid`: Monday-first grid, primary dot = classes, warning dot = attendance pending, soft danger background = cancellation, primary border = today. Tapping a day selects it and collapses back to the week strip.
