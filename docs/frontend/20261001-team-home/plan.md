# Team home (Inicio)

Date: 2026-10-01

The team app opened on Alumnos and its first tab was Hoy, a day agenda. The family app has a home (greeting, next class hero, tiles, bell) that the owner liked, so the team app now reuses that layout. **Inicio replaces Hoy** in the tab bar without losing anything Hoy did.

## What Inicio shows

Follows the Claude Design "Profesor Hoy" (inicio del profe sin saltos de layout): every block has a fixed height, so moving between days, weeks and months doesn't shift the screen.

1. **Header:** the business logo, the brand name, "Hola, <first name>" and the avisos bell with the unread count. Inicio is the only team screen with the logo.
2. **Today banner (72 px):** "En curso · hasta 18:00" with the class and an **Asistencia** button while a class runs; otherwise "Próxima clase · en 2 h 49 min" with the time and class (tapping opens it); or "No te quedan clases por hoy".
3. **Agenda header (48 px):** Hoy / Mañana / Ayer / En N días / Hace N días (or "Calendario" in month view), the short date ("Jue 1 oct") or the month, and a **Día / Mes** switch.
4. **Calendar card:** one card for both views. Día shows the selected week (Lun Mar Mié Jue Vie Sáb Dom, a dot on days with classes, today outlined); Mes shows six weeks (pending attendance in amber, cancelled days in red). The arrows move a week or a month; picking a day in Mes goes back to Día.
5. **Summary row (28 px):** "N clases · M alumnos" or "Sin clases", and a **Hoy** pill to return to today (kept in place, invisible, when already on today).
6. **Classes (84 px each, at least 276 px of list):** start and end time, class (or private lesson students), students and coach (or who the substitute replaces), and a status: En curso (present count), Asistencia ✓ (who came), Falta asistencia, Programada / Particular (time left today, or the original time when rescheduled) or Suspendida. While another day loads, the previous list stays dimmed instead of jumping to a spinner.
7. **Free day:** "Día libre · No hay clases programadas" with a **Particular** button; the **Nueva particular** button floats only when the day has classes.

## Decisions

- The route stays `/today`, so session links, avisos URLs and push notifications keep working; only the tab label ("Inicio") and icon (home) change.
- The team app now opens on Inicio after sign-in, sign-up and when the root URL is opened (`homeRouteOf`, `app/index.tsx`), the same as the family app.
- `GET /api/me` returns the member's `fullName` (from the identity account) for the greeting; when it's missing, the title is "Inicio".
- The bell moved from Alumnos to Inicio, and the logo and brand name only appear on Inicio: Alumnos is back to its plain header.
- The day arrows were replaced by week (or month) arrows, as in the design; a single day is picked in the calendar.
- The "Hoy" and "Avisos" tiles of the first version were dropped: the bell already shows unread avisos.
