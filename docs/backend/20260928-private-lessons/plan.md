# Backend plan — Private lessons (Phase 1 of the DF Swimming pilot)

See [pilot plan](../../pilot-df-swimming.md). Coaches of the pilot sell courses of private lessons (10 × 45 min) and still run group classes.

## Decisions

- **A private lesson is a one-off scheduled class**, not a weekly group: coach, date, start time, duration, optional place, one to four students, optional notes. It never repeats. For "every Tuesday for 10 weeks", the app offers **repeat weekly N times**, which creates N separate lessons that can each be moved or cancelled on their own.
- **Its own entity (`PrivateLesson`)**, not a `ClassGroup` without weekdays. Group classes keep their invariants (weekly schedule, capacity, enrollments); a private lesson has its students directly and no enrollment.
- **Attendance per student**, same `Present` / `Absent` as group sessions, in `PrivateLessonAttendance`. A lesson can be cancelled with a reason, or moved to another date or time, while no attendance was taken.
- **Packs pay private lessons like group classes.** The class-balance allocation (M6) takes the union of group and private attendances of a family: oldest first, the pack valid on that date that expires first. Private lesson attendances count for families on `ClassPacks`; for families on a monthly fee they don't use classes (a family can have both).
- **Pack catalog additions:** optional `classDurationMinutes` (30, 45, 60...) and optional `materialUrl` (https only, ≤ 500 characters). The purchase copies both. Scheduling a lesson for a student whose family has an active pack with a duration defaults the lesson to that duration.
- **Trial class:** `isTrial` on the lesson, plus a `trialPrice` (0 = free). A paid trial is recorded as a payment. When the family buys a pack after a trial, the sale can include the trial ("count the trial as the first class"): the trial attendance is then paid by that pack.
- **Coach conflicts:** a coach can't have two private lessons, or a private lesson and a group class, overlapping on the same date. Same `instructorBusy` code as class groups.

## Scope

| Operation | Endpoint | Use case |
|---|---|---|
| Schedule (with optional weekly repeat) | `POST /api/private-lessons` | `SchedulePrivateLessonUseCase` |
| Get one | `GET /api/private-lessons/{id}` | `GetPrivateLessonUseCase` |
| Move (date, time, duration, place, coach) | `PUT /api/private-lessons/{id}` | `ReschedulePrivateLessonUseCase` |
| Cancel / restore | `PUT` / `DELETE /api/private-lessons/{id}/cancellation` | `CancelPrivateLessonUseCase`, `RestorePrivateLessonUseCase` |
| Mark a student | `PUT /api/private-lessons/{id}/attendance/{studentId}` | `RecordPrivateLessonAttendanceUseCase` |
| Delete (mistakes, no attendance) | `DELETE /api/private-lessons/{id}` | `DeletePrivateLessonUseCase` |

Existing endpoints that change:

- `GET /api/sessions?date=` returns group sessions **and** private lessons of that day, each with `kind: "Group" | "Private"`, the private lesson id and its student names.
- `GET /api/sessions/calendar?month=` counts private lessons in `classCount`, `cancelledCount` and `pendingAttendanceCount`.
- `GET /api/clients/{id}/class-balance` includes private lesson attendances; `unpaidAttendances` shows them with the coach name instead of a class name.
- Class pack catalog and purchase gain `classDurationMinutes` and `materialUrl`. The sale gains `includeTrialLessonId`.

## Contract

```json
POST /api/private-lessons
{
  "instructorId": "0192...",
  "studentIds": ["0192..."],
  "date": "2026-10-06",
  "startTime": "18:00",
  "durationMinutes": 45,
  "location": "Piscina Alboraya",
  "notes": null,
  "isTrial": false,
  "trialPrice": null,
  "repeatWeeks": 10
}
→ 201 [ { "id": "...", "date": "2026-10-06", ... }, ... ]
```

## Business rules

| # | Rule | Result |
|---|---|---|
| L1 | One to four distinct students, all of the business | Validation / `404` |
| L2 | Active coach of the business | Validation / `404` |
| L3 | Start time `HH:mm`, duration 15–240 in steps of 5, ends by midnight (same as class groups) | Validation |
| L4 | `repeatWeeks` 1–52; every created lesson passes L5, or none is created | Validation / `409` |
| L5 | The coach has no overlapping private lesson or group session that date | `409 instructorBusy` with the conflicting date |
| L6 | Attendance only for the lesson's students, not on a cancelled lesson, not for a future date | Validation / `409` |
| L7 | Cancel, move and delete only while no attendance was taken | `409 hasAttendance` |
| L8 | `trialPrice` follows the amount rule and is only allowed with `isTrial` | Validation |
| L9 | `materialUrl` is an absolute `https` URL | Validation |

## Data

- `PrivateLessons` (`TenantId`, `InstructorId`, `Date`, `StartTime`, `DurationMinutes`, `Location?`, `Notes?`, `IsTrial`, `TrialPrice?`, `IsCancelled`, `CancellationReason?`, `SeriesId?`, `CreatedAt`), index `(TenantId, Date)` and `(TenantId, InstructorId, Date)`.
- `PrivateLessonStudents` (`TenantId`, `PrivateLessonId`, `StudentId`), unique per lesson and student.
- `PrivateLessonAttendances` (`TenantId`, `PrivateLessonId`, `StudentId`, `Status`), unique per lesson and student.
- `ClassPacks` and `ClassPackPurchases` gain nullable `ClassDurationMinutes` and `MaterialUrl`; `ClassPackPurchases` gains nullable `TrialLessonId`.
- One additive migration `AddPrivateLessons`.

## Tests (write first)

Unit: every rule above, the weekly repeat (all or none on conflict), balance allocation mixing group and private attendances, a paid trial counted as the first class of the pack.

Integration: schedule and mark a private lesson and see a class used; the day list and month calendar include private lessons; a coach conflict with a group class returns 409; tenant isolation for the three new tables.

## App (frontend plan to follow)

- Hoy: a "Nueva clase particular" action. Private lessons show in the day list with a person icon and the student names, and open the same attendance screen.
- Family card: the pack shows duration, classes left and the material link. Selling a pack right after a trial offers to count it.
- Schedule form: students (search, up to four), coach, date, time, duration (defaulting from the family's pack), place, "repeat weekly" with a count, trial switch.
