# Frontend plan — Clients and students (M1)

The instructor thinks in students, not clients. The main tab becomes "Alumnos": search students, register someone new (an adult who attends, a parent with children, or both), and open the family card to add another child later.

Backend contract: [backend/20260925-clients-and-students](../../backend/20260925-clients-and-students/plan.md).

## Navigation

The template's `clients` tab becomes `students`:

```text
app/(tabs)/students/
  _layout.tsx                         → Stack
  index.tsx                           → StudentListScreen
  new.tsx                             → RegisterClientScreen (with "who attends")
  clients/[clientId]/index.tsx        → ClientDetailScreen (contact + students)
  clients/[clientId]/new-student.tsx  → AddStudentScreen
```

```ts
routes.students               = "/students"
routes.registerClient         = "/students/new"
routes.clientDetail(clientId) = "/students/clients/{clientId}"
routes.addStudent(clientId)   = "/students/clients/{clientId}/new-student"
```

Sign up lands on `routes.students`.

## Folder structure

```text
src/features/
  clients/                      ← existing; the form gains the "who attends" section
    registerClientSchema.ts     → clientAttends + additionalStudents, at least one attendee
    components/ClientForm.tsx
    components/AttendeesSection.tsx
    screens/RegisterClientScreen.tsx
    screens/ClientDetailScreen.tsx → adds the students list and "Add student"
  students/
    types.ts                    → Student, StudentSummary, NewStudentRequest
    studentsApi.ts              → addStudent, searchStudents
    studentQueryKeys.ts
    studentErrorCodes.ts
    studentSchema.ts            → fullName, birthDate (dd/mm/yyyy), notes; same limits as backend
    birthDateFormatting.ts      → parse dd/mm/yyyy to ISO date, age in years
    useStudentSearch.ts
    useAddStudent.ts
    components/StudentFields.tsx   → name, birth date, notes (reused by register and add)
    components/StudentListItem.tsx
    screens/StudentListScreen.tsx
    screens/AddStudentScreen.tsx
```

## Screens

### Student list — `students/index.tsx`

- Search box (debounced 300 ms): student name, parent name or phone.
- Item: student name, age when the birth date is known, and "A cargo de {client}" when the client is someone else.
- Tap → client detail. "+" → register.
- Empty state: "Registrá tu primer alumno".

### Register — `students/new.tsx`

1. Contact: name, phone, notes, and email under "Más detalles" (the existing client fields). Section title "Contacto".
2. "¿Quién viene a clase?":
   - Switch "{name} viene a clase", on by default. On sends one student with the contact's name.
   - "Agregar hijo/a u otra persona" adds a block of `StudentFields` (name, birth date, notes) with a remove button. Up to 10 students in total.
3. Submit "Registrar".

| Situation | UX |
|---|---|
| Nobody attends (switch off, no extra students) | Error under the section: "Indicá quién viene a clase" |
| Two students with the same name | Error under the second one (client-side and from the API) |
| `400` with `students[1].fullName` | Error under that student's field |
| `409 client.phone_number_taken` | Unchanged: banner with "Abrir cliente", which now opens the family card to add the student there |
| `201` | Toast "Alumno registrado" (plural when more than one), go to the client detail |

Birth date is a plain text field with a `dd/mm/aaaa` mask: no native date picker dependency, and typing a year is faster than scrolling back to 2018.

### Client detail — `students/clients/[clientId]/index.tsx`

The existing card (name, phone, call, WhatsApp, email, notes) plus a "Alumnos" section: each student with age and notes, and a button "Agregar alumno".

### Add student — `students/clients/[clientId]/new-student.tsx`

`StudentFields` and a submit button. `409 student.already_registered` shows "Ya hay un alumno con este nombre" under the name. `201` invalidates the client and student queries and goes back to the client detail.

## Tests (write first)

```text
src/features/clients/__tests__/
  When_nobody_attends/Then_attendee_error_is_shown.test.tsx
  When_client_attends/Then_request_includes_client_as_student.test.tsx
  When_children_are_added/Then_request_includes_each_child.test.tsx
  When_api_returns_student_field_error/Then_error_is_shown_under_that_student.test.tsx
src/features/students/__tests__/
  When_birth_date_is_typed/Then_it_is_parsed_to_iso_date.test.ts
  When_adding_student_with_taken_name/Then_name_error_is_shown.test.tsx
  When_student_search_text_changes/Then_request_is_debounced.test.tsx
  When_student_belongs_to_another_client/Then_responsible_name_is_shown.test.tsx
```

The existing client tests move to the new routes and keep passing.

## Implementation order

1. Routes and the `students` tab; move the existing screens.
2. `students` API, types, schema and birth date helpers.
3. "Who attends" in the register form.
4. Student list with search.
5. Students section in the client detail and the add student screen.
6. Manual check on web against the local API.

## Out of scope

- Editing and deleting clients and students.
- A student detail screen (arrives with enrollments in M3).
- Native date picker.
