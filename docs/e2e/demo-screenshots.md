# Demo screenshots

`e2e/screenshots/` loads a demo business through the API and captures every main screen of the web build at phone size (Pixel 7), both the team app and the student app. Use it to show the app, review a UI change, or check copy.

It reuses the e2e setup: the same web build, API start, database and Playwright install. It is **not** part of the e2e suite: it lives outside `e2e/tests/` and has its own config (`playwright.screenshots.config.ts`), so CI never runs it.

## What it loads

"Escuela de Natación Brazada" with owner Laura Gómez (also the first instructor) and:

- A second instructor, Martín Díaz.
- Four class groups: Natación inicial and Natación avanzada (Monday, Wednesday, Friday and whatever day the script runs, so "Hoy" always has classes), Aquagym and Natación adultos (Tuesday and Thursday).
- Six students, nine enrollments from the first day of the current month, and attendance for today.
- A default fee of $25.000, one student with its own fee, and payments that leave students paid, partially paid and owing.
- A raise to $28.000 from next month, three class packs (single class, 4 classes for a month, 8 classes for two months), and the Suárez client paying per class with a discounted 4-class pack and one class already used.

Every run signs up a new business with a unique email, so it can run against the same database many times.

## Run it

Same prerequisites as [the e2e tests](running-e2e-tests.md): SQL Server running and a dedicated database with migrations applied.

```powershell
cd e2e
$env:E2E_DATABASE_CONNECTION_STRING = "Server=tcp:[::1],1433;Database=ClassManagerE2E;User Id=sa;Password=<your-password>;TrustServerCertificate=True"
pnpm db:migrate
pnpm screenshots            # builds the web app, then captures
pnpm screenshots:no-build   # reuses the last web build
```

The images land in `e2e/screenshots-output/` (git-ignored); set `SCREENSHOTS_DIR` to write them somewhere else. Set `SCREENSHOTS_COLOR_SCHEME=dark` to capture the dark theme (light by default). The API and the web server start automatically, or are reused if already running on ports 5000 and 8081.

## Screens

| File | Screen |
|---|---|
| `01-hoy` | Today's classes with present counts |
| `02-asistencia` | Attendance of Natación inicial |
| `02b-comentario-del-profe` | Comment sheet an instructor fills in for a student after class |
| `03-clases-semana` | Weekly classes |
| `04-clase-alumnos` | Class roster |
| `05-clase-editar` | Class form |
| `06-inscribir` | Enroll a student |
| `07-alumnos` | Student list |
| `08-alumno` | Client card with students, classes, fee and payments |
| `09-nuevo-alumno` | Register form with "¿Quién viene a clase?" |
| `10-cuotas`, `10b-cuotas-todos` | Month fees, debtors and everyone |
| `10c-cobros-pedidos` | Cobros on Pedidos: orders summary, filters and order cards |
| `11-registrar-pago` | Record a payment |
| `12-ajustes`, `13-profes` | Settings and instructors |
| `12b-novedades` | Publishing an announcement for students |
| `12c-ajustes-negocio` | Business settings |
| `12d-logros-y-asistencia` | Levels for students and whether noticed absences keep the streak |
| `14-alumno-por-clases` | Student paying per class: classes left and packs |
| `15-vender-pack` | Sell a pack |
| `16-packs` | Class pack catalog |
| `16b-pack-nuevo` | New class pack form, with the classes where the pack can be booked |
| `17-cuota-mensual` | Default fee with "Desde" and the fee changes |
| `18-colores` | Color picker in Ajustes |
| `19-hoy-tema-<name>` | Today in each built-in theme; Agua goes last so the run ends on the default |

When a new screen is added, add a row to `screens` in `e2e/screenshots/App_screens.capture.ts` with the path and a text that proves it finished loading. When a built-in theme is added, add it to `themes` in the same file.

## Student app screens

`e2e/screenshots/Student_screens.capture.ts` captures the app that students and parents use. On top of the demo business it loads (`demoStudentApp.ts`):

- Five shop products: a T-shirt with sizes and one size sold out, a cap and a bottle with unlimited stock, a hoodie made to order, and a sold-out towel.
- An app invitation for the Pérez client, accepted through the API. The invitation link only travels by email, so the test starts a small SMTP sink (`e2e/support/smtpSink.ts`) and the screenshots config points the API at it (`Email:Smtp`, port `E2E_SMTP_PORT`, 2525 by default). If you reuse an API that is already running, start it with that SMTP setting.
- Attendance in Natación inicial for every past class of the month: Lucía never misses, Tomás missed the class two weeks ago, so each shows a different streak.
- A comment from the instructor for each student: Lucía's on her last past class, Tomás's on today's class.
- Two announcements from the school, and next week's Natación inicial class cancelled for a holiday.
- A "No voy" notice for Tomás on his next class after today (skipping the cancelled one).
- A "Pack Aquagym" of 4 classes valid for Aquagym, sold to the Pérez client, with one Aquagym class booked for Lucía this week.
- Three student orders: one paid and ready to hand over in Natación inicial, one class pack waiting for payment, and one cancelled.

The student signs in through the sign-in screen, then the test captures:

| File | Screen |
|---|---|
| `20-alumno-inicio` | Home: next class, streak, monthly fee, instructor comment and shop preview |
| `21-alumno-inicio-otro-alumno` | Home after picking the other student |
| `21b-alumno-novedades` | News: announcements, ready order, instructor comments and the cancelled class, grouped by day |
| `22-alumno-clases` | Week of classes for the selected student |
| `22b-alumno-clases-no-voy` | A class the student won't attend, with "Voy a ir igual" |
| `22c-alumno-clase-de-recuperacion` | A booked makeup class in the student schedule |
| `22d-alumno-recuperar-clases` | Classes to make up, with open slots to book |
| `22e-alumno-clase-del-pack` | Lucía's booked Aquagym class from her pack, with "Reservar clases del pack": classes left and open dates |
| `23-alumno-tienda`, `23b-alumno-tienda-productos` | Shop: search, categories, packs and product grid |
| `24-alumno-producto` | Product sheet with sizes, a sold-out size and quantity |
| `25-alumno-tienda-con-carrito` | Shop with the floating cart bar and the tab badge |
| `26-alumno-carrito` | Cart: lines, delivery place, payment and total |
| `27-alumno-pedidos`, `28-alumno-pedidos-anteriores` | Orders in progress with tracking, and past orders |
| `28b-alumno-logros` | Achievements: level, progress to the next one and level path |
| `28c-alumno-logros-medallas` | Achievements: medals and the last 12 weeks |
| `29-alumno-ajustes` | Settings grouped like the team app: your phone (appearance, notifications) and account (sign out) |
| `29b-alumno-ajustes-notificaciones`, `29c-alumno-ajustes-apariencia` | Notifications and appearance sheets opened from Ajustes |
| `30-elegir-cuenta` | Choosing between the team and the student account when the same email has both |

Run only these with `pnpm screenshots:no-build Student_screens`.

## Invitation flows

`e2e/screenshots/Invitation_flows.capture.ts` captures who can use the app and how each person gets in (see [Ages and minors](../ages-and-minors.md)). It signs up its own business ("Natación Olas") with the Pérez family: Ana, the client who pays and already uses the app, Lucía (14) and Tomás (11), plus Marta Ruiz, who is invited with an email that already has an account. Emails go through the same SMTP sink as the student app screens.

```powershell
cd e2e
pnpm web:build                  # once, or after changing the app
pnpm screenshots:invitations
```

The images land in `e2e/screenshots-output/invitaciones/`:

| File | Screen |
|---|---|
| `01-registro-del-dueno-con-fecha` | Owner sign-up asks for the birth date (must be an adult) |
| `02-ficha-de-la-familia` | Client card: the client uses the app, and each child can be invited on their own |
| `03-invitar-a-lucia-14-anos`, `04-lucia-invitada` | Inviting a child old enough for an account of their own |
| `05-tomas-11-anos-espera-autorizacion` | Inviting a child below the minimum age: waiting for the client, with "Recordar al responsable" |
| `06-responsable-autoriza-desde-el-mail` | The client can authorize from the email link |
| `07-responsable-autoriza-desde-la-app`, `08-responsable-autorizo` | Or from the card on the home of their app |
| `09-tomas-crea-su-cuenta`, `10-tomas-entra-a-la-app` | The child creates the account (name and birth date from the team) and sees the family |
| `11-lucia-crea-su-cuenta-sin-autorizacion` | A 14-year-old creates the account with no authorization needed |
| `12-ya-tiene-cuenta-aceptar-o-rechazar`, `13-rechazo-la-invitacion` | Someone who already has an account only accepts or declines |
| `14-aviso-al-equipo` | The team is notified of the decline |
| `15-editar-perfil` | Editar perfil: name and birth date |
