# Frontend plan — Instructors and class groups (M2)

The instructor sees their week: a new first tab "Clases" with the classes of each day, and a form to create or edit a class. Instructors are managed in Ajustes, like employees in salon-manager.

Backend contract: [backend/20260925-instructors-and-class-groups](../../backend/20260925-instructors-and-class-groups/plan.md).

## Navigation

```text
app/(tabs)/
  classes/                              ← new first tab "Clases"
    _layout.tsx
    index.tsx                           → WeeklyClassesScreen
    new.tsx                             → ClassGroupFormScreen (create)
    [classGroupId].tsx                  → ClassGroupFormScreen (edit, activate/deactivate)
  students/                             ← unchanged
  settings/
    instructors/index.tsx               → InstructorListScreen
    instructors/new.tsx                 → InstructorFormScreen
    instructors/[instructorId].tsx      → InstructorFormScreen (rename, activate/deactivate)
```

```ts
routes.classes                   = "/classes"
routes.newClassGroup             = "/classes/new"
routes.classGroup(classGroupId)  = "/classes/{classGroupId}"
routes.instructors               = "/settings/instructors"
routes.newInstructor             = "/settings/instructors/new"
routes.instructor(instructorId)  = "/settings/instructors/{instructorId}"
```

Sign up and sign in keep landing on `/students`: a new business has students to load before classes make sense. The tab order is Clases, Alumnos, Ajustes.

## Folder structure

```text
src/features/
  instructors/                   ← port of salon-manager's employees, without colors and hours
    types.ts, instructorsApi.ts, instructorQueryKeys.ts, instructorErrorCodes.ts
    instructorSchema.ts
    useActiveInstructors.ts, useInstructorsIncludingInactive.ts, useInstructorMutations.ts
    components/InstructorListItem.tsx
    screens/InstructorListScreen.tsx, screens/InstructorFormScreen.tsx
  classGroups/
    types.ts, classGroupsApi.ts, classGroupQueryKeys.ts, classGroupErrorCodes.ts
    classGroupSchema.ts            → name, instructorId, weekdays, startTime (HH:mm), duration, capacity, location
    weekdays.ts                    → order Monday–Sunday, short and long Spanish labels, today's weekday
    classesOfDay.ts                → filter active groups by weekday, sort by start time
    useClassGroups.ts, useClassGroupMutations.ts
    components/WeekdayChips.tsx    → single select (weekly view) and multi select (form)
    components/ClassGroupCard.tsx
    components/ClassGroupForm.tsx
    screens/WeeklyClassesScreen.tsx, screens/ClassGroupFormScreen.tsx
```

`DurationChips` from salon-manager is ported with class-friendly presets (30, 45, 60, 90 minutes).

## Screens

### Weekly classes — `classes/index.tsx`

- A row of day chips (Lun … Dom), today selected by default.
- The day's active classes ordered by start time: "18:00–18:45 · Natación inicial", then "Laura Gómez · 8 lugares · Pileta chica".
- Tap → edit. "+" → new class, with the selected day preselected.
- Empty day: "No hay clases el martes" and "Crear clase". No classes at all: "Creá tu primera clase".

### Class form — `classes/new.tsx` and `classes/[classGroupId].tsx`

Fields, in this order:

1. Name — "Natación inicial", "Yoga suave".
2. Days — multi-select chips.
3. Start time — `HH:mm` text field with mask, numeric keyboard.
4. Duration — chips 30 / 45 / 60 / 90 and "Otra" for a custom value.
5. Capacity — numeric.
6. Instructor — chips of active instructors; preselected when there is only one.
7. Location — optional ("Pileta chica", "Sala 2").

| Situation | UX |
|---|---|
| `409 class_group.instructor_busy` | Banner "{instructor} ya da {other class} a esa hora" with the other class's time, fields kept |
| No active instructors | Banner with "Agregar profe" linking to settings (only reachable if the owner deactivated themselves) |
| Edit | Same form plus "Desactivar clase" / "Activar clase" |
| Saved | Toast "Clase guardada", back to the weekly view on the class's first day |

### Instructors — `settings/instructors`

List with active first and an "Inactivo" chip, "+" to add, tap to rename. Deactivating an instructor who still teaches shows "Primero asigná sus {count} clases a otro profe o desactivalas".

## Tests (write first)

```text
src/features/classGroups/__tests__/
  When_day_is_selected/Then_only_that_days_classes_are_shown_in_time_order.test.tsx
  When_class_form_is_submitted_without_days/Then_days_error_is_shown.test.tsx
  When_start_time_is_typed/Then_it_is_formatted_as_hours_and_minutes.test.ts
  When_instructor_is_busy/Then_conflict_banner_names_the_other_class.test.tsx
  When_class_is_created/Then_request_has_weekdays_and_start_time.test.tsx
src/features/instructors/__tests__/
  When_deactivating_instructor_with_classes/Then_message_is_shown.test.tsx
  When_listing_instructors/Then_inactive_ones_are_last.test.tsx
```

## Implementation order

1. Instructors feature and settings screens (port).
2. `weekdays.ts`, start time helpers, `WeekdayChips`.
3. Class form with create and edit.
4. Weekly view and the new tab.
5. Manual check on web against the local API.

## Out of scope

- A calendar grid (week at a glance with hours); the per-day list is enough for a handful of classes.
- Copying a class, colors per instructor.
- Enrolled count per class (M3).
