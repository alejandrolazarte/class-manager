# Profile photos

Date: 2026-10-29

Instructors, the team and students can add a profile photo. Photos are private: they live in the private Blob Storage container, are only ever kept at avatar size, and each one is shown only to the people who need to recognize that person inside the business.

## Who can add one

| Person | Rule |
|---|---|
| Owner and team (instructors included) | Always. Accepting a team invitation already requires the minimum age of the country (or the guardian's authorization), see [Ages and minors](ages-and-minors.md) |
| Client who pays and students from the minimum age of the branch's country (13 in Argentina, 14 in Spain, 16 elsewhere, `Countries.OwnAccountMinimumAge`) | Always |
| Students below that age, or without a known birth date | They upload it, and it **waits for the client who pays** (the guardian) before anyone else sees it |

The age is the most protective of the two birth dates (the one the team typed and the one on the account), the same rule as [what each account of a family sees](ages-and-minors.md#what-each-account-of-a-family-sees) (`StudentAppFamily.OwnStudentNeedsGuardian`, `FamilyVisibility.NeedsGuardian`).

### Guardian approval

- Uploading as a minor stores the photo with `UserPhotos.AwaitsGuardian = 1` and sends a push to the accounts of the client who pays ("Tomás quiere poner una foto de perfil").
- The client's app home shows a card with the photo and **Aprobar** / **No aprobar** (`GET /api/student-app/photo-approvals`, `POST .../{photoId}/authorization`, `POST .../{photoId}/refusal`). Refusing asks first and deletes the photo and its file. Only the client who pays of that family can answer; anyone else gets `404`.
- Until it is approved, only the child (in Mi perfil, with "Esperando que tu responsable la apruebe") and the guardian (in the card) see it.
- A new photo replaces the previous one; for a minor it waits again.
- The guardian has to use the app to answer. An email link, like the one for the [app authorization](ages-and-minors.md#authorization-of-the-client-who-pays), is the next step if guardians without the app need it.

## Who sees it

| Photo of | Seen by | Where |
|---|---|---|
| An instructor | The team, and the students of the business | The class detail where students are enrolled, class attendance, the instructor list, the instructor picker of group classes and private lessons, and the instructor card (team); the next class, the class list and the class detail of the student app (larger there), next to the instructor's comment on the home and in its news |
| A student | The team | Class attendance, the class roster, the student list and pickers, and the client detail (each student, and the header for the client who pays) |
| Anyone of a family (the client who pays and every student of that client) | Every account of the same family | Tu familia, the student buttons of the app home, and Ajustes |
| Anyone | Themselves | Mi perfil, and the avatar right of the bell in the Inicio header (team and student), which opens Mi perfil |

Students never see the photos of students of other families. Each response that carries a photo URL decides who sees it (`IUserPhotoUrlService.OfStudentsAsync` / `OfInstructorsAsync` / `OfUsersAsync`, also used by `GET /api/instructors`, `GET /api/students`, `GET /api/clients/{id}` and the class roster), and photos awaiting the guardian are never returned there.

The privacy policy (version 2026-10-30) explains who sees a profile photo and that a minor's photo waits for the guardian, see [legal texts](legal-and-privacy.md).

## Only avatar size

The original upload is never stored. `SkiaAvatarImageService` (SkiaSharp, MIT) decodes PNG, JPG or WebP of up to 10 MB, turns it upright using its EXIF orientation, crops the centered square and saves a **256 × 256 WebP**. Re-encoding from pixels drops every metadata block, so the location a phone writes into a photo never reaches storage. There is no full-size version to open: the app shows it as an avatar (40 px in lists, 88 px in the class detail) and has no viewer to enlarge it.

## Storage and links

- One `UserPhotos` row per person of the business (`UserId` unique), pointing to a `Documents` row with `Visibility = Private`, at `<tenantId>/users/<userId>/<random>.webp`. It counts against the business's [document storage](storage.md#document-storage-limit) (a photo is about 10 KB).
- The photo belongs to the person **in that business** (`Users`), not to the account: a person in two businesses adds it in each one, and one business never sees a photo given to another.
- Responses carry a **signed read link** (SAS) built by `IFileStorage.PrivateReadUrlOf`, valid for one to two hours. Its expiry is rounded to the hour, so the same photo has the same URL for an hour and browsers and phones cache it. The expiry uses the real clock (`TimeProvider.System`), because Azure checks it against its own clock.
- Removing a photo, replacing it, the guardian refusing it, and **deleting the account** delete the row and the file (physical delete: a photo has no value once its owner removes it). Account deletion removes the photos of every business of that account.

## Endpoints

| Endpoint | What it does |
|---|---|
| `GET /api/me/photo` | `{ url, awaitsGuardian, needsGuardian }` of the signed-in person in the current business |
| `PUT /api/me/photo` | Multipart `file`. 400 `image.unsupported` or `user_photo.too_large` (10 MB), 403 `feature.limit_reached` when the business has no space left |
| `DELETE /api/me/photo` | 204, or 404 `user_photo.not_found` |
| `GET /api/student-app/photo-approvals` | Photos of the family waiting for the client who pays (empty for other accounts) |
| `POST /api/student-app/photo-approvals/{photoId}/authorization` and `/refusal` | Approve, or refuse and delete |

## Tests

`tests/ClassManager.Api.I.Tests/Endpoints/UserPhotos` (avatar size and format, private without signature, replace and remove delete the file, guardian approval and refusal, another family can't answer, students see the instructor's photo, the team doesn't see a child's photo until approved), `Persistence/When_UserPhoto_belongs_to_another_business`, `Accounts/When_a_student_deletes_their_account/Then_their_photo_is_deleted`, and the signed link tests in `tests/ClassManager.Storage.I.Tests`.
