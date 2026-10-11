# Calma redesign

Date: 2026-10-11

The app looks large and generic: Nunito at 800–900 almost everywhere, one shadowed card per class, student or payment (84 px rows, 20 px radii), and almost no motion. **Calma** is the new look the owner chose: the same aqua colors and the same screens, lighter type, no cards, hairlines instead of shadows, and short animations. It is a visual change only.

Designs (Claude artifacts, private to the owner):

- [Calma, app completa](https://claude.ai/artifact/MGjmVNd9Qav63zCt7WfueM): every main team screen, sign-in and sign-up, a component sheet and a clickable prototype.
- [Rediseño minimalista](https://claude.ai/artifact/8VsXqgpt8sphR6wDya5hzU): the three directions that were compared, the animation catalog and the font comparison.
- [Calma, antes y después](https://claude.ai/artifact/AMoHSyDZQNwkEszSW6NqkJ): the before/after screenshots of the steps with important changes (rule 4); step 0 holds the baseline of every screen of both apps, light and dark, and every email.

## Rules

These rules apply to every pull request of this plan. A PR that breaks one is not merged.

### 1. No feature is removed

Removing functionality is forbidden. Every action, state, permission check, banner, empty state, toast, undo, confirm dialog, link and piece of information a screen has today stays, even when the design moves it. If a design leaves something out, the design is incomplete, not the feature: add it back in the Calma style. When a feature truly has to move to another place, the PR says where it went.

How it is checked:

- The [feature checklist](#feature-checklist) below lists what each screen does today. Every PR that touches a screen ticks its list in the PR description.
- Behavior tests (jest and e2e) are not deleted, skipped or weakened. They test behavior, not looks, so a visual change keeps them green. When a test has to change because the markup changed (a role, a label), the PR says why and the test still proves the same behavior.
- Accessibility labels, roles and `testID`s stay: tests and screen readers depend on them.

### 2. Texts stay as they are

The redesign changes how the app looks, never what it says. Every text in `app/src/i18n/es.json`, in the emails and in accessibility labels stays word for word: no value is edited and no key is added or removed for the redesign.

- The texts in the design artifacts are illustrative. Where a design shows a text that differs from the app, the app's current text wins (for example, the line under "País" in sign-up keeps "Zona horaria: … · Moneda: … · Código telefónico: +…" and its "Cambiar zona horaria" button; the toast after "Avisar que está listo" keeps "Le avisamos al alumno").
- Information that only the design shows (a student count under Alumnos, "Desde el …" in the class roster, values next to Ajustes rows that the app doesn't show today, the "Todavía no usa la app de alumnos" line in the client card) is not added. New information is a feature, with its own plan, not part of the redesign.
- A key whose last user disappears is removed under rule 5 only when the feature it belonged to moved, and rule 1 still holds: the same text keeps appearing wherever that feature is now.

### 3. One component base

Screens never style things on their own. Everything visual comes from `app/src/ui` and the theme (see [Theming](../theming.md)):

- Reuse an existing primitive before writing anything. If two screens need the same look, it is a primitive.
- A new pattern (a timeline row, a counts row, a hairline list) becomes a primitive in `src/ui` with its own test, and every screen that shows that pattern uses it. No screen keeps the old version of a pattern another screen already moved to.
- Motion lives in shared primitives and constants (durations, easing), never in a screen. A screen asks for "a list that enters", "a pressable row" or "a sliding indicator"; it does not write `withTiming` itself.
- The lint rules in `app/eslint/themeRules.js` still apply: no raw colors, no raw font sizes, no `Text` outside `src/ui`.

### 4. Before and after screenshots for important changes

Screenshots go in the gallery only when a change is important: a screen whose layout changes (a list that becomes a timeline, a card that becomes rows, a panel that moves), a new pattern, or an email. A change applied the same way everywhere through shared primitives and tokens (fonts, sizes, shadows, radii, motion), with no change in functionality, does not need them: the PR says which screens it checked by eye.

When a PR needs them:

1. Before changing anything, capture on `main` the screens the PR changes, with [Demo screenshots](../../e2e/demo-screenshots.md): `pnpm screenshots` (team app), `pnpm screenshots:no-build Student_screens` (student app) and `pnpm screenshots:invitations` when sign-up or invitations are touched. Capture light and dark (`SCREENSHOTS_COLOR_SCHEME=dark`).
2. Make the change and capture the same screens again on the branch.
3. Publish only the screens that changed, at phone size, in the [Calma, antes y después](https://claude.ai/artifact/AMoHSyDZQNwkEszSW6NqkJ) gallery: one section per step, each screen with its before and after side by side (light and dark). The images are not committed to the repository; `e2e/screenshots-output/` stays git-ignored.
4. The PR description links the gallery section of its step and lists the screens that changed.
5. When the whole plan is done, the gallery gets a final section with the first "before" and the last "after" of every main screen and email, to show the full change, and the last PR links it.

A changed screen that is not in the capture scripts gets added to them in the same PR, before the "before" capture. Emails are captured the same way through the email previews (step 0), at phone and desktop width.

### 5. Nothing of the old design is left, and nothing unused

The redesign replaces, it does not pile up. Each PR deletes what it replaces, in the same PR:

- Old variants, class names, styles and props that no screen uses anymore (for example the card level of `elevation.ts`, `Card` once no screen needs it, `AppText` variants or font roles nobody uses, unused icons in `iconGlyphs`, unused theme tokens).
- Fonts and packages that are no longer loaded (`@expo-google-fonts/nunito` and its assets once Figtree is in).
- Components, hooks, helpers, translation keys (`es.json`) and email fragments that lost their last user, with the tests that only covered them.
- No commented-out code, no `Old`/`Legacy`/`V2` copies, no feature flags to switch back to the old look.

How it is checked: before opening the PR, search for every name the PR replaced (`git grep`) and remove what has no users left; TypeScript, ESLint (`no-unused-vars`) and `dotnet build -warnaserror` stay clean. Step 0 adds [knip](https://knip.dev) as a dev dependency of `app/`, run in CI, so an unused file, export or dependency fails the build, plus a test that fails when an `es.json` key has no user (keys built at runtime, like `orders.filter.${filter}`, are listed by prefix).

### 6. Motion is short and optional

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

## Emails

Every email the app sends follows Calma too. They share one layout (`src/Notifications/Email/Templates/BrandedEmail.html` and the fragments in `BrandedEmailHtml`, see [Branded emails](../../backend/20261002-branded-emails/plan.md)), so the change happens there and every email takes it:

- Figtree, the app's font, with Arial as the fallback (`Figtree, Arial, Helvetica, sans-serif`). Apple Mail (iPhone and Mac), Samsung Mail, Thunderbird and Outlook for Mac show Figtree; Gmail, Outlook for Windows and Yahoo ignore web fonts and show Arial, which still looks right.
- The font is served from our own web app, never from Google Fonts: a request to Google would tell it who opened the email and from which IP (European courts have fined sites for loading Google Fonts without consent). The `.woff2` files go in `app/public/fonts/` with a stable, versioned name (Figtree is under the SIL Open Font License, which allows it), and the email layout points `@font-face` at `WebApp:Url` plus that path, the same base URL email links already use.
- Outlook for Windows can fall back to Times New Roman when it ignores a web font: the layout adds the usual `<!--[if mso]>` style that forces Arial for it.
- The layout must look right in both fonts: email previews are captured with the web font and with it blocked.
- Weight 600 instead of `bold`, lighter headings, the same type steps as the app.
- Items and totals separated by 1 px lines instead of boxes; the note as a tonal block; button 12 px radius and 44 px high; header with the logo or initials kept.
- Kept as they are: brand colors from `EmailPalette` (darkened until white text reads), logo as an inline attachment, plain-text alternative, link fallback under the button, HTML encoding, sender name, and every email type in the table of that plan.
- No motion in emails.

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

Each step is its own PR, with before/after screenshots when its changes are important (rule 4).

0. **Screenshot coverage.** Add the screens that the capture scripts miss: welcome, sign-in (and its error), both sign-up steps, forgot password and its confirmation, Inicio in month view, a private lesson, team notifications, order list with an open payment panel, and a confirm dialog. Add email previews: a small tool that renders every email type with demo data (with and without logo, with and without brand color, with the web font and with it blocked) to HTML files, and a capture that screenshots them at phone and desktop width. Add knip and the unused translation keys test (rule 5), removing what they find unused today in its own commit. Create the screenshots artifact with the baseline of every screen and email. No visual change.
1. **Base.** Figtree (`@expo-google-fonts/figtree`) and the new weights in `typography.ts` and `tailwind.config.js`, the smaller type scale, no card shadows (`elevation.ts`), flat `Card`, hairline `ListRow` and `ListDivider`, button, input, icon and tab bar sizes. Update [Theming](../theming.md) (visual language and type scale tables). This step alone changes every screen.
2. **Motion primitives.** `src/ui/motion/` with the durations and easing constants, `useReducedMotion`, and the primitives in the table above, wired into the existing `src/ui` components. Tests prove each primitive renders its content with reduced motion on.
3. **Team screens with their own layout.** Inicio (timeline, live strip, calendar), Asistencia (counts row, marks, panels), Clases, Alumnos, client card, Cobros (Cuotas and Pedidos), Registrar pago, Ajustes, sign-in and sign-up. New patterns go to `src/ui` first.
4. **Student app.** The student app follows Calma too, through the same primitives (it already shares `src/ui`). Its design is already in the [Calma artifact](https://claude.ai/artifact/MGjmVNd9Qav63zCt7WfueM), in the "App de alumnos" rows: Inicio, Clases, Novedades, Tienda, Producto, Carrito, Mis pedidos, Logros, Ajustes and the student tab bar, drawn from the current code with today's texts. Build it from those boards. Logros is only restyled here; making it more exciting is its own plan, [Logros that make progress exciting](../20261011-student-achievements/plan.md). Screens not drawn yet (Mi perfil, Editar perfil, pack detail, accepting invitations, authorizing a minor, choosing an account) take the same primitives.
5. **Emails.** The Calma layout for every email (see [Emails](#emails)), with before/after previews of each email type.
6. **Sweep and cleanup.** Walk every screenshot of both apps and every email, light and dark and every built-in theme, and fix what still looks like the old style (secondary forms: nueva clase, editar alumno, packs, productos, equipo, roles, sedes, importar y exportar, mi perfil, venta en mostrador). Then remove whatever the old design left behind (rule 5), so the plan ends with no unused code, and update [Theming](../theming.md) and [Branded emails](../../backend/20261002-branded-emails/plan.md) to describe only Calma.

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
- Pedidos: collected in the month, unpaid count and amount, to hand over count, pack sales, filters (Todos, Pendientes de pago, Para retirar) with empty texts, order rows (client, total, number, date, from the app, lines, refunds, delivery place, status and Listo pills), Cobrar (payment panel: method, confirm the payment, mark ready, "Cancelar pedido"), Avisar que está listo, Entregar, Devolver (refund panel), "Venta en mostrador".
- Registrar pago: closes with ✕, client and month fee, already paid, amount prefilled with the balance, overpaid warning, method, date, notes, "* Obligatorio", button grey until filled.

### Ajustes

- Business identity row (opens business settings with permission), every group and row visible today with the values it shows today (Marca, Plan, Sedes, Cuota mensual or "Sin definir", Equipo, Roles, Profes, Logros y asistencia, Novedades, Packs, Productos, Pedidos, Apariencia, Notificaciones, Importar y exportar, Mi perfil, Cambiar de cuenta, Cerrar sesión), rows hidden by permission stay hidden, app version.
- Apariencia sheet: Automático / Claro / Oscuro and the hint about who picks the colors ("Los colores de la app se eligen en Marca" for the owner, "Los colores de la app los elige el negocio" for the team). Notifications sheet.
- Marca: theme for everyone (a built-in theme or the business colors), main color with hex code, accent, the contrast and status-clash warnings, the adjusted-color note, logo, name students see, live preview, welcome preview, reset and save. The built-in themes keep working in Calma, since the business chooses them for students and team.

### Student app

Every screen listed in [Demo screenshots, student app](../../e2e/demo-screenshots.md#student-app-screens) keeps its content and actions: student picker, next class, streak, monthly fee, comments, news, "No voy" and "Voy a ir igual", makeups, pack bookings, shop with search, categories, sizes, sold-out items and cart, orders in progress and past, achievements, settings and account choice.

### Emails

- Team invitation and resend, family invitation, password reset (app brand), new order for the team, payment confirmed, order ready and order cancelled for families, sign-in and email-change emails, and any email added later.
- Each keeps its eyebrow, title, intro, items and total, note, button with link fallback, footer note, subject, sender name, logo or initials, brand colors and plain-text part.

## Decisions

- 2026-10-11: every current text stays (rule 2). The two texts the design invented (under "País" in sign-up, and the toast after "Avisar que está listo") are not used.
- 2026-10-11: the design does not draw the free-day "Particular" button, the month view day colors or the dimmed list while loading; they stay (see the checklist).
- 2026-10-11: colors are chosen by the business for everyone ([branding and plans](../../branding-and-plans.md)); Calma keeps the built-in themes and the brand theme, and only changes type, spacing, lines and motion.
- 2026-10-11: before/after screenshots live in a private Claude artifact (a gallery per step), not in the repository, so it doesn't grow.
- 2026-10-11: `knip` and an unused translation keys test are added in step 0, so unused code fails CI instead of depending on a manual search.
- 2026-10-11: the student app is drawn in Calma and reviewed by the owner before it is built (step 4). Done the same day: the boards are in the Calma artifact.
- 2026-10-11: Logros keeps today's content in the redesign; a more exciting Logros is a separate proposal ([Logros that make progress exciting](../20261011-student-achievements/plan.md)).
- 2026-10-11: "Cancelar pedido" (student and team) cancels without asking; adding a confirmation is a behavior change, so it is a separate pending plan ([Confirm before cancelling an order](../20261011-confirm-order-cancellation/plan.md)). The redesign keeps today's behavior.
- 2026-10-11: emails use Figtree with Arial as the fallback, the font served from our own web app (not Google Fonts) and Arial forced for Outlook for Windows.
- 2026-10-11 (step 0): email previews are a console project in the solution (`tools/EmailPreviews`), so it builds in CI and breaks when an email changes shape; the capture runs it, so emails need neither the API nor a database. The screenshots of the web font blocked are taken with `SCREENSHOTS_EMAIL_FONT=blocked`; until step 5 adds the web font they are identical to the others.
- 2026-10-11 (step 0): knip treats an export used only in its own file as used (`ignoreExportsUsedInFile`), and two Expo dependencies its plugin expects are ignored because the app does not use them; the reasons are in [Unused code](../unused-code.md).
- 2026-10-11 (step 0): team notifications and the order payment panel are captured in `Student_screens.capture.ts`, which seeds the student activity they need.
- 2026-10-11 (step 1): the font roles keep their names and move to Figtree: `text` 400, `label` 500, `strong` 600, `heavy` 700. Titles, amounts and names use `strong`; the `font-heavy` overrides screens had for prices and times are gone, and `heavy` is left for initials and badge numbers.
- 2026-10-11 (step 1): buttons and typed text use the `body` step (15 px), as the component sheet draws them, instead of `large`; the tab bar labels are 11 px at 500 (`tab` variant in the brand preview).
- 2026-10-11 (step 1): the active tab's dot goes under its icon, not under its label as the design draws it: a custom `tabBarLabel` would drop the accessibility label React Navigation gives each tab on iOS, and rule 1 keeps it.
- 2026-10-11 (step 1): until step 3 turns lists into hairline rows, `Card` stays as a flat surface with a 1 px `border-subtle` line and a 14 px radius, so items still read as separate on the tinted background. The card level of `elevation.ts` is gone; `useFloatingShadowStyle` is the only shadow.
- 2026-10-11 (step 1): screenshots only for important changes (rule 4). Step 1 changes fonts, sizes and shadows the same way on every screen without changing functionality, so it adds no gallery section; steps 3, 4 and 5 (screen layouts, the student app and emails) do, with only the screens they change.
- 2026-10-11 (step 2): the tab bar is drawn per tab by React Navigation, so its dot pops in with the soft spring instead of sliding between tabs; a custom tab bar would risk the per-tab accessibility labels (rule 1).
- 2026-10-11 (step 2): filter chips that pick one value are a `ChipGroup` with a sliding selection; inside a group, unselected chips are outlined on the background so the moving selection shows under them.
- 2026-10-11 (step 2): the row tint in Asistencia waits for the Asistencia layout in step 3; step 2 adds the spring fill to the Vino and Faltó marks.
- 2026-10-11 (step 2): dialogs, sheets and toasts animate when they appear and close at once, as they do today; panels animate both ways through `Collapsible`.
- 2026-10-11 (step 2): no screenshots (rule 4): motion doesn't show in a still image, and the only visual changes are the chip and tab indicators and the loading skeleton.
