# Demo screenshots

`e2e/screenshots/` loads a demo business through the API and captures every main screen of the web build at phone size (Pixel 7), both the team app and the family app. Use it to show the app, review a UI change, or check copy.

It reuses the e2e setup: the same web build, API start, database and Playwright install. It is **not** part of the e2e suite: it lives outside `e2e/tests/` and has its own config (`playwright.screenshots.config.ts`), so CI never runs it.

## What it loads

"Escuela de Natación Brazada" with owner Laura Gómez (also the first instructor) and:

- A second instructor, Martín Díaz.
- Four class groups: Natación inicial and Natación avanzada (Monday, Wednesday, Friday and whatever day the script runs, so "Hoy" always has classes), Aquagym and Natación adultos (Tuesday and Thursday).
- Six families, nine enrollments from the first day of the current month, and attendance for today.
- A default fee of $25.000, one family with its own fee, and payments that leave families paid, partially paid and owing.
- A raise to $28.000 from next month, three class packs (single class, 4 classes for a month, 8 classes for two months), and the Suárez family paying per class with a discounted 4-class pack and one class already used.

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
| `02b-comentario-del-profe` | Comment sheet a coach fills in for a student after class |
| `03-clases-semana` | Weekly classes |
| `04-clase-alumnos` | Class roster |
| `05-clase-editar` | Class form |
| `06-inscribir` | Enroll a student |
| `07-alumnos` | Student list |
| `08-familia` | Family card with students, classes, fee and payments |
| `09-nuevo-alumno` | Register form with "¿Quién viene a clase?" |
| `10-cuotas`, `10b-cuotas-todos` | Month fees, debtors and everyone |
| `10c-cobros-pedidos` | Cobros on Pedidos: orders summary, filters and order cards |
| `11-registrar-pago` | Record a payment |
| `12-ajustes`, `13-profes` | Settings and instructors |
| `12b-novedades` | Publishing an announcement for families |
| `12c-ajustes-negocio` | Business settings |
| `12d-logros-y-asistencia` | Levels for students and whether noticed absences keep the streak |
| `14-familia-por-clases` | Family paying per class: classes left and packs |
| `15-vender-pack` | Sell a pack |
| `16-packs` | Class pack catalog |
| `16b-pack-nuevo` | New class pack form, with the classes where the pack can be booked |
| `17-cuota-mensual` | Default fee with "Desde" and the fee changes |
| `18-colores` | Color picker in Ajustes |
| `19-hoy-tema-<name>` | Today in each built-in theme; Agua goes last so the run ends on the default |

When a new screen is added, add a row to `screens` in `e2e/screenshots/App_screens.capture.ts` with the path and a text that proves it finished loading. When a built-in theme is added, add it to `themes` in the same file.

## Family app screens

`e2e/screenshots/Family_screens.capture.ts` captures the app that families and students use. On top of the demo business it loads (`demoFamily.ts`):

- Five shop products: a T-shirt with sizes and one size sold out, a cap and a bottle with unlimited stock, a hoodie made to order, and a sold-out towel.
- An app invitation for the Pérez family, accepted through the API. The invitation link only travels by email, so the test starts a small SMTP sink (`e2e/support/smtpSink.ts`) and the screenshots config points the API at it (`Email:Smtp`, port `E2E_SMTP_PORT`, 2525 by default). If you reuse an API that is already running, start it with that SMTP setting.
- Attendance in Natación inicial for every past class of the month: Lucía never misses, Tomás missed the class two weeks ago, so each shows a different streak.
- A comment from the coach for each student: Lucía's on her last past class, Tomás's on today's class.
- Two announcements from the school, and next week's Natación inicial class cancelled for a holiday.
- A "No voy" notice for Tomás on his next class after today (skipping the cancelled one).
- A "Pack Aquagym" of 4 classes valid for Aquagym, sold to the Pérez family, with one Aquagym class booked for Lucía this week.
- Three family orders: one paid and ready to hand over in Natación inicial, one class pack waiting for payment, and one cancelled.

The family signs in through the sign-in screen, then the test captures:

| File | Screen |
|---|---|
| `20-familia-inicio` | Home: next class, streak, monthly fee, coach comment and shop preview |
| `21-familia-inicio-otro-alumno` | Home after picking the other student |
| `21b-familia-novedades` | News: announcements, ready order, coach comments and the cancelled class, grouped by day |
| `22-familia-clases` | Week of classes for the selected student |
| `22b-familia-clases-no-voy` | A class the family said the student won't attend, with "Voy a ir igual" |
| `22c-familia-clase-de-recuperacion` | A booked makeup class in the family schedule |
| `22d-familia-recuperar-clases` | Classes to make up, with open slots to book |
| `22e-familia-clase-del-pack` | Lucía's booked Aquagym class from her pack, with "Reservar clases del pack": classes left and open dates |
| `23-familia-tienda`, `23b-familia-tienda-productos` | Shop: search, categories, packs and product grid |
| `24-familia-producto` | Product sheet with sizes, a sold-out size and quantity |
| `25-familia-tienda-con-carrito` | Shop with the floating cart bar and the tab badge |
| `26-familia-carrito` | Cart: lines, delivery place, payment and total |
| `27-familia-pedidos`, `28-familia-pedidos-anteriores` | Orders in progress with tracking, and past orders |
| `28b-familia-logros` | Achievements: level, progress to the next one and level path |
| `28c-familia-logros-medallas` | Achievements: medals and the last 12 weeks |
| `29-familia-ajustes` | Settings grouped like the team app: your phone (appearance, notifications) and account (sign out) |
| `29b-familia-ajustes-notificaciones`, `29c-familia-ajustes-apariencia` | Notifications and appearance sheets opened from Ajustes |
| `30-elegir-cuenta` | Choosing between the team and the family account when the same email has both |

Run only these with `pnpm screenshots:no-build Family_screens`.
