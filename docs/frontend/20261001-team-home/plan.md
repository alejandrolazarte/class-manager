# Team home (Inicio)

Date: 2026-10-01

The team app opened on Alumnos and its first tab was Hoy, a day agenda. The family app has a home (greeting, next class hero, tiles, bell) that the owner liked, so the team app now reuses that layout. **Inicio replaces Hoy** in the tab bar without losing anything Hoy did.

## What Inicio shows

1. **Header:** the business logo, the brand name, "Hola, <first name>" and the avisos bell ([team notifications](../../backend/20261001-team-notifications/plan.md)).
2. **Próxima clase:** the next class of the day that hasn't ended (cancelled ones are skipped), with its time, class, place, number of students, an "Ahora" badge while it's running, and **Tomar asistencia**. When nothing is left: "No te quedan clases por hoy."
3. **Tiles:** **Hoy**, with the day's classes and expected students, and **Avisos**, with unread avisos or "Al día", which opens the Avisos list.
4. **Agenda:** everything Hoy had: the day title (Hoy / Mañana / Ayer / Agenda) and long date, month view toggle, previous and next day, the week strip, "Volver a hoy", the day's classes and private lessons, pull to refresh, and **Nueva particular**.

## Decisions

- The route stays `/today`, so session links, avisos URLs and push notifications keep working; only the tab label ("Inicio") and icon (home) change.
- The team app now opens on Inicio after sign-in, sign-up and when the root URL is opened (`homeRouteOf`, `app/index.tsx`), the same as the family app.
- `GET /api/me` returns the member's `fullName` (from the identity account) for the greeting; when it's missing, the title is "Inicio".
- The bell moved from Alumnos to Inicio. Alumnos keeps the logo and brand name.
