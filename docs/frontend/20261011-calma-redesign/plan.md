# Calma redesign

Date: 2026-10-11

The app looks large and generic: Nunito at 800–900 almost everywhere, one shadowed card per class, student or payment (84 px rows, 20 px radii), and almost no motion. **Calma** is the new look the owner chose: the same aqua colors and the same screens, lighter type, no cards, hairlines instead of shadows, and short animations. It is a visual change only.

Designs (Claude artifacts, private to the owner):

- [Calma, app completa](https://claude.ai/artifact/MGjmVNd9Qav63zCt7WfueM): every main team screen, sign-in and sign-up, a component sheet and a clickable prototype.
- [Rediseño minimalista](https://claude.ai/artifact/8VsXqgpt8sphR6wDya5hzU): the three directions that were compared, the animation catalog and the font comparison.

## Rules

These rules apply to every pull request of this plan. A PR that breaks one is not merged.

### 1. No feature is removed

Removing functionality is forbidden. Every action, state, permission check, banner, empty state, toast, undo, confirm dialog, link and piece of information a screen has today stays, even when the design moves it. If a design leaves something out, the design is incomplete, not the feature: add it back in the Calma style. When a feature truly has to move to another place, the PR says where it went.

How it is checked:

- The [feature checklist](#feature-checklist) below lists what each screen does today. Every PR that touches a screen ticks its list in the PR description.
- Behavior tests (jest and e2e) are not deleted, skipped or weakened. They test behavior, not looks, so a visual change keeps them green. When a test has to change because the markup changed (a role, a label), the PR says why and the test still proves the same behavior.
- Accessibility labels, roles and `testID`s stay: tests and screen readers depend on them.

### 2. One component base

Screens never style things on their own. Everything visual comes from `app/src/ui` and the theme (see [Theming](../theming.md)):

- Reuse an existing primitive before writing anything. If two screens need the same look, it is a primitive.
- A new pattern (a timeline row, a counts row, a hairline list) becomes a primitive in `src/ui` with its own test, and every screen that shows that pattern uses it. No screen keeps the old version of a pattern another screen already moved to.
- Motion lives in shared primitives and constants (durations, easing), never in a screen. A screen asks for "a list that enters", "a pressable row" or "a sliding indicator"; it does not write `withTiming` itself.
- The lint rules in `app/eslint/themeRules.js` still apply: no raw colors, no raw font sizes, no `Text` outside `src/ui`.

### 3. Before and after screenshots for every change

Every PR shows how the app looked before and after it:

1. Before changing anything, capture the screens on `main` with [Demo screenshots](../../e2e/demo-screenshots.md): `pnpm screenshots` (team app), `pnpm screenshots:no-build Student_screens` (student app) and `pnpm screenshots:invitations` when sign-up or invitations are touched. Capture light and dark (`SCREENSHOTS_COLOR_SCHEME=dark`).
2. Make the change and capture the same screens again on the branch.
3. Commit both sets, at phone size, under `docs/frontend/20261011-calma-redesign/screenshots/<step>/before/` and `.../after/`, with the same file names.
4. The PR description has a table with one row per screen that changed: name, before, after.
5. When the whole plan is done, the last PR collects the first "before" and the last "after" of every screen in one table, to show the full change.

A screen that is not in the capture scripts gets added to them in the same PR, before the "before" capture.

### 4. Motion is short and optional

- 140 to 420 ms, one easing curve (`cubic-bezier(.2,.8,.2,1)`), a soft spring only for toasts and selected marks.
- Every animation is off when the device has "Reduce motion" on (Reanimated `useReducedMotion`). Content still appears, just without movement.
- Only the live class pulses in a loop. Nothing animates while the user reads (no animated numbers in lists).
- Layouts keep their fixed heights where they have them today (Inicio has no layout jumps between days, weeks and months, see [Team home](../20261001-team-home/plan.md)).

## The Calma look

| | Today | Calma |
|---|---|---|
| Font | Nunito 600 / 700 / 800 / 900 | Figtree 400 / 500 / 600 (700 only for initials and badges) |
| Type scale | hero 32, display 26, title 20, large 17, body 15, caption 13, small 12, micro 11 | hero 28, display 24, the rest unchanged (steps stay at least 2 px apart, see [Theming](../theming.md#typography)); the lighter weights do most of the work |
| Lists | one shadowed card per item, 12 px apart | rows separated by 1 px lines (`border-subtle`), no card |
| Shadows | every card | only the floating button and dialogs |
| Radii | 16–24 px | 10 px inputs, 12 px buttons, 14 px highlights, 18–20 px sheets and dialogs |
| Buttons | 52 / 48 px | 48 / 44 px |
| Agenda row | 84 px card with a left color bar | timeline row: time, a dot on a line, title, meta, status as colored text |
| Summaries (cuotas, pedidos) | filled primary card | large number, thin progress bar, status line in color |
| Banners | bordered | tonal background, no border |
| Status pills | 12 px | 11 px, same tones |
| Icons | 24–26 px | 20–22 px, thinner stroke |
| Tab bar | pill behind the active icon | active color and a small dot, red badge on Cobros |
| Floating button | 56 px, 18 px radius | 48 px pill with icon and label (the label stays) |

Colors do not change: the aqua tokens, the other built-in themes and the brand theme keep working. Contrast rules (`When_any_theme_is_defined`) still apply.

## Animations

| Animation | Where | Primitive |
|---|---|---|
| List rows enter (fade + 8 px up, 380 ms, 45 ms stagger, max 8 rows) | every list | `AnimatedListItem` |
| Press (scale 0.97, 140 ms, light tint) | rows, buttons, floating button, chips | `PressableScale` inside `Button`, `ListRow`, `Card`, `FloatingActionButton`, `Chip` |
| Sliding indicator (320 ms) | `SegmentedControl`, `Tabs`, filter chips, `DayStrip`, Inicio week strip, tab bar | `SlidingIndicator` |
| Mark present / absent (fill + check, spring) and row tint | Asistencia | `AttendanceRow` mark buttons; the existing swipe stays |
| Progress grows (700–900 ms) | cupos, cuotas, pedidos, sign-up steps | `ProgressBar`, `StepProgress` |
| Live pulse (1.8 s loop) | the running class in Inicio | `LivePulse` |
| Toast slides up with a spring, undo included | `ToastProvider` | `ToastProvider` |
| Dialog fades and scales from 0.96 | `ConfirmDialog` | `ConfirmDialog` |
| Sheet slides up, backdrop fades | `BottomSheet` | `BottomSheet` |
| Panels open and close by height | Asistencia panels, order payment and refund | `Collapsible` |
| Skeleton instead of a spinner in lists | lists while loading | `SkeletonList` |
| Screen push (short slide) | stack navigation | navigation options |

## Steps

Each step is its own PR, with its before/after screenshots.

0. **Screenshot coverage.** Add the screens that the capture scripts miss: welcome, sign-in (and its error), both sign-up steps, forgot password and its confirmation, Inicio in month view, a private lesson, team notifications, order list with an open payment panel, and a confirm dialog. No visual change.
1. **Base.** Figtree (`@expo-google-fonts/figtree`) and the new weights in `typography.ts` and `tailwind.config.js`, the smaller type scale, no card shadows (`elevation.ts`), flat `Card`, hairline `ListRow` and `ListDivider`, button, input, icon and tab bar sizes. Update [Theming](../theming.md) (visual language and type scale tables). This step alone changes every screen.
2. **Motion primitives.** `src/ui/motion/` with the durations and easing constants, `useReducedMotion`, and the primitives in the table above, wired into the existing `src/ui` components. Tests prove each primitive renders its content with reduced motion on.
3. **Team screens with their own layout.** Inicio (timeline, live strip, calendar), Asistencia (counts row, marks, panels), Clases, Alumnos, client card, Cobros (Cuotas and Pedidos), Registrar pago, Ajustes, sign-in and sign-up. New patterns go to `src/ui` first.
4. **Student app.** The student app follows Calma too, through the same primitives (it already shares `src/ui`). Its screens (home, clases, novedades, tienda, carrito, pedidos, logros, ajustes) are drawn in Calma in the design artifact first, reviewed by the owner, then built.
5. **Sweep.** Walk every screenshot of both apps, light and dark and every built-in theme, and fix what still looks like the old style (secondary forms: nueva clase, editar alumno, packs, productos, equipo, roles, sedes, importar y exportar, mi perfil, venta en mostrador).

## Feature checklist

What each screen does today, read from the code on 2026-10-11. The Calma version must keep every item.

### Welcome, sign-in, sign-up, forgot password

- Welcome: title, subtitle, "Registrá tu negocio" and "Ya tengo cuenta"; brand welcome when the business has one.
- Sign-in: email (prefilled from a link), password with show/hide, button grey until both are filled, loading state; banners for wrong credentials, locked out (warning), network error with "Reintentar", unexpected error; server field errors; "¿Olvidaste tu contraseña?" passes the typed email; link to sign-up; back goes to welcome.
- Sign-up step 1: name, birth date with the adult hint, email, password with the 10-character hint that turns satisfied, progress that fills as fields are valid, "Continuar" grey until filled and validated.
- Sign-up step 2: business name, country picker (prefilled from the device time zone, sets time zone and currency), "Crear cuenta" grey until filled and a currency is known, loading; banners for email already taken (with "Ingresar con este email"), network (retry) and unexpected; server errors on step 1 fields go back to step 1; back from step 2 goes to step 1; link to sign-in; "* Obligatorio" legend.
- Forgot password: hint, email, "Enviar link", the sent message with the email and the 24-hour expiry, "Volver a ingresar".

### Inicio

- Logo and brand name, "Hola, <name>" (or "Inicio"), avisos bell with unread count, subscription notice.
- Today banner: live class with "Asistencia", next class with time left (opens it), or "No te quedan clases por hoy".
- Hoy / Mañana / Ayer / En N días / Hace N días, short date, Día / Mes switch, week and month arrows.
- Week strip with today marked and a dot on days with classes; month view with pending attendance (amber) and cancelled days (red); picking a day in Mes returns to Día.
- "N clases · M alumnos" or "Sin clases", and the "Hoy" pill (hidden but kept in place on today).
- Rows: start and end, class or private lesson students, students count and coach or substitute, status (En curso with present count, Asistencia ✓, Falta asistencia, Programada, Particular, Suspendida, rescheduled original time, time left today). Each row opens the session or the private lesson.
- The previous list stays dimmed while another day loads; pull to refresh; error banner with retry.
- Free day: "Día libre" with a "Particular" button; "Nueva particular" floats only when the day has classes and the member can schedule lessons.

### Asistencia (session)

- Class name, long date, time and instructor; error banner with retry.
- Counts: vinieron, faltaron, sin marcar (with an accessible summary).
- Banners: cancelled with reason and "Reactivar clase", rescheduled with "Volver al horario habitual", substitute with "Quitar reemplazo", "Todavía no es el día de la clase", save failed.
- Swipe hint, "Todos vinieron" (skips students who warned they won't come), no students message.
- Rows: student, status and responsible, "Avisó que no viene", makeup, pack booking, Comentar / Ver comentario, Vino and Faltó buttons that toggle, swipe to mark; disabled without permission or before the day.
- Comment sheet: save, delete with undo, hint that the student sees it.
- Deliveries for the class ("Entregado").
- Panels (with permission): change time that day, assign substitute, cancel the class with a reason (blocked when attendance was taken).

### Clases and class detail

- "N clases por semana", day strip with dots and today, class rows (time, duration, name, "Completa", coach, place, capacity bar and count), empty day message, "Nueva clase" with permission, pull to refresh, error with retry.
- Detail: edit (with permission), name and details, "N de M lugares", free count, capacity bar, "Inscribir alumno" (disabled and relabeled when full), roster with dar de baja behind `ConfirmDialog`, last day shown after unenrolling, can be enrolled again, error banner.

### Alumnos and client card

- Search by student, responsible or phone, no results message, empty state, locked notice at the plan's student limit, error with retry, "Nuevo alumno" with permission, rows with age, responsible and notes.
- Card: edit, avatar, name, phone, email, Llamar, WhatsApp, Email, notes, invite to the student app (client and each student, with the minor rules), Clases / Cobro tabs (only with payment permission), each student's classes, add student or "Agregar hijo/a u otra persona", counter sale.
- Cobro: billing plan with upcoming changes (delete with undo), month balance, "Registrar pago", payments with delete and undo, class packs (sell, usage, expiry, "Anular" behind a confirm dialog, kept classes).

### Cobros

- No payment permission: locked empty state. No orders permission: only Cuotas.
- Cuotas / Pedidos switch with the pending orders count.
- Cuotas: month with arrows, missing default fee banner with "Definir cuota mensual", collected of total, debtors and amount (or everybody paid / no fee), amount from clients now paying per class, Deben / Todos filters with counts, rows (Debe, Pagó X de Y, Pagó, sin cuota) opening the payment, "Nadie debe este mes" and empty month states, "Pagan por clases" rows, refetch on focus.
- Pedidos: collected in the month, unpaid count and amount, to hand over count, pack sales, filters (Todos, Pendientes de pago, Para retirar) with empty texts, order rows (client, total, number, date, from the app, lines, refunds, delivery place, status and Listo pills), Cobrar (payment panel), Avisar que está listo, Entregar, Devolver (refund panel), "Venta en mostrador".
- Registrar pago: closes with ✕, client and month fee, already paid, amount prefilled with the balance, overpaid warning, method, date, notes, "* Obligatorio", button grey until filled.

### Ajustes

- Business identity row (opens business settings with permission), every group and row visible today with its values (Marca, Plan, Sedes, Cuota mensual or "Sin definir", Equipo, Roles, Profes, Logros y asistencia, Novedades, Packs, Productos, Pedidos, Apariencia, Notificaciones, Importar y exportar, Mi perfil, Cambiar de cuenta, Cerrar sesión), rows hidden by permission stay hidden, the appearance and notification sheets, app version.

### Student app

Every screen listed in [Demo screenshots, student app](../../e2e/demo-screenshots.md#student-app-screens) keeps its content and actions: student picker, next class, streak, monthly fee, comments, news, "No voy" and "Voy a ir igual", makeups, pack bookings, shop with search, categories, sizes, sold-out items and cart, orders in progress and past, achievements, settings and account choice.

## Open questions

- Sign-up step 2: the line under "País" in the design ("Hora de Buenos Aires · cobrás en pesos (ARS)") is new copy; confirm it or keep today's picker text.
- Pedidos: the toast after "Avisar que está listo" in the design is new copy; use the existing `orders.readyNotified` text.
- The design does not draw the free-day "Particular" button, the month view day colors or the dimmed list while loading; they stay (see the checklist).
