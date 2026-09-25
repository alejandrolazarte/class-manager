# Backend plan — Enrollments (M3)

Third milestone of the [MVP plan](../../mvp-plan.md). A student enrolls in a class group from a date and stays until they leave. The class shows its roster and free spots; the student shows their classes. Attendance (M4) takes the roster of a date from here.

## Scope

| Operation | Endpoint | Use case |
|---|---|---|
| Enroll a student | `POST /api/class-groups/{classGroupId}/enrollments` | `EnrollStudentUseCase` |
| Roster of a class | `GET /api/class-groups/{classGroupId}/enrollments` | `ListClassRosterUseCase` |
| Classes of a student | `GET /api/students/{studentId}/enrollments` | `ListStudentEnrollmentsUseCase` |
| Unenroll | `PUT /api/enrollments/{enrollmentId}/end` | `EndEnrollmentUseCase` |
| Enrolled count in the class list | `GET /api/class-groups` gains `enrolledCount` | `ListClassGroupsUseCase` |

Writes are owner-only, like the rest of the class setup.

## Contract

### Enroll

```json
POST /api/class-groups/{classGroupId}/enrollments
{ "studentId": "0192f0c6-...", "startDate": "2026-10-01" }
```

`startDate` is optional and defaults to today in the business's time zone.

```json
{
  "id": "0192f0e1-...",
  "classGroupId": "0192f0d2-...",
  "studentId": "0192f0c6-...",
  "startDate": "2026-10-01",
  "endDate": null
}
```

| Status | When |
|---|---|
| `201` | Enrolled, `Location` points to the class roster |
| `404` | `class_group.not_found` or `student.not_found` (includes another business's) |
| `409` | `enrollment.already_enrolled` with `enrollmentId`; `class_group.full` with `capacity`; `class_group.inactive` |

### Roster

```json
GET /api/class-groups/{classGroupId}/enrollments
[
  {
    "enrollmentId": "0192f0e1-...",
    "studentId": "0192f0c6-...",
    "studentFullName": "Tomás Pérez",
    "birthDate": "2018-03-14",
    "clientId": "0192f0c4-...",
    "clientFullName": "Ana Pérez",
    "startDate": "2026-10-01",
    "endDate": null
  }
]
```

Current enrollments only (not ended before today), ordered by student name. Future starts are included so the owner sees who is about to join.

### Classes of a student

```json
GET /api/students/{studentId}/enrollments
[
  {
    "enrollmentId": "0192f0e1-...",
    "classGroupId": "0192f0d2-...",
    "classGroupName": "Natación inicial",
    "weekdays": ["Tuesday", "Thursday"],
    "startTime": "18:00",
    "endTime": "18:45",
    "instructorFullName": "Laura Gómez",
    "startDate": "2026-10-01",
    "endDate": null
  }
]
```

### Unenroll

```json
PUT /api/enrollments/{enrollmentId}/end
{ "endDate": "2026-10-31" }
```

`endDate` is optional and defaults to today; it is the last day the student attends. Ending before the start date means the student never came: the enrollment is deleted. Response `204`.

## Business rules

| # | Rule | Result |
|---|---|---|
| E1 | The class group and the student belong to the business; the class group is active | `404` / `409 class_group.inactive` |
| E2 | A student has at most one current enrollment per class group | `409 enrollment.already_enrolled` |
| E3 | Current enrollments of a class group never exceed its capacity | `409 class_group.full` |
| E4 | `endDate` ≥ `startDate`; before the start the enrollment is deleted | — |
| E5 | An enrollment already ended can't be ended again | `409 enrollment.already_ended` |
| E6 | A class group with current enrollments can't be deactivated | `409 class_group.has_enrollments` with `enrollmentCount` |
| E7 | Lowering capacity below the current enrollments is rejected | `400` on `capacity` |

"Current" means not ended before today in the business's time zone: `EndDate is null or EndDate >= today`.

E2 is also enforced by a filtered unique index `(TenantId, StudentId, ClassGroupId) WHERE EndDate IS NULL`. E3 is checked in the use case only: two owners enrolling the last spot at the same second could exceed it by one. With one owner per business that is accepted for the MVP and noted here.

## Design

```text
src/Core/
  Domain/Enrollments/
    Enrollment.cs                     → Create(studentId, classGroupId, startDate), End(endDate), IsCurrentOn(date)
    EnrollmentErrorCodes.cs
  Abstractions/Persistence/
    IEnrollmentRepository.cs          → Add, Remove, GetForUpdateAsync, FindCurrentAsync, CountCurrentAsync,
                                        CountCurrentByClassGroupAsync, ListRosterAsync, ListCurrentByStudentAsync
    RosterEntry.cs                    → read model: enrollment + student + client names
  UseCases/Enrollments/ ...
  UseCases/ClassGroups/               → enrolledCount in responses, E6 and E7
src/Infrastructure/
  EnrollmentConfiguration: FKs to Student and ClassGroup (Restrict), filtered unique index, index (TenantId, ClassGroupId, EndDate)
  Migration AddEnrollments
src/Api/
  Endpoints/EnrollmentEndpoints.cs
```

## Tests (write first)

### Unit

```text
Domain/Enrollments/
  When_Enrollment_ends_before_it_starts/Then_validation_fails.cs
  When_Enrollment_is_already_ended/Then_ending_again_fails.cs
  When_Enrollment_has_no_end_date/Then_it_is_current_from_its_start.cs
UseCases/Enrollments/
  When_EnrollStudent_in_full_class/Then_returns_conflict_with_capacity.cs
  When_EnrollStudent_already_enrolled/Then_returns_conflict_with_existing_id.cs
  When_EnrollStudent_in_inactive_class/Then_returns_conflict.cs
  When_EnrollStudent_without_start_date/Then_starts_today_in_business_time_zone.cs
  When_EndEnrollment_before_start/Then_enrollment_is_removed.cs
UseCases/ClassGroups/
  When_SetClassGroupActive_false_with_enrollments/Then_returns_conflict.cs
  When_UpdateClassGroup_capacity_below_enrolled/Then_returns_validation.cs
```

### Integration

```text
Endpoints/Enrollments/
  When_enrolling_student/Then_roster_and_enrolled_count_include_them.cs
  When_enrolling_in_full_class/Then_returns_409.cs
  When_enrolling_student_of_another_business/Then_returns_404.cs
  When_ending_enrollment/Then_student_leaves_the_roster_after_end_date.cs
  When_listing_student_enrollments/Then_class_details_are_returned.cs
  When_enrolling_same_student_twice/Then_returns_409.cs
Persistence/
  When_Enrollment_belongs_to_another_business/Then_it_is_not_returned.cs
```

## Implementation notes

- Status: backend done. Unit tests in `Domain/Enrollments` and `UseCases/Enrollments`, integration tests in `Endpoints/Enrollments`.
- `Location` of a new enrollment points to the class roster: there is no single-enrollment `GET`.
- `IBusinessCalendarService` (`Core/Services/BusinessCalendarService.cs`) gives "today" in the business's time zone; class group use cases use it for `enrolledCount`, E6 and E7.
- The integration tests' clock is fixed at 2026-09-24, so "today" in them is that date.

## Out of scope

- Waitlist when the class is full.
- Moving a student to another class in one step (end + enroll).
- Enrollment history screens; ended enrollments stay in the table for M4 and M5.
