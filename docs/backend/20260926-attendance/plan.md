# Backend plan — Attendance (M4)

Fourth milestone of the [MVP plan](../../mvp-plan.md). The instructor opens a day, sees its classes, and marks each enrolled student present or absent. A class can be cancelled for a date (holiday, pool closed) so it doesn't count as an absence.

## Scope

| Operation | Endpoint | Use case |
|---|---|---|
| Classes of a day | `GET /api/sessions?date=2026-09-25` | `ListDaySessionsUseCase` |
| One class on a date, with its students | `GET /api/class-groups/{classGroupId}/sessions/{date}` | `GetSessionUseCase` |
| Mark a student | `PUT /api/class-groups/{classGroupId}/sessions/{date}/attendance/{studentId}` | `RecordAttendanceUseCase` |
| Cancel the class that day | `PUT /api/class-groups/{classGroupId}/sessions/{date}/cancellation` | `CancelSessionUseCase` |
| Undo the cancellation | `DELETE /api/class-groups/{classGroupId}/sessions/{date}/cancellation` | `RestoreSessionUseCase` |

`date` defaults to today in the business's time zone. Writes are owner-only.

## Sessions are created on demand

A `ClassSession` row exists only when something happened on that date: attendance was taken or the class was cancelled. The day view is computed from the active class groups that meet on the weekday, merged with the sessions stored for that date. Nothing is generated ahead, so editing a class group never leaves stale future rows.

## Contract

### Day

```json
GET /api/sessions?date=2026-09-29
[
  {
    "classGroupId": "0192f0d2-...",
    "classGroupName": "Natación inicial",
    "date": "2026-09-29",
    "startTime": "18:00",
    "endTime": "18:45",
    "instructorFullName": "Laura Gómez",
    "location": "Pileta chica",
    "isCancelled": false,
    "cancellationReason": null,
    "enrolledCount": 6,
    "presentCount": 4,
    "absentCount": 1
  }
]
```

Ordered by start time. `enrolledCount` counts enrollments active on that date (started on or before it and not ended before it).

### Session

```json
GET /api/class-groups/{classGroupId}/sessions/2026-09-29
{
  "classGroupId": "0192f0d2-...",
  "classGroupName": "Natación inicial",
  "date": "2026-09-29",
  "startTime": "18:00",
  "endTime": "18:45",
  "isCancelled": false,
  "cancellationReason": null,
  "canTakeAttendance": true,
  "students": [
    { "studentId": "0192f0c6-...", "studentFullName": "Tomás Pérez", "clientFullName": "Ana Pérez", "birthDate": "2018-03-14", "status": "Present" },
    { "studentId": "0192f0c7-...", "studentFullName": "Lucía Pérez", "clientFullName": "Ana Pérez", "birthDate": null, "status": null }
  ]
}
```

`status` is `Present`, `Absent` or `null` (not taken yet). `canTakeAttendance` is false for future dates and cancelled sessions.

### Mark

```json
PUT /api/class-groups/{classGroupId}/sessions/2026-09-29/attendance/{studentId}
{ "status": "Absent" }
```

`status: null` clears the mark. Response `204`.

### Cancel and restore

```json
PUT /api/class-groups/{classGroupId}/sessions/2026-10-12/cancellation
{ "reason": "Feriado" }
```

`DELETE` on the same path restores the session. Both return `204`.

## Business rules

| # | Rule | Result |
|---|---|---|
| A1 | The class group belongs to the business and meets on that weekday | `404 class_group.not_found` / `400 session.not_scheduled` |
| A2 | Attendance can only be taken for today or past dates | `400 session.in_future` |
| A3 | Only students enrolled in the class on that date can be marked | `400 attendance.student_not_enrolled` |
| A4 | A cancelled session takes no attendance | `409 session.cancelled` |
| A5 | A session with attendance can't be cancelled | `409 session.has_attendance` |
| A6 | One session per class group and date; one mark per student and session | Unique indexes |
| A7 | Cancellation reason is optional, ≤ 200 characters | Validation |

Cancelling future dates is allowed: "the 12th is a holiday" is decided ahead of time.

## Design

```text
src/Core/Domain/Sessions/
  ClassSession.cs        → Create(classGroupId, date), Cancel(reason), Restore()
  Attendance.cs          → Create(classSessionId, studentId, status), ChangeStatus(status)
  AttendanceStatus.cs    → Present, Absent (stored as string)
  SessionErrorCodes.cs
src/Core/Abstractions/Persistence/
  IClassSessionRepository.cs   → Add, FindForUpdateAsync(classGroupId, date), ListByDateAsync(date)
  IAttendanceRepository.cs     → Add, Remove, FindForUpdateAsync(sessionId, studentId), ListBySessionAsync, CountBySessionsAsync
  IEnrollmentRepository.cs     → + ListRosterOnAsync(classGroupId, date), CountActiveOnByClassGroupAsync(date)
src/Core/UseCases/Sessions/ ...
src/Infrastructure/            → configurations, repositories, migration AddSessionsAndAttendance
src/Api/Endpoints/SessionEndpoints.cs
```

A lost race creating the same session (two devices marking the first student at once) hits the unique index and returns `409 session.concurrent_update`; the app retries the tap.

## Tests (write first)

### Unit

```text
Domain/Sessions/
  When_ClassSession_is_cancelled_with_long_reason/Then_validation_fails.cs
  When_ClassSession_is_restored/Then_it_is_not_cancelled.cs
UseCases/Sessions/
  When_RecordAttendance_in_the_future/Then_returns_validation.cs
  When_RecordAttendance_for_student_not_enrolled/Then_returns_validation.cs
  When_RecordAttendance_on_cancelled_session/Then_returns_conflict.cs
  When_RecordAttendance_first_mark_of_the_day/Then_session_and_attendance_are_added.cs
  When_RecordAttendance_with_null_status/Then_mark_is_removed.cs
  When_RecordAttendance_on_a_day_the_class_does_not_meet/Then_returns_validation.cs
  When_CancelSession_with_attendance/Then_returns_conflict.cs
  When_ListDaySessions/Then_only_classes_meeting_that_weekday_are_returned.cs
```

### Integration

```text
Endpoints/Sessions/
  When_marking_attendance/Then_session_shows_status_and_day_counts.cs
  When_cancelling_session/Then_day_shows_it_cancelled.cs
  When_marking_student_of_another_class/Then_returns_400.cs
  When_getting_session_of_another_business/Then_returns_404.cs
Persistence/
  When_ClassSession_belongs_to_another_business/Then_it_is_not_returned.cs
  When_Attendance_belongs_to_another_business/Then_it_is_not_returned.cs
```

## Out of scope

- Make-up classes, trial students and walk-ins who aren't enrolled.
- Attendance reports and absence alerts.
- Cancelling a whole date range at once (holidays one day at a time for now).
