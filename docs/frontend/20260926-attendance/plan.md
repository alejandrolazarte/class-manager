# Frontend plan — Attendance (M4)

A new first tab "Hoy": the classes of the day and, inside each one, the enrolled students with two big buttons, "Vino" and "Faltó". One tap per student.

Backend contract: [backend/20260926-attendance](../../backend/20260926-attendance/plan.md).

## Navigation

```text
app/(tabs)/
  today/
    _layout.tsx
    index.tsx                              → DayScreen (?date=YYYY-MM-DD, today by default)
    [classGroupId]/[sessionDate].tsx       → SessionScreen
  classes/ students/ settings/             ← unchanged
```

```ts
routes.today                         = "/today"
routes.session(classGroupId, date)   = "/today/{classGroupId}/{date}"
```

Tab order: Hoy, Clases, Alumnos, Ajustes. Sign in keeps landing on `/students` (changing it is a one-line decision for later; the e2e tests expect it).

## Screens

### Day — `today/index.tsx`

- Header "‹ Martes 29 de septiembre ›" with previous and next day; "Hoy" link when another day is shown.
- Each class: "18:00–18:45 · Natación inicial", then "Laura Gómez · 4 de 6 presentes", or "Cancelada: Feriado".
- Tap → session. Empty day: "No hay clases este día".

### Session — `today/[classGroupId]/[date].tsx`

- Header: class, date and time.
- Each student: name (and "A cargo de …" when someone else pays) with two chips, "Vino" and "Faltó". Tapping the selected chip again clears it. The change shows immediately and is sent in the background; on error it reverts and shows a banner.
- Counter "4 vinieron · 1 faltó · 1 sin marcar".
- Future date: chips disabled with "Todavía no es el día de la clase".
- "Cancelar clase ese día" → reason field (optional, "Feriado", "Pileta cerrada") → confirm. Cancelled: banner "Clase cancelada: Feriado" and "Reactivar clase". With attendance taken the cancel button explains "Ya tomaste asistencia; borrá las marcas para cancelar".

## Folder structure

```text
src/features/sessions/
  types.ts, sessionsApi.ts, sessionQueryKeys.ts, sessionErrorCodes.ts
  dates.ts                      → add days, ISO date, long Spanish label ("Martes 29 de septiembre")
  useDaySessions.ts, useSession.ts, useSessionMutations.ts (optimistic mark)
  components/DaySessionCard.tsx, components/AttendanceRow.tsx, components/CancelSessionPanel.tsx
  screens/DayScreen.tsx, screens/SessionScreen.tsx
```

## Tests (write first)

```text
src/features/sessions/__tests__/
  When_student_is_marked_present/Then_request_is_sent_and_chip_is_selected.test.tsx
  When_selected_chip_is_pressed_again/Then_mark_is_cleared.test.tsx
  When_marking_fails/Then_mark_reverts_and_error_is_shown.test.tsx
  When_session_is_in_the_future/Then_attendance_chips_are_disabled.test.tsx
  When_session_is_cancelled/Then_banner_and_restore_are_shown.test.tsx
  When_next_day_is_pressed/Then_sessions_of_that_day_are_requested.test.tsx
  When_date_is_labeled/Then_it_uses_spanish_weekday_and_month.test.ts
```

## Implementation notes

- Status: done. Tests in `src/features/sessions/__tests__`.
- The day screen keeps the shown date in local state (initialized from `?date=`), so the arrows don't push a new screen per day.
- A mark shows immediately from local pending state; on success the session cache is updated in place and the day counts are refetched; on error the pending mark is dropped and a banner shows.
- `todayIsoDate` moved to `src/features/sessions/dates.ts` and is shared with the class roster.
- Pending: manual check on Android (Expo Go).

## Out of scope

- Marking everyone present with one tap (easy to add if the pilot asks).
- Offline attendance.
