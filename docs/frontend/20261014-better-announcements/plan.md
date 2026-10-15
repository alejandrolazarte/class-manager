# Better announcements

Date: 2026-10-14

Announcements ("Novedades") were a title and a text sent to every student. This plan makes them closer to what class apps use (Mindbody, Glofox, ClassDojo, Remind): who they are for, what kind they are, a photo, and one that stays on top. The design was drawn on a canvas before building (private to the owner): [Novedades, propuesta de rediseño](https://claude.ai/artifact/TRfjsYrAeNcwvmj2fQhtns).

## Step 1 (built)

### What changes

| | Before | Now |
|---|---|---|
| Who sees it | Every student | Every student, or only the students enrolled today in the chosen classes. The push follows the same rule |
| Kind | None | `News` (Novedad), `Important` (Importante), `Event` (Evento) or `Reminder` (Recordatorio): a pill, an icon and a tone from the existing status colors |
| Photo | None | One photo, stored like a class pack photo |
| On top | None | One pinned announcement per business, shown first in Inicio and Novedades of the student app |
| Until when | Shown for 60 days | Optional "Visible hasta": students stop seeing it the day after |
| Text | 500 characters | 1000 characters |
| Team screen | The form above the list | The list with the floating "Nuevo aviso" button, and the form on its own screen (`/settings/announcements/new`). Each row pins or unpins and deletes with undo, as before |

### Decisions

- **One pinned announcement.** Two pinned announcements compete and none stands out. Pinning one (from the list or when publishing) unpins the other. A filtered unique index (`IX_Announcements_TenantId_Pinned`, `[IsPinned] = 1 AND [IsDeleted] = 0`) enforces it, so the use case unpins the old one in a first save and pins the new one in a second, inside the command's transaction. Restoring a deleted pinned announcement brings it back unpinned when another one was pinned meanwhile.
- **Pinned announcements are listed even when old.** The team list keeps its 180 days and the student news its 60, plus the pinned one at any age, so it never disappears while it is pinned.
- **Classes, not students.** The audience is a list of classes (`AnnouncementClassGroups`, a link table with `TenantId` like `ClassPackClassGroups`). A student sees the announcement while one of the client's students has a current enrollment in one of them; the student app says which of their classes it is for ("Para tu clase de …"). Choosing single students is left for later.
- **"Visible hasta" is a business date**, inclusive, not a record date (`IExpiredOn` is for replaced records). It can't be before today.
- **The photo is a document.** `AnnouncementImages` links the announcement to `Documents` (one photo, `Announcement.MaximumImages`), so it counts against the plan's document storage and is served from Blob Storage like shop photos (path `<tenantId>/announcements/<announcementId>/<documentId>.<extension>`). The app publishes first and uploads the photo after; if the upload fails the announcement stays published and the toast says the photo was not uploaded. The push goes out when the announcement is published and carries no photo.
- **The category list is fixed.** A business can't add categories: four kinds with distinct tones are enough to scan the feed, and they reuse the status colors, so every theme already passes the contrast checks.

### Endpoints

| Endpoint | What it does |
|---|---|
| `POST /api/announcements` | Now also takes `category`, `classGroupIds`, `isPinned` and `visibleUntil`. 400 `announcement.class_group_not_found` when a class is not the business's |
| `POST /api/announcements/{id}/pin`, `DELETE /api/announcements/{id}/pin` | Pins (unpinning the current one) or unpins |
| `POST /api/announcements/{id}/images`, `DELETE /api/announcements/{id}/images/{documentId}` | Adds or removes the photo, same errors as class pack photos |

All of them require `announcements.manage`. The student news items carry `category`, `isPinned`, `imageUrl` and `audienceClassNames`.

## Step 2 (built): who saw it and answers

### What changes

| | Before | Now |
|---|---|---|
| Team list | Category, audience, pin and photo | Also "Vista por N de M" with a thin bar and, for events, "Van N · No van M". Tapping a row opens who saw it |
| Who saw it | Unknown | A screen per announcement: counts (La vieron, Sin ver, and for events Van and No van), the families that saw it and when, the ones that didn't, and their answers |
| Remind | Publish again | "Recordar a quienes no lo vieron" sends the push again ("Recordatorio: …") only to the families that haven't seen it |
| Events | Read only | Students answer "¿Vas a ir?" with Voy or No voy, and can change it |

### Decisions

- **Seen is per family, not per person.** The team thinks in families (a client), and a family with two accounts has seen it once someone opened it. `AnnouncementViews` (`AnnouncementId`, `ClientId`, `SeenAt`) keeps one row per family the first time it opens Novedades while the announcement is visible to it; answering an event counts as seeing it.
- **"Out of M" counts families that can see it in the app.** Families without an app account can't see an announcement, so they don't count: M is the families with at least one app account that are in the audience today (everyone, or a current enrollment in one of its classes). Views and answers from a family that left the audience no longer count.
- **One answer per family, the last one wins.** `AnnouncementReplies` (`AnnouncementId`, `ClientId`, `IsGoing`, `RepliedAt`) is edited in place; only events can be answered (400 `announcement.not_an_event`), and only by families that can see the announcement (404 otherwise).
- **The reminder doesn't change the announcement.** It is a push, not a new announcement, so the date and the order in the news stay the same.

### Endpoints

| Endpoint | What it does |
|---|---|
| `GET /api/announcements` | Each announcement now carries `recipientCount`, `seenCount`, `goingCount` and `notGoingCount` |
| `GET /api/announcements/{id}/recipients` | The families in the audience with the app, with `seenAt` and `isGoing` |
| `POST /api/announcements/{id}/reminder` | Pushes the announcement again to the families that haven't seen it; returns `remindedCount` |
| `PUT /api/student-app/news/announcements/{id}/reply` | `{ "isGoing": true }`: the family's answer to an event |

The team endpoints require `announcements.manage`. The student news items carry `announcementId` and `isGoing`.

## Next steps (proposal)

1. **Schedule.** Publish at a date and time, with "Programados" in the team list. Runs on the background tasks worker.
2. **A button in the announcement.** Take the student to the shop, their week or a link.
3. **Preview and editing.** See the announcement as the student and as a push before publishing, and fix a published one without deleting it.
