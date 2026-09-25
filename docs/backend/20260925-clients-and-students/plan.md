# Backend plan — Clients and students (M1)

First milestone of the [MVP plan](../../mvp-plan.md). The template's `Client` becomes the person who pays and is contacted; the new `Student` is the person who attends. An adult who comes to class is both: one client with one student of the same name.

## Scope

| Operation | Endpoint | Use case |
|---|---|---|
| Register a client with their students | `POST /api/clients` | `RegisterClientUseCase` (extended) |
| Get a client with their students | `GET /api/clients/{clientId}` | `GetClientUseCase` (extended) |
| Search clients | `GET /api/clients?search=ana&limit=20` | `SearchClientsUseCase` (unchanged) |
| Add a student to an existing client | `POST /api/clients/{clientId}/students` | `AddStudentUseCase` |
| Get a student | `GET /api/students/{studentId}` | `GetStudentUseCase` |
| Search students | `GET /api/students?search=tomi&limit=20` | `SearchStudentsUseCase` |

Registering the client and the students in one request keeps them in one transaction: the app never leaves a parent without the child it was registering.

## Contract

### Register a client

```json
POST /api/clients
{
  "fullName": "Ana Pérez",
  "phoneNumber": "11 2233-4455",
  "email": "ana@example.com",
  "notes": null,
  "students": [
    { "fullName": "Tomás Pérez", "birthDate": "2018-03-14", "notes": "Afraid of deep water" },
    { "fullName": "Lucía Pérez", "birthDate": null, "notes": null }
  ]
}
```

`students` is optional; `null` or `[]` registers the client alone. An adult who attends is sent with one student of their own name (the app fills it).

| Status | When | Body |
|---|---|---|
| `201 Created` | Client and students registered | `ClientDetailsResponse`, `Location: /api/clients/{id}` |
| `400 Bad Request` | Validation failed | `ValidationProblemDetails`; student fields are named `Students[0].FullName` |
| `409 Conflict` | Phone number already registered | Unchanged: `client.phone_number_taken` with `clientId` |

```json
{
  "id": "0192f0c4-...",
  "fullName": "Ana Pérez",
  "phoneNumber": "+541122334455",
  "email": "ana@example.com",
  "notes": null,
  "createdAt": "2026-09-25T14:05:00Z",
  "students": [
    { "id": "0192f0c5-...", "clientId": "0192f0c4-...", "fullName": "Lucía Pérez", "birthDate": null, "notes": null, "createdAt": "2026-09-25T14:05:00Z" },
    { "id": "0192f0c6-...", "clientId": "0192f0c4-...", "fullName": "Tomás Pérez", "birthDate": "2018-03-14", "notes": "Afraid of deep water", "createdAt": "2026-09-25T14:05:00Z" }
  ]
}
```

`GET /api/clients/{clientId}` returns the same `ClientDetailsResponse`. Students are ordered by `fullName`.

### Add a student

```json
POST /api/clients/{clientId}/students
{ "fullName": "Martín Pérez", "birthDate": "2021-07-02", "notes": null }
```

| Status | When | Body |
|---|---|---|
| `201 Created` | Student added | `StudentResponse`, `Location: /api/students/{id}` |
| `400 Bad Request` | Validation failed | `ValidationProblemDetails` |
| `404 Not Found` | The client doesn't exist in this business | `ProblemDetails` with `client.not_found` |
| `409 Conflict` | The client already has a student with that name | `ProblemDetails` with `student.already_registered` and the existing `studentId` |

### Get and search students

`GET /api/students/{studentId}` returns a `StudentSummaryResponse`, or `404` with `student.not_found`. `GET /api/students` returns a list of them:

```json
{
  "id": "0192f0c6-...",
  "fullName": "Tomás Pérez",
  "birthDate": "2018-03-14",
  "notes": "Afraid of deep water",
  "clientId": "0192f0c4-...",
  "clientFullName": "Ana Pérez",
  "clientPhoneNumber": "+541122334455"
}
```

Search matches the student's name, the client's name (contains) or the client's phone number (normalized prefix, same rule as client search). Ordered by student name; `limit` defaults to 20, capped at 50. Searching by the parent's name finds the children, which is how instructors think ("the Pérez kids").

## Business rules

| # | Rule | Result on failure |
|---|---|---|
| S1 | `fullName` is required, trimmed, 2–120 characters | Validation |
| S2 | `birthDate`, if present, is not after today in the business's time zone and not before 1900-01-01 | Validation |
| S3 | `notes`, if present, ≤ 1000 characters | Validation |
| S4 | A client can't have two students with the same name (case-insensitive) | Validation inside one registration request; conflict when adding |
| S5 | At most 10 students per registration request | Validation |
| S6 | A student belongs to a client of the current business | `404` when adding to a client of another business |
| S7 | A student belongs to the current business (`TenantId`) | Never sent by the caller |

S4 is checked in the use case **and** enforced by the unique index `(TenantId, ClientId, FullName)`; SQL Server's default collation is case-insensitive. A lost race surfaces as `UniqueConstraintViolationException` and maps to the same `409`, as client registration already does.

Client rules are unchanged.

## Design

```text
src/Core/
  Domain/Students/
    Student.cs                          → Create(clientId, fullName, birthDate, notes, today, createdAt) : Result<Student>
    StudentErrorCodes.cs                → student.not_found, student.already_registered, studentId detail
  Abstractions/Persistence/
    IStudentRepository.cs               → Add, FindByClientAndNameAsync, ListByClientAsync, GetSummaryByIdAsync, SearchAsync
    StudentSearchCriteria.cs
    StudentSummary.cs                   → read model: student + client name and phone
  UseCases/Clients/
    RegisterClientUseCase.cs            → command gains Students; returns ClientDetailsResponse
    GetClientUseCase.cs                 → returns ClientDetailsResponse
    ClientDetailsResponse.cs
  UseCases/Students/
    NewStudent.cs                       → record used by both commands
    AddStudentUseCase.cs
    GetStudentUseCase.cs
    SearchStudentsUseCase.cs
    StudentResponse.cs
    StudentSummaryResponse.cs

src/Infrastructure/
  Persistence/Configurations/StudentConfiguration.cs   → FK to Client (Restrict), FK to Business, unique (TenantId, ClientId, FullName), index (TenantId, FullName)
  Persistence/Repositories/StudentRepository.cs        → search joins Clients inside the tenant filter
  Migration AddStudents

src/Api/
  Endpoints/StudentEndpoints.cs         → POST /api/clients/{clientId}/students, GET /api/students/{id}, GET /api/students
```

Guidelines:

- "Today" for S2 is computed in the use case from `TimeProvider` and the business's `TimeZoneId`, and passed to `Student.Create`, so the entity stays free of time zones and the clock.
- `Student` holds `ClientId` only, no navigation property; the summary read model is a projection in the repository.
- Student validation errors inside a registration are prefixed with the index (`Students[1].FullName`) so the app can put the message under the right field.
- Error codes and route segments are constants.

## Tests (write first)

### Unit — `tests/ClassManager.Core.U.Tests`

```text
Domain/Students/
  When_Student_is_created_with_short_name/Then_validation_fails.cs
  When_Student_is_created_with_future_birth_date/Then_validation_fails.cs
  When_Student_is_created_with_birth_date_before_1900/Then_validation_fails.cs
  When_Student_is_created_with_long_notes/Then_validation_fails.cs
  When_Student_is_created_with_valid_data/Then_name_is_trimmed.cs
UseCases/Clients/
  When_RegisterClient_with_students/Then_client_and_students_are_added_and_saved.cs
  When_RegisterClient_with_duplicate_student_names/Then_field_error_names_second_student.cs
  When_RegisterClient_with_too_many_students/Then_nothing_is_saved.cs
  When_RegisterClient_with_invalid_student/Then_field_error_names_student_index.cs
UseCases/Students/
  When_AddStudent_to_unknown_client/Then_returns_not_found.cs
  When_AddStudent_with_taken_name/Then_returns_conflict_with_existing_id.cs
  When_AddStudent_loses_a_race_on_name/Then_returns_conflict.cs
  When_AddStudent_with_valid_data/Then_student_is_added_and_saved.cs
  When_AddStudent_birth_date_is_tomorrow_in_business_time_zone/Then_returns_validation.cs
  When_GetStudent_with_unknown_id/Then_returns_not_found.cs
  When_SearchStudents_with_limit_over_maximum/Then_limit_is_capped.cs
```

### Integration — `tests/ClassManager.Api.I.Tests` (Testcontainers)

```text
Endpoints/Clients/
  When_posting_client_with_students/Then_returns_201_with_students.cs
  When_posting_client_with_invalid_student/Then_returns_400_with_indexed_field_error.cs
Endpoints/Students/
  When_adding_student_to_existing_client/Then_returns_201_with_location.cs
  When_adding_student_with_taken_name/Then_returns_409.cs
  When_adding_student_to_client_of_another_business/Then_returns_404.cs
  When_searching_students_by_client_name/Then_their_students_are_returned.cs
  When_searching_students/Then_only_current_business_students_are_returned.cs
  When_getting_student_of_another_business/Then_returns_404.cs
Persistence/
  When_Student_belongs_to_another_business/Then_it_is_not_returned.cs
```

The last three are the tenancy guarantees.

## Implementation order

1. `Student.Create` (unit tests → implementation).
2. `StudentConfiguration`, `StudentRepository`, migration `AddStudents`, persistence isolation test.
3. `AddStudentUseCase` + `GetStudentUseCase` + endpoints.
4. `RegisterClientUseCase` with students, `ClientDetailsResponse` in register and get.
5. `SearchStudentsUseCase` + endpoint.

## Implementation notes

- Status: backend done. Unit tests in `tests/ClassManager.Core.U.Tests/Domain/Students` and `UseCases/Students`, integration tests in `tests/ClassManager.Api.I.Tests/Endpoints/Students`.
- Validation field names come back as `students[1].FullName`: `ResultHttpExtensions` only lowercases the first letter. The app normalizes each segment.
- The student summary is a join of `Students` and `Clients`; both keep their tenant query filter. EF Core can't translate filters on a record built through its constructor, so the join goes through a small member-initialized class and the projection to `StudentSummary` is the last step.
- The `LIKE` escaping moved from `ClientRepository` to `LikePatterns`, shared by both searches.

## Out of scope

- Editing and deleting clients and students (next iteration; deleting needs a decision on history once attendance exists).
- Moving a student to another client.
- Medical certificate, level and emergency contact.
