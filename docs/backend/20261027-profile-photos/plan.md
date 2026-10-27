# Profile photos

Date: 2026-10-27. Status: proposal, not built.

Today every person is shown with the initials of their name (`app/src/ui/Avatar.tsx`). This plan lets a student (or the adult who manages their account) and an instructor or team member add a photo, shown as the avatar instead of the initials. It covers who sees each photo, where the file is kept and how it is served.

## Competitors

| System | Profile photo | Who uploads it |
|---|---|---|
| Glofox | Yes | The member from the member app, and the staff from the dashboard or the staff app |
| Jackrabbit | Yes | The staff from the student record or the staff portal, and the parents from the parent portal |
| Momence | Yes | The customer from their account |
| Gymdesk, Mindbody, Arbox, TeamUp | Not confirmed | No documentation found |

None of them documents opening the photo full screen or blocking screenshots. The only screenshot blocking request found is for the access barcode in ABC Fitness (Glofox's owner), and it is a user suggestion, not a feature.

## Decisions

| Who | Sees |
|---|---|
| The student, or the adult who manages their account | Their own photo as the avatar. They add, change and remove it in Mi perfil |
| The team of the business | The student's photo as the avatar in lists and in the student detail |
| The student, about their instructors | The instructor's avatar in their classes (where it says "con {instructor}"), in class comments and in the news |
| A student, about other students | Nothing. The student app shows no other students today, and a "who comes to this class" list would tell any family the face, day, time and place of every other student, children included. If a business asks for it, it is a separate plan: turned on by the business, each person (or their adult) chooses to appear, minors not shown |

- **Avatar only, no full screen view.** A full screen view (and blocking screenshots while it is open, possible on Android, partly on iOS, not on the web) waits until businesses ask for it.
- **Each person decides about their own photo.** The team does not upload photos of students in this first version. Glofox and Jackrabbit allow it; it can be added later with the `Students.Manage` permission if businesses ask.
- **No photo, initials.** Exactly as today.

## Storage: private, with links that expire

A profile photo is personal data, often of a minor. It goes to the **private** container (`FileVisibility.Private`), not the public one used by shop photos, announcement photos and class PDFs (see [storage](../../storage.md)):

- A public URL can be opened by anyone who has it, without signing in, forever. Random file names stop guessing, not sharing (a screenshot of a link, a forwarded message, browser history).
- Public files are served with `Cache-Control: public, max-age=31536000, immutable`, so a removed photo could stay in caches for a year. Private ones use `private, no-store`.

### Signed links (SAS)

`storage.md` leaves signed links for private files as not built. This plan builds them:

- `IFileStorage.SignedUrlOf(path, expiresOn)` returns a read-only URL for one private file, valid until `expiresOn`. `AzureBlobFileStorage` builds a service SAS with the account key of the connection string (Azurite supports it). Moving to managed identity later means a user delegation SAS inside the same method.
- **The expiry is rounded, not "now + 1 hour".** It is the start of the current hour plus 2 hours. The same file gets the same URL for the whole hour, so the browser and the phone can cache it while the list is reloaded, and a URL is valid for at least one hour after it is handed out.
- The private container's `Cache-Control` stays `private, no-store` for other private documents; profile photos set `private, max-age=3600` when saved, so caching is only in the viewer's own browser or phone, and never past the link's life.
- `DocumentStorageService` gets `SignedUrlOf(Document)`, which refuses public documents, the same way `PublicUrlOf` refuses private ones.

### Model

Like every other file, a `Documents` row and a link table (see [storage, Model](../../storage.md#model)):

| Table | Columns | Notes |
|---|---|---|
| `UserPhotos` | `Id`, `TenantId`, `UserId`, `DocumentId`, audit columns | One per user: unique index on `UserId`. A `User` is one person in one branch, so the student, the instructor and the team member are covered by the same table |

- **Path**: `<tenantId>/users/<userId>/<fileName>.<extension>`, with a random file name, `Private` visibility.
- **Replacing** removes the old link, `Documents` row and file in the same save, as replacing a class PDF does.
- **Removing** is a physical delete of the row and the file, with no undo: a photo has no value once its owner removes it, and keeping it would contradict the owner's wish. The app asks first with `ConfirmDialog`.
- **Deleting the account** (`DeleteMyAccountUseCase`) deletes the photos of every `User` of that account. The `Users` row is kept, as today, without a photo.
- **Soft deleting a student** keeps the photo, so restoring them brings it back.
- **Space.** A photo counts against the `document-storage` feature like any other document. After resizing (below) it is around 50–150 KB, so 200 students use less than 30 MB. Past the limit the upload is refused with `feature.limit_reached`.

## Upload

- **Resized and cleaned in the phone or browser before uploading**, with `expo-image-manipulator`: at most 800 px on the long side, JPEG at about 80 % quality. Re-encoding drops the EXIF data, which can hold the GPS position where the photo was taken (often the student's home). 800 px is enough for every avatar size and leaves room for a full screen view later.
- **The API checks anyway**: format by its first bytes (`ImageFormat`, PNG, JPEG or WebP) and at most 1 MB. It does not trust that the app resized it.
- **Picking the photo** uses `expo-image-picker` (camera or gallery on the phone, a file on the web), with a square crop on the phone. The current pickers use `expo-document-picker`, which offers no camera.
- Both packages are Expo modules with no install scripts, so `pnpm.onlyBuiltDependencies` doesn't change.

## Endpoints

| Endpoint | Who | What it does |
|---|---|---|
| `PUT /api/me/photo` | Any signed-in person, about their own `User` in the current branch | Uploads or replaces the photo. 400 `image.unsupported` or `image.too_large`, 403 `feature.limit_reached` |
| `DELETE /api/me/photo` | Same | Removes it. 404 `user_photo.not_found` when there is none |
| `PUT /api/student-app/students/{studentId}/photo`, `DELETE` on the same route | The adult whose `ClientAccount` covers that student (a child without their own account) | Same, for that student's `User` |

Responses that already carry a person (students, instructors, team, roster, the student app's classes, comments and news) add `photoUrl`, `null` when there is no photo. URLs are built when answering (`SignedUrlOf`), never stored.

Commands carry no text about people, so no new `[Audit*]` attribute is needed; the file content never reaches the command log.

## App

- `Avatar` gets an optional `photoUrl`. With it, it shows the photo cropped to the same circle and size; without it or if the photo fails to load, the initials, as today. Every screen that shows a person already uses `Avatar`, so they only pass the new prop.
- **Mi perfil** (team and student app): the avatar with "Cambiar foto" / "Quitar foto". Quitar asks with `ConfirmDialog`. For a child, the adult does it from the child's profile in the student app.
- **Student app**: the instructor's avatar next to "con {instructor}" in the class rows and class detail, in `FeedbackNote` and in the news about a class change.
- New texts go in `es.json` and `en.json`. The terms and privacy texts (Spanish) mention that a photo is optional, who sees it and how to remove it.
- `FILES_BASE_URL` is already in the CSP `img-src`, and signed URLs use the same origin.

## Tests

| Project | What |
|---|---|
| `ClassManager.Storage.I.Tests` | A signed URL downloads a private file before it expires and fails after; the same path within the same hour gives the same URL; a public file is refused |
| `ClassManager.Core.U.Tests` | Upload checks (format, size), replacing removes the old document, removing, the space limit |
| `ClassManager.Api.I.Tests` | Upload, replace and remove through each endpoint; the photo URL in the student, team and student app responses; an adult can't change the photo of a student their account doesn't cover; another student can't see it; business B can't read business A's `UserPhotos` (tenant isolation); deleting the account deletes the file |
| `app` (jest) | `Avatar` shows the photo and falls back to initials; Quitar foto does nothing until confirmed; the student app shows the instructor's photo |

## Steps

1. **Signed links**: `SignedUrlOf` in `src/Storage` and `src/Storage.AzureBlob`, with its tests. Update `storage.md` (private files can now be served).
2. **Backend**: `UserPhotos`, migration, use cases, endpoints, `photoUrl` in responses, account deletion.
3. **App**: `Avatar` with photo, Mi perfil, the child's profile, the instructor in the student app. Before and after screenshots of Mi perfil and a list with photos.
4. **Legal texts**: the privacy policy mentions profile photos.

## Later, if asked

- The photo full screen from the student detail, with screenshots blocked while it is open (`expo-screen-capture`).
- The team uploading a student's photo (`Students.Manage`).
- Students seeing each other, under the conditions above.
