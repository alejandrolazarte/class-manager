# Frontend plan — Import and export

Backend: [plan](../../backend/20260929-import-export/plan.md).

## Screens

Since 2026-10-27 import and export live next to each list instead of in Settings:

| Part | Behavior |
|---|---|
| ⋮ in Alumnos and Profes | `OverflowMenu` (from `src/ui`) at the top right with **Importar alumnos/profes** and **Exportar alumnos/profes (Excel)**. Shown only with `importExport.run` and the whole list: `students.view.all` for Alumnos, `instructors.view` for Profes. Without the plan, both options open the plan offer |
| Exportar | `GET /export`, saved as `alumnos-2026-09-29.xlsx` / `profes-2026-09-29.xlsx`; a toast if the download fails |
| Importar alumnos / profes | `/students/import` and `/settings/instructors/import`, three numbered steps: download the template, fill it in (every column from `GET /schema`, required ones marked, with its example), upload the file |
| Descargar plantilla | `GET /template`, saved as `plantilla-alumnos.xlsx` / `plantilla-profes.xlsx` |
| Elegir archivo (Excel o CSV) | Document picker, then `POST /preview`: counts (to import, with errors, already loaded), ignored columns and every row that won't be imported, with its line, the person's name, and the reason in Spanish |
| Importar N filas | `POST /import` with the same file; shows the result with **Ver alumnos/profes** (back to the list) and **Importar otro archivo** |

## Decisions

- **Files:** `expo-document-picker` to choose the XLSX or CSV. To save: on web a normal browser download; on Android and iOS the file is written to the cache with `expo-file-system` and handed to the share sheet (`expo-sharing`), so the owner can save it to Drive, send it by WhatsApp or open it in Excel. Versions come from the SDK's `bundledNativeModules.json`.
- **File names are built by the app** in Spanish; the API's `Content-Disposition` name is not read, so CORS doesn't need to expose that header.
- **Downloads are XLSX**, read as bytes (`httpClient.getBytes`) and written unchanged. They were CSV with a BOM until 2026-09-27, but the Google Sheets app on Android ignored the BOM and showed broken accents; see [import-export.md](../../import-export.md#why-exports-are-xlsx).
- **Messages are translated from error codes**, never shown from the API (its messages are English). Row errors: `import.required`, `import.invalid_date`, `import.duplicate_in_file`, `import.contact_mismatch`, `student.already_registered`, `instructor.name_taken`; any other code (domain `validation`) shows "<column> no es válido". File errors: `import.missing_columns`, `import.file_too_large`, `import.too_many_rows`, `import.unreadable_file`, `import.empty_file`.
- `httpClient` gains `postForm` (multipart, no JSON content type) and `getText`.

## Tests (write first)

- Previewing a students file shows the rows with errors and the reasons.
- Confirming imports the same file and shows the result.
- A file without a required column shows the file error.
- Cancelling the picker previews nothing.
- Downloading the template and exporting save the XLSX with the Spanish file name.
- The ⋮ is hidden without the whole list; choosing import opens its screen; a failed export shows a toast.
- A problem row shows its name; the instructors file lists the Email column.
- `postForm` doesn't send `Content-Type: application/json`.
