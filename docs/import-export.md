# Import and export library

Date: 2026-09-29

`src/ImportExport` is the engine that reads and writes spreadsheet files for any module. It knows columns, types, rows, cells and errors, never students or coaches, so it can be reused like the [tenancy](tenancy.md) and [security](security.md) libraries. Plan and module rules: [import and export plan](backend/20260929-import-export/plan.md).

## Contents

| Folder | Types | Purpose |
|---|---|---|
| `Columns` | `ImportColumn`, `ColumnType` | What a module accepts: stable `Key`, file `Header`, `Type`, `IsRequired`, `Aliases`, `Example` |
| `Tabular` | `ITabularReader`, `ITabularWriter`, `TabularData`, `FormulaEscaping` | File format boundary: bytes to rows and rows to bytes |
| `Tabular/Csv` | `CsvTabularReader`, `CsvTabularWriter` | The only format for now |
| `Parsing` | `IImportParser`, `ImportParser`, `HeaderMatcher`, `ImportRow`, `ImportCellError` | Limits, header matching, required cells and date conversion |
| root | `ImportLimits`, `ImportFileError`, `ImportErrorCodes` | File-level limits and error codes |

The library has no dependencies. `Core` references it and defines the modules and use cases in `src/Core/UseCases/ImportExport`; `Api` registers `ImportParser`, `CsvTabularReader`, `CsvTabularWriter` and every `IImportModule`, and exposes them under `/api/import-export/{module}`.

## Adding a module

1. In `Core`, implement `IImportModule`: a lowercase `Name` (the route segment), its `Columns`, and `PlanAsync`, which turns rows without cell errors into domain entities with the existing factories and returns one `ImportRowResult` per row (`Valid`, `Error` or `Skipped`) plus `AddValidRows`, which only adds entities to repositories. Saving is the use case's job.
2. Register it in `AddUseCases` as `services.AddScoped<IImportModule, ...>()`.
3. Tests: unit tests for the module rules, and an integration test proving another business doesn't see the imported rows.

## CSV rules

| Concern | Reading | Writing |
|---|---|---|
| Encoding | UTF-8 with or without BOM; falls back to Windows-1252 when the bytes are not valid UTF-8 (Excel's plain "CSV" in Spanish) | UTF-8 with BOM |
| Delimiter | Detected from the first non-blank line: `;`, `,` or tab | `;`, so Excel set to Spanish opens it with a double click |
| Quotes | RFC 4180: quoted cells keep delimiters, `""` and line breaks | Quoted only when the cell has `;`, `"` or a line break |
| Lines | `\r\n`, `\n` or `\r`; blank rows are skipped; each row keeps the line number where it starts, for error messages | `\r\n` |
| Formulas | A leading `'` before `=`, `+`, `-` or `@` is removed | Cells starting with `=`, `+`, `-`, `@`, tab or carriage return get a leading `'`, except signed numbers such as `+34 611 22 23 33`, so an export can't run formulas in Excel |

## Parsing

`ImportParser.ParseAsync(file, columns, limits)` returns either a file-level `ImportFileError` or a `ParsedImport` with the column mapping and one `ImportRow` per data row.

- File-level errors stop everything: `import.file_too_large`, `import.too_many_rows`, `import.unreadable_file` (a quoted cell never closes), `import.empty_file` and `import.missing_columns` (the message names the missing headers). `ImportLimits.Default` is 1 MB and 1,000 rows.
- A header matches a column when, after trimming, lower-casing and removing accents, spaces, `_`, `-` and `.`, it equals the column key, header or one of its aliases. Unknown headers are kept in the mapping with a `null` key so the preview can show them as ignored. When two headers match the same column, the first wins.
- Cell errors belong to the row: `import.required` for an empty required cell, `import.invalid_date` for a date that is not `d/M/yyyy`, `d-M-yyyy` or `yyyy-M-d`. Other rules (phone, email, lengths, duplicates) belong to the module in `Core`, which reuses the domain factories.
- `ImportRow.GetText(key)` returns the trimmed value or `null`; `GetDate(key)` the parsed date or `null`.

## Rules

- ARCH005: `ImportExport` and `ImportExport.*` cannot use `ClassManager.Core`, `Infrastructure`, `Api`, `Security` or `Tenancy` (see [analyzers.md](analyzers.md)). The tenant never comes from a file: modules turn rows into normal commands and the tenancy interceptor stamps them.
- A new format (for example XLSX) is a new `ITabularReader` / `ITabularWriter` in its own project; profiles and use cases don't change.
