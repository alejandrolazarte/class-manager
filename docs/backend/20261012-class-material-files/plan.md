# Several PDFs per class

Date: 2026-10-12. Status: proposal, not built.

A class shares a link and one PDF with its students (see [Class material files](../../storage.md#class-material-files)). Some instructors may want to upload more than one PDF to a class (a guide, a routine, a calendar). This plan describes how, and how it is paid for.

## What is already in place

- **The table.** Files live in `ClassGroupMaterialFiles` (class, document, position) since 2026-10-12, like product and pack photos. Allowing more files is a change of `ClassGroup.MaximumMaterialFiles` and of the screens, with no data migration.
- **The limit that is sold.** Every PDF counts against the `document-storage` feature of the plan (Free 100 MB, Lite 1 GB, Pro 5 GB, Enterprise 10 GB), and more space is already sold as an add-on (2 € per month for 10 GB more). See [Document storage limit](../../storage.md#document-storage-limit).

## Decision: sell space, not a number of PDFs

More PDFs per class are paid through the space they use, not through a new "files per class" add-on:

- A 200 KB PDF and a 19 MB one don't cost the same; space charges the instructor who uploads a lot, and doesn't limit the one who uploads little.
- It needs no new feature, price or text on the plans screen: the existing storage add-on already covers it.
- If a count limit is wanted anyway (for example 1 PDF per class on Free, 5 on Pro), the subscriptions library supports it: a counted feature like `students` or `team`, which can also be an add-on. It is a second concept to explain, so it waits until someone asks for it.

## Steps when it is built

1. **Backend.** Raise `ClassGroup.MaximumMaterialFiles` (a constant, or a per-plan limit if the count is ever sold). Uploading past the limit is refused instead of replacing the oldest file. `DELETE /api/class-groups/{id}/material/{documentId}` removes one file; reordering reuses the catalog image gallery rules. `ClassGroupResponse.MaterialFile` and `StudentAppClassMaterialResponse.File` become lists.
2. **Team form.** "Material para los alumnos" lists the files (name, size, Ver PDF, Quitar) and keeps "Subir PDF" until the limit, the same way product photos work.
3. **Student app.** The class detail lists every file under "Material de la clase", after the link.
4. **Tests.** Uploading up to the limit, refusing one more, removing one file, and the student seeing every file.
