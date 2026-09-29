# Plan — Substitute instructor for one session

Follow-up to [reschedule one session](../20260926-reschedule-session/plan.md). When the usual instructor can't teach one date ("Laura is sick, Marcos gives today's 18:00 class"), someone who manages sessions assigns a substitute for that date. The weekly class group doesn't change.

## Backend

| Operation | Endpoint | Permission |
|---|---|---|
| Assign a substitute | `PUT /api/class-groups/{classGroupId}/sessions/{date}/substitute` `{ "instructorId": "..." }` | `sessions.manage` |
| Back to the usual instructor | `DELETE /api/class-groups/{classGroupId}/sessions/{date}/substitute` | `sessions.manage` |

- `ClassSession` gains `SubstituteInstructorId` (`Guid?`, FK to `Instructors`). Like cancelling and rescheduling, assigning creates the session on demand.
- The effective instructor of a date is the substitute, or the class group's instructor when there is none.
- Day responses return the effective `instructorFullName` plus `originalInstructorFullName` when the date has a substitute. Session responses return `instructorId`, `instructorFullName` and `originalInstructorFullName`.

| # | Rule | Result |
|---|---|---|
| S1 | The class group meets on that weekday | `404` / `400 session.not_scheduled` |
| S2 | The substitute exists and is active | `404 instructor.not_found` / `400 instructor.inactive` |
| S3 | A cancelled date can't get a substitute | `409 session.cancelled` |
| S4 | The substitute has no class (their own or one they substitute) or private lesson overlapping the session's effective time that day | `409 class_group.instructor_busy` with `classGroupId` or `privateLessonId` |
| S5 | Choosing the usual instructor clears the substitute | — |

Past dates are allowed, so the owner can record who actually gave a class.

## Who sees the session

- A coach sees a date and can take attendance in it when they are the usual instructor **or** the substitute (`SessionRules.IsInScope`). Day list, month calendar, session details and attendance use the same rule.
- The class group itself, its roster and its students stay with the usual instructor: a substitute sees the students of that date in the session, not in Alumnos.

## Agenda conflicts

`InstructorAgendaRules.FindClassGroupOverlapAsync` checks an instructor's group classes on given dates using effective instructors and times:

- The usual instructor is free on a date someone else substitutes, so they can take a private lesson then.
- The substitute is busy on that date, so scheduling a private lesson for them, rescheduling another of their classes or assigning them a second overlapping substitution conflicts.

Creating or changing a weekly class group ignores substitutions (they are one-off dates).

## Frontend

- Hoy: "Marcos Díaz (reemplaza a Laura Gómez)" on the card.
- Session: the instructor in the subtitle; notice "Marcos Díaz reemplaza a Laura Gómez ese día" with "Quitar reemplazo"; "Asignar reemplazo ese día" (active instructors except the current one) next to "Cambiar horario" and "Cancelar clase". All actions need `sessions.manage`.

## Tests

```text
Core.U.Tests/Domain/Sessions/When_ClassSession_gets_a_substitute/Then_the_substitute_teaches_it.cs
Core.U.Tests/Domain/Sessions/When_ClassSession_substitute_is_the_usual_instructor/Then_the_substitute_is_cleared.cs
Core.U.Tests/Domain/Sessions/When_cancelled_ClassSession_gets_a_substitute/Then_returns_conflict.cs
Core.U.Tests/UseCases/Sessions/When_AssignSubstitute_first_change_of_the_day/Then_session_is_added_with_substitute.cs
Core.U.Tests/UseCases/Sessions/When_AssignSubstitute_with_inactive_instructor/Then_returns_validation.cs
Core.U.Tests/UseCases/Sessions/When_AssignSubstitute_with_busy_substitute/Then_returns_conflict.cs
Core.U.Tests/UseCases/Sessions/When_AssignSubstitute_to_instructor_substituting_elsewhere/Then_returns_conflict.cs
Core.U.Tests/UseCases/Sessions/When_ListDaySessions_with_substitute/Then_substitute_and_usual_instructor_are_shown.cs
Core.U.Tests/UseCases/PrivateLessons/When_SchedulePrivateLesson_for_a_replaced_instructor/Then_it_is_scheduled.cs
Core.U.Tests/UseCases/PrivateLessons/When_SchedulePrivateLesson_for_an_instructor_who_substitutes/Then_returns_instructor_busy.cs
Api.I.Tests/Endpoints/Sessions/When_assigning_substitute/Then_session_shows_the_substitute.cs
Api.I.Tests/Endpoints/Sessions/When_removing_substitute/Then_usual_instructor_is_shown.cs
Api.I.Tests/Endpoints/Coaches/When_coach_substitutes_another_coach/Then_the_class_is_in_their_day.cs
Api.I.Tests/Endpoints/Coaches/When_coach_substitutes_another_coach/Then_they_record_attendance.cs
Api.I.Tests/Endpoints/Coaches/When_coach_assigns_a_substitute/Then_returns_403.cs
app/src/features/sessions/__tests__/When_substitute_is_assigned/Then_request_has_the_instructor.test.tsx
app/src/features/sessions/__tests__/When_substitute_is_busy/Then_busy_message_is_shown.test.tsx
app/src/features/sessions/__tests__/When_session_has_a_substitute/Then_notice_and_remove_are_shown.test.tsx
app/src/features/sessions/__tests__/When_day_has_a_substituted_class/Then_card_shows_who_it_replaces.test.tsx
```

## Out of scope

- Paying instructors per class given (the data is there: the effective instructor of every past session).
- Substituting a private lesson: edit the lesson and pick another coach.
