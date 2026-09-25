# Frontend plan — Enrollments (M3)

The class becomes the place to manage who comes: tapping a class opens its roster, with free spots, "Inscribir alumno" and "Dar de baja". The family card shows each student's classes.

Backend contract: [backend/20260925-enrollments](../../backend/20260925-enrollments/plan.md).

## Navigation

```text
app/(tabs)/classes/
  [classGroupId]/index.tsx     → ClassGroupDetailScreen (new: roster)
  [classGroupId]/edit.tsx      → ClassGroupFormScreen (moved from [classGroupId].tsx)
  [classGroupId]/enroll.tsx    → EnrollStudentScreen (search and pick a student)
```

```ts
routes.classGroup(classGroupId)      = "/classes/{classGroupId}"
routes.editClassGroup(classGroupId)  = "/classes/{classGroupId}/edit"
routes.enrollStudent(classGroupId)   = "/classes/{classGroupId}/enroll"
```

## Screens

### Weekly view

Each card shows "5/8 lugares" and a "Completa" chip when the class is full.

### Class detail — `classes/[classGroupId]`

- Header: name, days and time, instructor, location, "5 de 8 lugares".
- Roster: student name and age, "A cargo de {client}" when someone else pays, "Empieza el {date}" for future starts.
- "Inscribir alumno" (disabled with "Clase completa" when full) and "Editar clase".
- Each student: "Dar de baja" → confirmation "¿Dar de baja a Tomás de Natación inicial?" → ends today.

### Enroll student — `classes/[classGroupId]/enroll`

- Student search (the same search as the Alumnos tab).
- Already enrolled students are marked "Ya inscripto" and disabled.
- Tap → enroll from today, toast "Tomás inscripto en Natación inicial", back to the class.
- `409 class_group.full` → banner "La clase está completa"; `409 enrollment.already_enrolled` → "Ya está inscripto".

### Family card (client detail)

Under each student: their classes ("Natación inicial · Mar y Jue 18:00"), or "Sin clases".

### Class edit

Deactivating a class with students shows "Tiene {count} alumnos inscriptos. Dalos de baja primero." Lowering capacity below the roster shows the server message under Cupo.

## Folder structure

```text
src/features/enrollments/
  types.ts, enrollmentsApi.ts, enrollmentQueryKeys.ts, enrollmentErrorCodes.ts
  useClassRoster.ts, useStudentEnrollments.ts, useEnrollmentMutations.ts
  weekdaySummary.ts               → "Mar y Jue 18:00"
  components/RosterItem.tsx, components/StudentClasses.tsx
  screens/ClassGroupDetailScreen.tsx, screens/EnrollStudentScreen.tsx
```

## Tests (write first)

```text
src/features/enrollments/__tests__/
  When_class_is_full/Then_enroll_button_is_disabled.test.tsx
  When_student_is_picked/Then_enrollment_is_created_and_class_is_shown.test.tsx
  When_student_is_already_enrolled/Then_it_is_marked_and_disabled.test.tsx
  When_unenrolling_is_confirmed/Then_enrollment_is_ended.test.tsx
  When_weekdays_are_summarized/Then_they_use_short_names_in_week_order.test.ts
src/features/classGroups/__tests__/
  When_class_has_enrollments/Then_card_shows_enrolled_over_capacity.test.tsx
```

## Implementation notes

- Status: done. Tests in `src/features/enrollments/__tests__` and the enrolled count test in `src/features/classGroups/__tests__`.
- Tapping a class in the weekly view now opens the roster; the form moved to `classes/[classGroupId]/edit`.
- The unenroll confirmation is an in-screen banner instead of a native alert, so it works the same on web and in tests.
- "Empieza el …" compares the start date with the device's date; the API's roster already uses the business's today.
- Pending: manual check on Android (Expo Go).

## Out of scope

- Choosing a future start date in the UI (the API supports it; the app enrolls from today).
- Enrolling from the family card.
- Waitlist.
