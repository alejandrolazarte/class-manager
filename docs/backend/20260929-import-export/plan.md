# Backend plan — Import and export (students and coaches)

Status: **validated** (2026-09-29): CSV only, valid rows imported and invalid rows skipped, existing records skipped, no class group column.

Adapted from the design proposal "Motor genérico de Importación / Exportación" (metadata-driven engine with one profile per module). The [pilot plan](../../pilot-df-swimming.md) lists "import current students from a spreadsheet" as needed before the pilot starts; this plan covers that with the smallest version of the engine that is still reusable.

## Goal

An owner moving from a spreadsheet loads their current students and coaches in one go, sees what will happen before anything is saved, and can export the same data back to a spreadsheet.

## Decisions

- **Two modules only: `students` and `instructors`.** Classes, enrollments, fees and packs stay manual for now.
- **The engine is its own library, `src/ImportExport`**, like `Tenancy`: no business concepts, no dependencies (not even EF Core or ASP.NET), so `Core` can reference it without breaking ARCH001. A new rule **ARCH005** keeps it reusable: no `using` of `ClassManager.Core`, `Infrastructure`, `Api`, `Security` or `Tenancy`.
- **Profiles and use cases live in `Core`.** The engine knows columns, types, rows, cells and errors; `Core` knows what a student or a coach is. Each module has a profile (columns + how a row becomes a command) and reuses the existing domain factories (`Client.Create`, `Student.Create`, `PhoneNumber.Create`, `Instructor.Create`), so an import can never create data the app itself would reject.
- **CSV first, XLSX later.** CSV needs no package and Excel, Google Sheets and Numbers all save it. The reader and writer sit behind interfaces (`ITabularReader`, `ITabularWriter`), so XLSX is a later adapter project (`ImportExport.Xlsx`, referenced by `Infrastructure`) with no change to profiles or use cases.
  - Reading: UTF-8 with or without BOM; delimiter detected from the header line (`,`, `;` or tab), because Excel in Spanish saves with `;`; quoted fields per RFC 4180.
  - Writing: UTF-8 with BOM and `;`, so a double click opens it correctly in Excel set to Spanish.
- **Stateless preview.** `preview` parses and validates without saving and keeps nothing on the server. `import` receives the same file again, validates again and saves. No temporary files, no cleanup job.
- **Limits:** 1 MB and 1,000 data rows per file. Enough for a school; above that the request is rejected with a clear message. No background jobs.
- **Column mapping is deterministic.** A header matches a column when, after normalizing (trim, lower case, no accents, no spaces/`_`/`-`), it equals the column header or one of its aliases. Unknown headers are reported and ignored. Missing required columns block the whole file. No manual mapping screen in the first version: the user edits the header in the file, or downloads the template.
- **Invalid rows are skipped, valid rows are imported.** The preview shows each row as `Valid`, `Error` (with column and message) or `Skipped` (already exists). The import saves only valid rows, in one transaction, and returns the same per-row result. A file where every row fails saves nothing.
- **Duplicate policy: skip existing, never update.** Updating people from a file is risky and not needed to leave a spreadsheet. Duplicates inside the file are errors on the later row.
- **The tenant never comes from the file.** Rows become normal commands; `TenantId` is stamped by the existing interceptor from the `tenant_id` claim, and lookups go through the tenant query filters. There is no `TenantId`, `Id` or `CreatedAt` column.
- **Headers are Spanish in the file, keys are English in the API.** Each column has a stable English `key` and a `header` for the file (Spanish, the only language the app ships), plus aliases in both languages. When a second language arrives, headers move to a per-language map.

## Modules

### `students` — one row per student

Adult students on their own are the normal case: a row with only `Alumno` and `Teléfono` creates one client and one student with the same name, exactly what the app does today with the "who attends" switch ([clients and students plan](../20260925-clients-and-students/plan.md)). The `Responsable` column is only for minors or anyone whose classes someone else pays: leave it empty otherwise.

Rows with the same phone number are the same family: siblings go on separate rows with the same phone and the same `Responsable`.

| Key | Header | Type | Required | Aliases | Rule |
|---|---|---|---|---|---|
| `studentName` | Alumno | text | yes | nombre, nombre alumno, student | `Student.Create` |
| `phone` | Teléfono | phone | yes | telefono, móvil, movil, celular, whatsapp | `PhoneNumber.Create` with the business country code; the student's own phone, or the responsible person's |
| `email` | Email | email | no | correo, mail, e-mail | `Client.Create` |
| `birthDate` | Fecha de nacimiento | date | no | nacimiento, fecha nac, birth date | `dd/MM/yyyy` or `yyyy-MM-dd` |
| `studentNotes` | Notas alumno | text | no | notas, observaciones | `Student.Create` |
| `contactName` | Responsable | text | no | tutor, padre, madre, cliente, contact | only for minors; empty = the student is their own contact |
| `contactNotes` | Notas responsable | text | no | | `Client.Create` |

Per row, in order:

1. The phone already belongs to a client of the business → the student joins that family; `Email` and `Notas responsable` are ignored because existing clients are never updated.
2. Otherwise the first **valid** row with that phone creates the client (named after `Responsable`, or after the student when it is empty); later rows with the same phone join it. A row whose own student is invalid doesn't create the family, so its siblings still can.
3. A row that fills `Responsable` with a name different from the family's contact (existing or created earlier in the file) is an error (`import.contact_mismatch`): it usually means a wrong phone would join two unrelated families. An empty `Responsable` always joins, so two adults sharing a phone end up in one family, as the app does when adding a student to a family.
4. The family already has a student with that name in the database → `Skipped` (`student.already_registered`); earlier in the same file → error (`import.duplicate_in_file`).

Domain errors land on the column that caused them: the phone on `phone`, the client name on `contactName` (or `studentName` when `Responsable` is empty), email, birth date and notes on their own columns.

### `instructors` — coaches

| Key | Header | Type | Required | Aliases | Rule |
|---|---|---|---|---|---|
| `fullName` | Profesor | text | yes | nombre, coach, entrenador, monitor, instructor | `Instructor.Create` |

A coach with the same name already exists (case-insensitive, as today) → `Skipped`.

### Export

Same columns as the import, so an export can be edited and imported into another business. `students` exports one row per student with its client, leaving `Responsable` empty when the client has the student's name; `instructors` exports every coach, active or not. No filters in the first version.

- Phones are written as `+<country code> <number>` (for example `+34 611222333`) when they use the business's country code. With the space Excel keeps them as text; without it Excel would turn `+34611222333` into a number, drop the `+` and a later import would add the country code twice.
- Birth dates are written as `dd/MM/yyyy`.
- A family registered without students has no row: the file is one row per student.

## Endpoints

`{module}` is `students` or `instructors`. All require the owner's token.

| Method | Endpoint | Result |
|---|---|---|
| `GET` | `/api/import-export/{module}/schema` | Columns: `key`, `header`, `type`, `required`, `aliases`, `example` |
| `GET` | `/api/import-export/{module}/template` | CSV with the header row and one example row |
| `POST` | `/api/import-export/{module}/preview` | `multipart/form-data` with `file` → per-row result, nothing saved |
| `POST` | `/api/import-export/{module}/import` | Same body → per-row result after saving |
| `GET` | `/api/import-export/{module}/export` | CSV file `students-2026-09-29.csv` |

Unknown module → 404. File too big, too many rows, unreadable file or missing required column → 400 problem details with an error code (`import.file_too_large`, `import.too_many_rows`, `import.unreadable_file`, `import.empty_file`, `import.missing_columns`).

```json
POST /api/import-export/students/preview → 200
{
  "module": "students",
  "columns": [
    { "header": "Alumno", "key": "studentName" },
    { "header": "Tel", "key": "phone" },
    { "header": "Grupo", "key": null }
  ],
  "summary": { "total": 152, "valid": 148, "errors": 3, "skipped": 1 },
  "rows": [
    { "line": 2, "status": "Valid", "errors": [] },
    { "line": 7, "status": "Error", "errors": [ { "key": "phone", "code": "validation", "message": "Phone number must have between 8 and 15 digits including the country code." } ] },
    { "line": 9, "status": "Skipped", "errors": [ { "key": "studentName", "code": "student.already_registered", "message": "The client already has a student with this name." } ] }
  ]
}
```

`import` returns the same shape; its `valid` rows are the ones created. Error codes follow the repo's `area.snake_case` convention (`import.required`, `import.duplicate_in_file`, `instructor.name_taken`...). The upload is `multipart/form-data` with a `file` field; antiforgery is disabled on these endpoints because the API uses bearer tokens, not cookies, and the form body is capped at 1 MB + 64 KB so bigger uploads are rejected before reaching the parser.

## Architecture

```text
src/ImportExport/            ← engine, no dependencies (ARCH005)
  ImportColumn               key, header, type, required, aliases, example
  ColumnType                 Text, Date, Email, Phone
  ITabularReader / Writer    CsvTabularReader, CsvTabularWriter
  HeaderMatcher              normalization + aliases → ColumnMapping
  ImportParser               file → mapping + ImportRow (GetText, GetDate) + cell errors (limits enforced here)
  ImportLimits, ImportFileError, ImportErrorCodes

src/Core/UseCases/ImportExport/   ← business side
  IImportModule              per module: Name, Columns, PlanAsync(rows) → Result<ImportPlan>
  ImportPlan                 per-row results + AddValidRows (adds the new entities to the repositories, no save)
  ImportFlow                 module lookup, parse, rows with cell errors never reach the module, report
  GetImportSchemaUseCase     module columns
  PreviewImportUseCase       ImportFlow, nothing added or saved
  ImportFileUseCase          ImportFlow + AddValidRows + one SaveChanges (atomic); unique violation → 409
  ImportRowResult, ImportSummary, ImportReport, ImportUseCaseErrorCodes
  Instructors/InstructorImportModule
  Students/StudentImportModule
  GetImportTemplateUseCase, ExportUseCase, ExportFile

src/Infrastructure/          ← repository queries the modules need (clients by phone in bulk, students by client ids)
src/Api/Endpoints/ImportExportEndpoints.cs   ← multipart upload, file results, module lookup
```

Flow of `import`:

```text
IFormFile ─► Api ─► ImportFileUseCase(module, stream)
                      ├─ ImportParser (engine): read CSV, match headers, convert types, cell errors
                      ├─ IImportModule.PlanAsync (Core): domain factories, duplicates in file and in DB
                      └─ ImportPlan.AddValidRows + IUnitOfWork.SaveChangesAsync (one atomic save)
                                                       └─ tenant stamped by TenantStampingSaveChangesInterceptor
```

Preview and import share the same validation code; the only difference is the last step.

## Frontend (sketch, separate plan)

In Ajustes → "Importar datos": pick Alumnos or Profesores → "Descargar plantilla" / "Elegir archivo (CSV)" → preview screen with counts and the rows with errors (line, column, message) → "Importar N filas" → result. In the students and coaches lists, an "Exportar" action that downloads or shares the CSV. Uses `expo-document-picker` and, on mobile, `expo-sharing`.

## Out of scope (later phases)

XLSX, manual column mapping, updating existing records, importing classes/enrollments/fees, background jobs for big files, import history and undo, AI mapping, OCR from a photo (it would produce the same `StudentImportRow` and reuse the same validation).

## Tests

- `ClassManager.ImportExport.U.Tests` (new): CSV reader (`;`, `,`, quotes, BOM, empty lines), header matching (accents, aliases, missing required, unknown), type conversion (both date formats), limits.
- `ClassManager.Core.U.Tests`: student module (new client, existing client by phone, siblings, conflicting contact name, duplicate in file, duplicate in DB, invalid phone), instructor module, export rows.
- `ClassManager.Api.I.Tests`: preview saves nothing; import saves only valid rows; business A imports and business B does not see the data; export of business A contains no rows of business B; 400 for a file over the limits.
- Analyzer tests for ARCH005.

## Steps

1. ~~`src/ImportExport` + ARCH005 + CSV reader/writer + header matcher + parser, with their unit tests.~~ Done: see [import-export.md](../../import-export.md). Windows-1252 fallback and formula escaping on export were added on the way.
2. ~~`Core`: instructors module and use cases (the simplest module, proves the whole path), endpoints and integration tests.~~ Done: schema, preview and import for `instructors`. No repository additions were needed (`ListAllAsync` covers duplicates).
3. ~~`Core`: students module, bulk repository queries, integration tests including tenant isolation.~~ Done: `IClientRepository.ListByPhoneNumbersAsync` and `IStudentRepository.ListByClientIdsAsync` load the families in two queries; `IImportModule.PlanAsync` returns `Result<ImportPlan>` so a module can fail as a whole (no current business).
4. ~~Export and template endpoints.~~ Done: `IImportModule.ExportRowsAsync` gives one value per column key; `GetImportTemplateUseCase` writes the header and each column's `Example`; `ExportUseCase` names the file `{module}-{yyyy-MM-dd}.csv`. An integration test exports a business's students and imports them into another one with nothing lost.
5. Docs: `docs/import-export.md` (library, like `tenancy.md`), `analyzers.md`, `domain-model.md` unchanged; link from `README.md`.
6. Frontend plan and screens (separate PR).

## Decisions taken on validation

1. CSV only for the pilot; XLSX stays in the later phases.
2. Invalid rows are skipped and the valid ones imported.
3. Existing records are skipped, never updated.
4. No class group column: enrollments stay manual.
