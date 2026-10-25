# Load testing

Date: 2026-10-25

How the API behaves with many users at once, measured with [k6](https://grafana.com/docs/k6/) against the real API and SQL Server: which endpoints were slow, why, what was changed, and what to do next. The scripts live in `loadtests/`.

## What is measured

`loadtests/scenarios/staff-day.js` simulates the team of many businesses working at the same time. Each virtual user signs in as the owner of one business, opens the app (`/api/me`, `/api/business`, `/api/me/accounts`, `/api/organization/subscription`, `/api/team/notifications`) and then repeats one flow with 1–5 seconds of thinking time between screens:

| Flow | Share of users | Requests |
|---|---:|---|
| Take attendance | 35% | Today's sessions, one session, mark the whole class, change one student |
| Browse students | 25% | Student list (`limit=20`), a search, a family, its payments, a student's enrollments |
| Fees | 15% | Month fees, record a payment, fees again, undo the payment |
| Plan classes | 15% | Class list, one class, its roster, the month calendar |
| New client | 5% | Register a family with one student and enroll it |
| Large business | 5% | Any of the flows above on a business with 1,000 families |
| Sign-ins | 1 per second | `POST /api/auth/sign-in` (configurable) |

Requests are tagged with the route template (`GET /api/clients/{id}`), so `loadtests/report.mjs` prints median, p95, p99 and max per endpoint; the large business's requests are reported apart with `[large]`.

## Data

`loadtests/seed.mjs` creates the businesses through the API, the same way a user would: it signs up each owner (reading the confirmation link from an SMTP sink, the one in `e2e/support/smtpSink.ts`), sets a monthly fee, creates 12 weekly classes (more, with extra instructors, for large businesses), registers families with one or two students, enrolls each student in one or two classes ten weeks ago, records this month's payment for 60% of the families and the attendance of every class for the last four weeks.

The measured database had 21 businesses: 20 with 150 families each and one with 1,000. About 4,250 families, 4,250 students, 6,000 enrollments, 37,000 attendance marks and 1,900 payments.

## Running it

Use a dedicated database, never the development or e2e one. In the cloud environment SQL Server and Azurite are started by `bash scripts/cloud-session-start.sh --wait`; locally use the Podman containers from [running-e2e-tests.md](e2e/running-e2e-tests.md).

1. Create the database and apply the migrations:

   ```bash
   MIGRATIONS_CONNECTION_STRING="Server=tcp:localhost,1433;Database=ClassManagerLoad;User Id=sa;Password=<password>;TrustServerCertificate=True" node scripts/migrate-database.mjs
   ```

2. Build the API in Release and run it from its output folder (so it finds `appsettings.json`) in the `Production` environment, with the auth rate limit raised (every virtual user signs in from the same address) and email going to the seed script's SMTP sink:

   ```bash
   dotnet build src/Api/Api.csproj -c Release
   cd src/Api/bin/Release/net10.0
   ASPNETCORE_ENVIRONMENT=Production ASPNETCORE_URLS=http://localhost:5000 \
   ConnectionStrings__BusinessDatabase="<the connection string above>" \
   ConnectionStrings__AuditLog=UseDevelopmentStorage=true \
   Subscriptions__SignUpPlan=enterprise \
   Authentication__Jwt__SigningKey=<a random key of 64 characters> \
   Authentication__RateLimit__PermitLimit=1000000 \
   Email__Smtp__Host=localhost Email__Smtp__Port=2525 Email__Smtp__FromAddress=load@test.local \
   Logging__LogLevel__Default=Warning \
   dotnet Api.dll
   ```

3. Seed the businesses (about 70 seconds for the default 20 × 150; `LOAD_APPEND=true` adds to the existing file):

   ```bash
   cd loadtests
   node seed.mjs
   LOAD_APPEND=true LOAD_BUSINESSES=1 LOAD_CLIENTS_PER_BUSINESS=1000 node seed.mjs
   ```

   Variables: `LOAD_API_URL` (default `http://localhost:5000`), `LOAD_SMTP_PORT` (2525), `LOAD_BUSINESSES` (20), `LOAD_CLIENTS_PER_BUSINESS` (150), `LOAD_ATTENDANCE_WEEKS` (4), `LOAD_SEED_CONCURRENCY` (4). The credentials and ids go to `loadtests/.data/businesses.json` (ignored by git).

4. Run k6 (through Docker, `grafana/k6:1.3.0`, so nothing has to be installed) and print the per-endpoint table:

   ```bash
   LOAD_PEAK_USERS=300 LOAD_RAMP_SECONDS=60 LOAD_HOLD_SECONDS=120 ./run-k6.sh scenarios/staff-day.js run-300
   ```

   `LOAD_PEAK_USERS` is the number of simultaneous users at the peak, `LOAD_SIGN_INS_PER_SECOND` the sign-in rate (default 1). The raw results go to `loadtests/.results/<name>.csv` (ignored by git); `node report.mjs .results/<name>.csv` prints the table again.

To see what the database spends its time on, clear the plan cache before a run (`DBCC FREEPROCCACHE`) and read `sys.dm_exec_query_stats` after it, ordered by `total_worker_time`; `sys.dm_db_missing_index_details` lists the indexes SQL Server would have used.

## Environment of the measurement

Everything ran on one cloud container with 4 cores and 15 GB: the API (Release, `Production`), SQL Server 2022 in Docker, Azurite and k6. They compete for the same CPU, so absolute numbers are pessimistic compared with separate machines, and the comparison between runs is what matters. Production is much smaller (0.25 vCPU for the API and a Basic 5 DTU database, see [hosting plan](hosting-plan.md)); see [What this means for production](#what-this-means-for-production).

k6 itself must stay light: the first version filtered the list of businesses on every iteration, and `SharedArray` deserializes an element on every access, so k6 took 1.5–2.3 cores and slowed the API down. Each virtual user now deserializes only its own business, once.

## Findings

### Before the changes

| Users | Requests/s | Median | p95 | p99 | Failed |
|---:|---:|---:|---:|---:|---:|
| 100 | 50 | 15 ms | 102 ms | 171 ms | 0% |
| 300 | 128 | 19 ms | 83 ms | 171 ms | 0% |
| 600 | 239 | 94 ms | 572 ms | 1.1 s | 0% |

Up to 300 users every endpoint answered in under 100 ms at p95. At 600 the container ran out of CPU (SQL Server about 1.5 cores, the API about 1.4) and every endpoint slowed down together; the slowest were:

| Endpoint | Median | p95 | Cause |
|---|---:|---:|---|
| `PUT /api/class-groups/{id}/sessions/{date}/attendance` (1,000 families) | 838 ms | 3.2 s | One query per student to find the existing mark (N+1) |
| `PUT /api/class-groups/{id}/sessions/{date}/attendance` | 345 ms | 1.3 s | Same |
| `GET /api/fees?month` (1,000 families) | 208 ms | 571 ms | Students and clients read by scanning the whole table (every business), then the roster query |
| `GET /api/class-groups/{id}/sessions/{date}` | 145 ms | 750 ms | Enrollments counted without an index on the date |
| `GET /api/sessions/calendar?month` | 111 ms | 482 ms | Attendance counts read every row through the clustered index to get `Status` |

What SQL Server spent its time on, from `sys.dm_exec_query_stats` after the 600-user run:

- **Marking the whole class ran one `SELECT TOP(1) ... FROM Attendances` per student**: 89,000 executions in 3 minutes, about 26 per request. `RecordClassAttendanceUseCase` called `AttendanceMarks.ApplyAsync` for each student, and each call looked up that student's mark.
- **Enrollments active on a date** (`StartDate <= @date AND (EndDate IS NULL OR EndDate >= @date)`, used by today's sessions, the calendar and the fees) had no index on `StartDate`: every call read all the business's enrollments through another index plus a lookup per row. SQL Server's missing-index list ranked it first.
- **Attendance counts per session** (calendar, today's sessions) used the unique `(TenantId, ClassSessionId, StudentId)` index but had to look up every row to read `Status`: about 5,200 logical reads per calendar.
- **Payments of a month** (fees) used `(TenantId, Month, ClientId)` but looked up `Amount`, `CurrencyCode` and `IsDeleted` in the table.
- **Students and clients of a business** (fees, student list and search) were read by scanning the whole `Students` and `Clients` tables, every business's rows, because no index led with `TenantId` and covered `UserId`. In a shared database this cost grows with the size of the whole database, not of the business.
- **About 10 queries per request.** Two of them run on every authenticated request to resolve the member's role (`CurrentMember`: `BusinessMembers` and the brand owner check), and one reads the business settings; together about a third of all queries, each cheap (under 0.1 ms of CPU) but a round trip.

Sign-ins are expensive by design: ASP.NET Core Identity hashes the password with PBKDF2 (100,000 iterations), about 100 ms of CPU each. With 15 sign-ins per second on top of 300 users, the API spent most of its CPU on hashing and p95 rose from 83 ms to 2.75 s for every endpoint. Normal use signs in rarely (the refresh token lasts 30 days) and the auth rate limit allows 10 sign-ins per minute per address, so this only matters for a burst of sign-ins from many addresses.

### Changes

1. **Attendance of the whole class reads the session's marks once** (`IAttendanceRepository.ListBySessionForUpdateAsync`) and applies each student's mark from that list, instead of one query per student. Covered by unit tests (`When_RecordClassAttendance_changes_marks_of_a_recorded_session`) and an integration test (`When_marking_the_whole_class_again`).
2. **Covering indexes** (migration `CoveringIndexesForDailyQueries`), all filtered by `TenantId` first:
   - `Enrollments (TenantId, StartDate) INCLUDE (EndDate, ClassGroupId, StudentId)`: new.
   - `Attendances (TenantId, ClassSessionId, StudentId)`: now `INCLUDE (Status)`.
   - `Payments (TenantId, Month, ClientId)`: now `INCLUDE (Amount, CurrencyCode, IsDeleted)`.
   - `Students (TenantId, ClientId)`: now `INCLUDE (UserId, Notes)`. Reads of `Students` in the fees roster: 87 → 9 pages.
   - `Clients (TenantId, RegisteredByUserId)`: now `INCLUDE (UserId)`. Reads of `Clients`: 79 → 7 pages.

   An index on `Users (TenantId, IsDeleted) INCLUDE (FullName, PhoneNumber, BirthDate)`, also suggested by SQL Server, was tried and left out: the plans kept seeking users by id and the student search read more pages with it.

### After the changes

Same data, same scenario, same container, measured right after the baseline:

| Users | Requests/s | Median | p95 | p99 | SQL CPU per request | Logical reads per request | Queries per request |
|---:|---:|---:|---:|---:|---:|---:|---:|
| 300, before | 128 | 19 ms | 83 ms | 171 ms | 3.4 ms | 879 | 9.9 |
| 300, after | 128 | 15 ms | 52 ms | 139 ms | 1.7 ms | 386 | 7.3 |
| 600, before | 239 | 94 ms | 572 ms | 1.1 s | 3.7 ms | 1,007 | 9.9 |
| 600, after | 250 | 36 ms | 258 ms | 599 ms | 2.1 ms | 406 | 8.2 |

The database does about half the work per request. With 600 users, by endpoint:

| Endpoint | Median before | Median after | p95 before | p95 after |
|---|---:|---:|---:|---:|
| `PUT .../sessions/{date}/attendance` (1,000 families) | 838 ms | 58 ms | 3,171 ms | 597 ms |
| `PUT .../sessions/{date}/attendance` | 345 ms | 60 ms | 1,321 ms | 347 ms |
| `GET /api/class-groups/{id}/sessions/{date}` | 145 ms | 55 ms | 750 ms | 378 ms |
| `GET /api/fees?month` (1,000 families) | 208 ms | 113 ms | 571 ms | 373 ms |
| `GET /api/fees?month` | 118 ms | 42 ms | 459 ms | 222 ms |
| `GET /api/sessions/calendar?month` | 111 ms | 40 ms | 482 ms | 230 ms |
| `GET /api/sessions?date` | 97 ms | 34 ms | 464 ms | 227 ms |
| `GET /api/clients/{id}` | 112 ms | 43 ms | 586 ms | 297 ms |
| `GET /api/students?search` (1,000 families) | 92 ms | 36 ms | 428 ms | 117 ms |
| `POST /api/clients` | 179 ms | 67 ms | 682 ms | 443 ms |

Every other endpoint improved too, because the whole container had more CPU left.

## What this means for production

Rough estimate, to be checked against Azure metrics: Microsoft equates 100 DTU to about 1 vCore, so the Basic database (5 DTU) has about 50 ms of CPU per second. At 3.4–3.7 ms of SQL CPU per request it saturated at about 14 requests per second (about 30 users working at once with this scenario's pace); at 1.7–2.1 ms it reaches about 25 (about 55 users). The API at 0.25 vCPU handles roughly 45 requests per second with this mix, but only about 2–3 sign-ins per second. So with one Basic database the database is the first limit, then sign-in bursts on the API.

## Next steps

In order of expected gain:

1. **Measure in Azure** with the pilot's real database: Query Performance Insight and `sys.dm_db_resource_stats` show DTU use per query; run k6 against a copy of the environment, never against production.
2. **Move the database to Standard S0/S1 when a few businesses use it daily** (10–20 DTU, about USD 15–30 a month): two to four times the database power of Basic. See [Database on the Basic tier](database-basic-tier.md).
3. **One query for the member's access**: `CurrentMember` runs two queries per request (branch member and brand owner); one query with both answers saves a round trip on every request. Caching it across requests would save more but delays a role change or a removal, so it needs a decision on how long that may take.
4. **Cache business settings per request or for a short time** (time zone, currency): read on most requests and rarely changed.
5. **Fees and student search read users one by one**: `GET /api/fees` joins enrollments, students, clients and users and SQL Server seeks `Users` by id about 2,000 times for the large business (3,900 logical reads). Selecting the client and student names with fewer joins, or projecting them from a single `Users` read, would cut it; worth it when businesses pass a few hundred families.
6. **Sign-in bursts**: if many people sign in at the same time (a new business importing students and inviting them), the API needs more than 0.25 vCPU or a second replica; the password hash cost should not be lowered.
