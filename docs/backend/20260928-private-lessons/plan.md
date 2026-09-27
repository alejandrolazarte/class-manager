# Backend plan — Private lessons (Phase 1 of the DF Swimming pilot)

See [pilot plan](../../pilot-df-swimming.md). Coaches of the pilot sell courses of private lessons (10 × 45 min) and still run group classes.

## Decisions

- **A private lesson is a one-off scheduled class**, not a weekly group: coach, date, start time, duration, optional place, one to four students, optional notes. It never repeats. For "every Tuesday for 10 weeks", the app offers **repeat weekly N times**, which creates N separate lessons that can each be moved or cancelled on their own.
- **Its own entity (`PrivateLesson`)**, not a `ClassGroup` without weekdays. Group classes keep their invariants (weekly schedule, capacity, enrollments); a private lesson has its students directly and no enrollment.
- **Attendance per student**, same `Present` / `Absent` as group sessions, stored on the lesson's student row (`PrivateLessonStudents.Status`) instead of a separate attendance table: a lesson's students are fixed, so one row per student is enough. A lesson can be cancelled with a reason, or moved to another date or time, while no attendance was taken.
- **Packs pay private lessons like group classes.** The class-balance allocation (M6) takes the union of group and private attendances of a family: oldest first, the pack valid on that date that expires first. Private lesson attendances count for families on `ClassPacks`; for families on a monthly fee they don't use classes (a family can have both).
- **Pack catalog additions:** optional `classDurationMinutes` (30, 45, 60...) and optional `materialUrl` (https only, ≤ 500 characters). The purchase copies both. Scheduling a lesson for a student whose family has an active pack with a duration defaults the lesson to that duration.
- **Trial class:** `isTrial` on the lesson, plus an optional `trialPrice` (empty = free). A trial never uses a class from a pack. DF sells trials "deductible from the course", so a **paid trial's price is deducted from the price of the next pack sold** to that family: the sale can reference the trial (`trialLessonId`), the app lowers the price by the trial price, and each trial can be deducted only once. The class balance lists `deductibleTrials` (paid, not cancelled, not yet deducted). Trial money is not a payment of its own yet: it is recorded on the lesson for reference.
- **Coach conflicts are per coach, never per business:** the same coach can't have two private lessons, or a private lesson and a group class, overlapping on the same date, because one person can't be at two pools at once. Different coaches never conflict, whatever the place or time (the owner in Tenerife and another coach in Valencia can both teach at 18:00). Two coaches sharing a pool at the same time is also allowed. Same `instructorBusy` code as class groups.
- **Times are local wall-clock times of the pool.** A business has one time zone, and the pilot spans the Canary Islands (one hour behind the peninsula). Each coach enters the local time of the pool, and the app shows it as entered. That is enough while nothing is sent at an exact time. Per-place time zones come with places, in a later phase (see the [pilot plan](../../pilot-df-swimming.md)).

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

- `GET /api/sessions?date=` returns group sessions **and** private lessons of that day, each with `kind: "Group" | "Private"`, `classGroupId` (null for private lessons), `privateLessonId` and `studentNames`. For a private lesson, `classGroupName` holds the student names, so an app that doesn't know the kind yet still shows something meaningful.
- Creating or editing a class group, and rescheduling one group session, also check the coach's private lessons.
- `GET /api/sessions/calendar?month=` counts private lessons in `classCount`, `cancelledCount` and `pendingAttendanceCount`.
- `GET /api/clients/{id}/class-balance` includes private lesson attendances; `unpaidAttendances` shows them with the coach name instead of a class name.
- Class pack catalog and purchase gain `classDurationMinutes` and `materialUrl`. The sale gains `trialLessonId`, and the class balance gains `deductibleTrials`.

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

## Delivery

- **Part 1 (this PR):** private lessons, weekly repeat, attendance, coach conflicts in both directions, and private lessons in the day list, month calendar and class balance.
- **Part 2:** pack `classDurationMinutes` and `materialUrl`, and trial classes (`isTrial`, `trialPrice`, counting the trial in a later pack).
- **Part 3:** the app screens.

## Business rules

| # | Rule | Result |
|---|---|---|
| L1 | One to four distinct students, all of the business | Validation / `404` |
| L2 | Active coach of the business | Validation / `404` |
| L3 | Start time `HH:mm`, duration 15–240 in steps of 5, ends by midnight (same as class groups) | Validation |
| L4 | `repeatWeeks` 1–52; every created lesson passes L5, or none is created | Validation / `409` |
| L5 | The same coach has no overlapping private lesson or group session that date (other coaches are never checked) | `409 instructorBusy` with the conflicting date |
| L6 | Attendance only for the lesson's students, not on a cancelled lesson, not for a future date | Validation / `409` |
| L7 | Cancel, move and delete only while no attendance was taken | `409 hasAttendance` |
| L8 | `trialPrice` follows the amount rule (0 or empty = free) and is only allowed with `isTrial` | Validation |
| L10 | A sale can deduct only a paid trial of one of the family's students, once | Validation `class_pack_purchase.trial_not_deductible` |
| L11 | A trial deducted from a sale can't be deleted until that sale is deleted | `409` |
| L9 | `materialUrl` is an absolute `https` URL | Validation |

## Data

- `PrivateLessons` (`TenantId`, `InstructorId`, `Date`, `StartTime`, `DurationMinutes`, `Location?`, `Notes?`, `IsTrial`, `TrialPrice?`, `IsCancelled`, `CancellationReason?`, `SeriesId?`, `CreatedAt`), index `(TenantId, Date)` and `(TenantId, InstructorId, Date)`.
- `PrivateLessonStudents` (`TenantId`, `PrivateLessonId`, `StudentId`, `Status?`), unique per lesson and student; deleted with its lesson.
- `ClassPacks` and `ClassPackPurchases` gain nullable `ClassDurationMinutes` and `MaterialUrl`; `ClassPackPurchases` gains nullable `TrialLessonId` (unique when set); `PrivateLessons` gains `IsTrial` and `TrialPrice`.
- Additive migrations `AddPrivateLessons` (part 1) and `AddPackLessonsAndTrials` (part 2).

## Tests (write first)

Unit: every rule above, the weekly repeat (all or none on conflict), balance allocation mixing group and private attendances, a paid trial counted as the first class of the pack.

Integration: schedule and mark a private lesson and see a class used; the day list and month calendar include private lessons; a coach conflict with a group class returns 409; tenant isolation for the three new tables.

## App (frontend plan to follow)

- Hoy: a "Nueva clase particular" action. Private lessons show in the day list with a person icon and the student names, and open the same attendance screen.
- Family card: the pack shows duration, classes left and the material link. Selling a pack right after a trial offers to count it.
- Schedule form: students (search, up to four), coach, date, time, duration (defaulting from the family's pack), place, "repeat weekly" with a count, trial switch.
