# Plan — Reschedule one session (same day)

Follow-up to [M4 attendance](../20260926-attendance/plan.md). The instructor moves one date of a class to another time on the same day ("today's 18:00 class starts at 19:00"). The weekly class group doesn't change. Moving a class to another day (a make-up class) stays out of scope.

## Backend

| Operation | Endpoint |
|---|---|
| Reschedule a date | `PUT /api/class-groups/{classGroupId}/sessions/{date}/schedule` `{ "startTime": "19:00" }` |
| Back to the usual time | `DELETE /api/class-groups/{classGroupId}/sessions/{date}/schedule` |

- `ClassSession` gains `RescheduledStartTime` (`TimeOnly?`). The duration stays the class group's.
- Like cancellations, rescheduling creates the session on demand.
- Day and session responses return the effective `startTime` / `endTime` plus `originalStartTime` when the date is rescheduled; the day is ordered by the effective time. Session responses gain `canReschedule`.

| # | Rule | Result |
|---|---|---|
| R1 | The class group meets on that weekday (same as A1) | `404` / `400 session.not_scheduled` |
| R2 | Only today or future dates | `400 session.in_past` |
| R3 | `startTime` is `HH:mm` and the class still ends by midnight | `400` on `startTime` |
| R4 | A cancelled date can't be rescheduled | `409 session.cancelled` |
| R5 | The instructor has no other active class overlapping the new time that day (using the other classes' effective times that date) | `409 class_group.instructor_busy` with `classGroupId` |

Rescheduling to the usual time clears the override.

## Frontend

- Day list: "19:00–19:45 · Reprogramada (era 18:00)".
- Session screen: "Cambiar horario ese día" → `hh:mm` field → confirm. When rescheduled: notice "Reprogramada: era a las 18:00" with "Volver al horario habitual". Hidden for past dates and cancelled sessions.
- The busy-instructor conflict shows "{instructor} ya da {class} a esa hora".

## Tests (write first)

```text
Core.U.Tests/Domain/Sessions/When_ClassSession_is_rescheduled_past_midnight/Then_validation_fails.cs
Core.U.Tests/UseCases/Sessions/When_RescheduleSession_in_the_past/Then_returns_validation.cs
Core.U.Tests/UseCases/Sessions/When_RescheduleSession_with_busy_instructor/Then_returns_conflict.cs
Core.U.Tests/UseCases/Sessions/When_RescheduleSession_first_change_of_the_day/Then_session_is_added_with_new_time.cs
Core.U.Tests/UseCases/Sessions/When_ListDaySessions_with_rescheduled_class/Then_effective_time_is_used_for_order.cs
Api.I.Tests/Endpoints/Sessions/When_rescheduling_session/Then_day_shows_new_and_original_time.cs
Api.I.Tests/Endpoints/Sessions/When_rescheduling_session_of_another_business/Then_returns_404.cs
app/src/features/sessions/__tests__/When_session_is_rescheduled/Then_request_has_new_start_time.test.tsx
app/src/features/sessions/__tests__/When_session_was_rescheduled/Then_original_time_and_restore_are_shown.test.tsx
```
