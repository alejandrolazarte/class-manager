# Backend plan — Instructors and class groups (M2)

Second milestone of the [MVP plan](../../mvp-plan.md). The business defines who teaches (instructors) and its weekly classes (class groups): name, weekdays, start time, duration, capacity and instructor. Enrollments come in M3.

## Scope

| Operation | Endpoint | Use case |
|---|---|---|
| List instructors | `GET /api/instructors?includeInactive=true` | `ListInstructorsUseCase` |
| Create an instructor | `POST /api/instructors` | `CreateInstructorUseCase` |
| Rename an instructor | `PUT /api/instructors/{instructorId}` | `UpdateInstructorUseCase` |
| Activate or deactivate | `PUT /api/instructors/{instructorId}/active` | `SetInstructorActiveUseCase` |
| List class groups | `GET /api/class-groups?includeInactive=true` | `ListClassGroupsUseCase` |
| Get a class group | `GET /api/class-groups/{classGroupId}` | `GetClassGroupUseCase` |
| Create a class group | `POST /api/class-groups` | `CreateClassGroupUseCase` |
| Update a class group | `PUT /api/class-groups/{classGroupId}` | `UpdateClassGroupUseCase` |
| Activate or deactivate | `PUT /api/class-groups/{classGroupId}/active` | `SetClassGroupActiveUseCase` |

Writes are owner-only (`AuthorizationPolicies.OwnerOnly`), like the business settings. Sign up also creates the first instructor with the owner's name, because most businesses are one instructor and the first class shouldn't need a detour through settings.

## Contract

### Instructor

```json
POST /api/instructors
{ "fullName": "Laura Gómez" }
```

```json
{ "id": "0192f0d1-...", "fullName": "Laura Gómez", "isActive": true }
```

`PUT /api/instructors/{id}` takes the same body. `PUT /api/instructors/{id}/active` takes `{ "isActive": false }`.

| Status | When |
|---|---|
| `400` | Name invalid |
| `404` | `instructor.not_found` (includes another business's instructor) |
| `409` | `instructor.name_taken`, with `instructorId`; `instructor.has_active_class_groups` when deactivating, with `classGroupCount` |

### Class group

```json
POST /api/class-groups
{
  "name": "Natación inicial",
  "instructorId": "0192f0d1-...",
  "weekdays": ["Tuesday", "Thursday"],
  "startTime": "18:00",
  "durationMinutes": 45,
  "capacity": 8,
  "location": "Pileta chica"
}
```

```json
{
  "id": "0192f0d2-...",
  "name": "Natación inicial",
  "instructorId": "0192f0d1-...",
  "instructorFullName": "Laura Gómez",
  "weekdays": ["Tuesday", "Thursday"],
  "startTime": "18:00",
  "endTime": "18:45",
  "durationMinutes": 45,
  "capacity": 8,
  "location": "Pileta chica",
  "isActive": true
}
```

`PUT /api/class-groups/{id}` takes the same body as create. `PUT /api/class-groups/{id}/active` takes `{ "isActive": false }`. The list is ordered by start time, then name.

| Status | When |
|---|---|
| `400` | Validation failed (field errors) |
| `404` | `class_group.not_found`, or `instructor.not_found` for the `instructorId` |
| `409` | `class_group.instructor_busy`: the instructor already teaches an active class group that overlaps on a shared weekday; details carry the other `classGroupId` |

## Business rules

| # | Rule | Result |
|---|---|---|
| I1 | Instructor name required, trimmed, 2–120 characters | Validation |
| I2 | Instructor name unique per business (case-insensitive) | Conflict |
| I3 | An instructor with active class groups can't be deactivated | Conflict with the count |
| C1 | Class group name required, trimmed, 2–80 characters | Validation |
| C2 | At least one weekday; duplicates collapse | Validation |
| C3 | `startTime` is a local time of day (`HH:mm`) in the business's time zone | Validation when malformed |
| C4 | `durationMinutes` from 15 to 240, multiple of 5 | Validation |
| C5 | The class ends the same day (`startTime + duration ≤ 24:00`) | Validation |
| C6 | `capacity` from 1 to 100 | Validation |
| C7 | `location`, if present, ≤ 80 characters | Validation |
| C8 | The instructor exists in this business and is active | `404` / validation |
| C9 | An instructor can't teach two active class groups that share a weekday and overlap in time | Conflict |

C9 is the decision to discuss: a swimming instructor sometimes watches two lanes at once. It starts as a hard rule because overlapping by mistake is far more common; if the pilot users need it, it becomes a warning like salon-manager's out-of-hours bookings.

Times are wall-clock: "Tuesday 18:00" stays 18:00 across daylight saving changes, so `StartTime` is stored as `time` and never converted to UTC. `ClassSession` (M4) is where a date meets the time zone.

## Design

```text
src/Core/
  Domain/Instructors/
    Instructor.cs                      → Create, Rename, Activate, Deactivate
    InstructorErrorCodes.cs
  Domain/ClassGroups/
    ClassGroup.cs                      → Create(...), Update(...), Activate, Deactivate, OverlapsWith(other)
    ClassSchedule.cs                   → value object: weekdays, start time, duration; validation C2–C5, overlap
    ClassGroupErrorCodes.cs
  Abstractions/Persistence/
    IInstructorRepository.cs
    IClassGroupRepository.cs           → includes ListActiveByInstructorAsync for C9 and CountActiveByInstructorAsync for I3
  UseCases/Instructors/ ...            → list, create, update, set active
  UseCases/ClassGroups/ ...            → list, get, create, update, set active
  UseCases/Authentication/SignUpOwnerUseCase.cs → adds the owner as first instructor

src/Infrastructure/
  Configurations: Instructor (unique (TenantId, FullName)), ClassGroup (Weekdays stored as an int flags column, StartTime as time, index (TenantId, IsActive, StartTime))
  Repositories and migration AddInstructorsAndClassGroups

src/Api/
  Endpoints/InstructorEndpoints.cs, Endpoints/ClassGroupEndpoints.cs
```

- `ClassSchedule` owns the time math so both create and update share it, and M4 can ask it "does this group meet on this date?".
- Weekdays travel as `DayOfWeek` names in JSON and are stored as a flags `int` (`Monday = 1 … Sunday = 64`), so "which groups meet on Tuesday" is a bitwise filter in SQL.
- `startTime` travels as `"HH:mm"` (a `TimeOnly`); `endTime` is computed, never stored.

## Tests (write first)

### Unit — `tests/ClassManager.Core.U.Tests`

```text
Domain/Instructors/
  When_Instructor_is_created_with_short_name/Then_validation_fails.cs
Domain/ClassGroups/
  When_ClassSchedule_has_no_weekdays/Then_validation_fails.cs
  When_ClassSchedule_duration_is_not_multiple_of_five/Then_validation_fails.cs
  When_ClassSchedule_ends_after_midnight/Then_validation_fails.cs
  When_ClassSchedules_share_a_weekday_and_overlap/Then_they_overlap.cs
  When_ClassSchedules_touch_at_the_boundary/Then_they_do_not_overlap.cs
  When_ClassSchedules_overlap_in_time_on_different_weekdays/Then_they_do_not_overlap.cs
  When_ClassGroup_is_created_with_zero_capacity/Then_validation_fails.cs
UseCases/Instructors/
  When_CreateInstructor_with_taken_name/Then_returns_conflict_with_existing_id.cs
  When_SetInstructorActive_false_with_active_class_groups/Then_returns_conflict.cs
UseCases/ClassGroups/
  When_CreateClassGroup_with_busy_instructor/Then_returns_conflict_with_other_group.cs
  When_CreateClassGroup_with_inactive_instructor/Then_returns_validation.cs
  When_CreateClassGroup_with_unknown_instructor/Then_returns_not_found.cs
  When_CreateClassGroup_with_valid_data/Then_class_group_is_added_and_saved.cs
  When_UpdateClassGroup_keeps_its_own_time/Then_it_does_not_conflict_with_itself.cs
UseCases/Authentication/
  When_SignUpOwner_with_valid_data/Then_owner_is_first_instructor.cs
```

### Integration — `tests/ClassManager.Api.I.Tests`

```text
Endpoints/Instructors/
  When_signing_up/Then_owner_is_listed_as_instructor.cs
  When_creating_instructor/Then_returns_201.cs
  When_renaming_instructor_of_another_business/Then_returns_404.cs
Endpoints/ClassGroups/
  When_posting_valid_class_group/Then_returns_201_with_end_time.cs
  When_posting_class_group_without_weekdays/Then_returns_400_with_field_error.cs
  When_posting_overlapping_class_group_for_same_instructor/Then_returns_409.cs
  When_posting_class_group_with_instructor_of_another_business/Then_returns_404.cs
  When_listing_class_groups/Then_only_current_business_groups_are_returned.cs
  When_deactivating_class_group/Then_it_is_hidden_from_active_list.cs
Persistence/
  When_Instructor_belongs_to_another_business/Then_it_is_not_returned.cs
  When_ClassGroup_belongs_to_another_business/Then_it_is_not_returned.cs
```

## Implementation order

1. `Instructor` + repository + endpoints + owner as first instructor.
2. `ClassSchedule` value object (unit tests carry most of the rules).
3. `ClassGroup` + repository + migration + create and list.
4. Update, set active, instructor deactivation guard (I3).

## Implementation notes

- Status: backend done. Unit tests in `Domain/ClassGroups`, `Domain/Instructors`, `UseCases/ClassGroups` and `UseCases/Instructors`; integration tests in `Endpoints/ClassGroups` and `Endpoints/Instructors`.
- Instructor and class group checks shared by create, update and activate live in `ClassGroupRules`.
- `SetActiveRequest` lives in `Core/UseCases` and is used by both `/active` endpoints.
- Deactivating a class group never checks conflicts; activating one checks the instructor is active and free.

## Out of scope

- Instructor accounts (instructors signing in).
- Per-instructor working hours and days off; exceptions arrive with sessions in M4 (a cancelled session covers "no class this Tuesday").
- Class groups that change time from a given date on (edit applies from now on; history arrives with sessions).
- Price per class group (fees are per client in M5).
