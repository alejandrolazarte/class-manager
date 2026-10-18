# Student measurements

Date: 2026-10-12

Status: steps 1–3 built. Design: the "Avance del alumno" rows of the [Calma design artifact](https://claude.ai/artifact/MGjmVNd9Qav63zCt7WfueM).

## Goal

The team records how a student progresses: times, weights, distances, repetitions or any number. The app works the same for swimming, yoga, pilates or functional training because it knows nothing about sports. Each business defines **what it measures**, and the app only needs to know the kind of value and whether a higher or a lower value is better. With that it shows the latest value, the change since the first record, a chart and the personal best.

## Decisions

### Three pieces

| Piece | What it is | Who creates it |
|---|---|---|
| `MeasurementDefinition` | What the business measures: name, kind, unit, when it improves, whether students see it and whether they can record it | The business (Ajustes → Mediciones) |
| `MeasurementRecord` | One value of one student on one date, with an optional note | The team, or the student when the definition allows it |
| `MeasurementGoal` | An optional target value of one student for one definition | The team |

### Kind and unit

The kind decides how a value is entered and shown; the value is always stored as one `decimal` in the base unit of the kind, so charts and comparisons never need to know the sport.

| Kind | Units | Stored as |
|---|---|---|
| `Time` | `s` | seconds (entered as minutes and seconds) |
| `Weight` | `kg`, `lb` | the chosen unit |
| `Distance` | `m`, `km` | the chosen unit |
| `Length` | `cm` | centimeters |
| `Repetitions` | `rep` | a whole number |
| `Number` | free text, up to 12 characters (`pts`, `nivel`) | the number |

Kind and unit can't change once the definition exists: old records would mean something else. The name, the direction and the two student switches can change.

### When it improves

`ImprovesWhen` is `Increases`, `Decreases` or `Neither`. It decides the color of the change in the team app (green or red), the personal best and the student app copy. `Neither` (height, body weight) only records values.

### What the student sees and records

- `IsVisibleToStudents`: the definition and its records show in the student app, in Logros → Mediciones. New `Weight` and `Length` definitions start hidden: body measurements must not reach a family by accident.
- `IsRecordedByStudents`: the student (or the guardian, for each student of the account) can add records from the student app. It requires `IsVisibleToStudents`; turning visibility off turns it off.
- The student app only celebrates improvements: no red, and no comparison sentence when a value got worse.
- Every record keeps who recorded it (`RecordedByUserId` for the team, `RecordedByStudent` for the student app), and the team sees it in the student's profile.

### One record per date

A student has at most one record per definition and date. Recording again on the same date replaces the value, so a typo doesn't stay as a spike in the chart.

### Deleting

Definitions and records are data with business meaning, so both implement `ISoftDeletable` and are removed with Deshacer ([deleting data](../../deleting-data.md)): `DELETE` then `POST …/restore`. Deleting a definition keeps its records; restoring it brings them back. Goals are replaced or cleared by saving them again.

### Permissions

No new permissions:

- Manage definitions: `business.manage`, like the other business settings. List them: `students.view.own`.
- See a student's measurements: `students.view.own`, limited to the students the member can reach (`AccessRules.CanReachClientsAsync`).
- Record, delete, restore and set goals: `attendance.record.own` plus the same reach check. Whoever takes attendance in a class can also measure in it.

### Tenancy

The three tables carry `TenantId` and are covered by the central tenant filter. Tests prove a business can't read another business's definitions or records.

## API

Team (`/api`):

| Method | Route | Use |
|---|---|---|
| `GET` | `/measurements` | Definitions of the business |
| `POST` | `/measurements` | Create a definition |
| `PUT` | `/measurements/{id}` | Change name, direction and student switches |
| `DELETE` / `POST` | `/measurements/{id}`, `/measurements/{id}/restore` | Delete with undo |
| `GET` | `/students/{studentId}/measurements` | Every definition with the student's records and goal |
| `POST` | `/students/{studentId}/measurements/{id}/records` | Record a value |
| `GET` | `/measurement-records?measuredOn=&studentIds=` | Records of several students on one date (the measurements of a class) |
| `POST` | `/measurement-records` | Record or change one definition for several students at once (the measurements of a class) |
| `DELETE` / `POST` | `/measurement-records/{id}`, `/measurement-records/{id}/restore` | Delete with undo |
| `PUT` | `/students/{studentId}/measurements/{id}/goal` | Set or clear (`null`) the goal |

Student app (`/api/student-app`):

| Method | Route | Use |
|---|---|---|
| `GET` | `/measurements` | Visible definitions with records and goals, for every student of the account |
| `POST` | `/students/{studentId}/measurements/{id}/records` | Record a value when the definition allows it |

## Steps

1. Backend: definitions, records, goals, team and student app endpoints, tests.
2. Team app: Ajustes → Mediciones, the Avance tab of the student profile, the measurement detail, recording, and the measurements of a class.
3. Student app: Logros gets two tabs, Constancia (everything it shows today) and Mediciones, plus the detail and the form to record.

## Later

- **Cómo se mide.** An optional text, photo, short video or link per definition, written by the instructor, shown to the student when recording ("Antebrazos en el piso, el tiempo corre hasta que la cadera baja"). It replaces the idea of a library of exercise GIFs, which only fits gyms. Retake once measurements are in use; files go through `src/Storage` like class material.
- Plan feature: decide whether measurements belong to a paid plan (`RequireFeature`).
