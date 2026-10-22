# Database-generated `bigint` ids

Status: proposal.

Every id in the app is a `Guid` version 7 created in code (`Guid.CreateVersion7()` in each entity's factory). This plan replaces them with `long` ids that SQL Server generates on insert (`bigint IDENTITY`).

## Why

The target is thousands of businesses with thousands of students each. The table that grows is not students but attendance: 5,000 businesses × 1,000 students × 3 classes a week × 52 weeks ≈ 780 million `Attendances` rows a year, plus sessions, bookings and payments.

- **Size.** A `uniqueidentifier` takes 16 bytes and a `bigint` 8. An attendance row carries four ids (`Id`, `TenantId`, `ClassSessionId`, `StudentId`): 64 bytes against 32, about 25 GB a year more in the clustered index alone. Every nonclustered index also stores the clustered key, so each one grows too. Bigger indexes mean less of the data fits in SQL Server's memory and more reads from disk, which in Azure SQL is cost.
- **Inserts.** SQL Server orders a `uniqueidentifier` by its last six bytes first. A .NET `Guid` v7 keeps its timestamp in the first bytes and the random part at the end, so to SQL Server each new id lands at a random place in the clustered index. That causes page splits and fragmentation, the same as `Guid.NewGuid()`. `docs/backend/domain-model.md` says v7 ids are "index-friendly in SQL Server". That is not the case for `uniqueidentifier`. An `IDENTITY` value always goes at the end of the index.
- **Joins and seeks** compare half the bytes. A single lookup by id is about as fast with either type. The gain comes from the size of the indexes.

`int` is not enough. It stops at about 2.1 billion, which attendance would pass in a few years.

## Decision

- Ids are `long` in C# and `bigint IDENTITY(1,1)` in SQL Server. EF Core configures an integer key as `ValueGeneratedOnAdd` by convention, so SQL Server assigns the id when the row is inserted and EF Core reads it back on `SaveChanges`. No code creates ids.
- **What changes:** the primary key of every table in `AppDbContext`, in the backoffice context and in the billing schema (`Subscriptions`), plus `BackgroundTasks`. Every foreign key to those tables changes too, including `TenantId` and the `CreatedByUserId`, `UpdatedByUserId`, `DeletedByUserId` and `RegisteredByUserId` columns.
- **What stays a `Guid`:**
  - Identity accounts (`ApplicationUser : IdentityUser<Guid>`) and the Security tables (`RefreshTokens`, `PasswordResetTokens`, `EmailChangeTokens`). They have few rows, and nobody should be able to guess their ids. `User.AccountId` and `TokenSubject.UserId` refer to the account, so they stay `Guid` too.
  - `PrivateLesson.SeriesId`. It only groups lessons, it is not a key of any table, and nothing generates it on insert.
  - File names in blob storage and the `RowKey` suffix of the command log. Neither is a SQL key (see [Ids that can be counted](#ids-that-can-be-counted)).
- **Why `IDENTITY` and not a sequence with HiLo.** HiLo hands out ids when an entity is added to the `DbContext`, before `SaveChanges`. That only helps code that needs an id between `Add` and the save. Once the cases below are fixed, no code does. `IDENTITY` is the simplest option and the one SQL Server and EF Core use by default. EF Core still batches inserts into identity tables and reads the new ids back in the same round trip.
- The primary key stays the clustered index. Queries already go through the indexes that start with `TenantId`.

## What changes because the id doesn't exist until the save

Today a new entity has its id the moment its factory returns. With `IDENTITY`, its `Id` is `0` until `SaveChanges`. Every place that reads the id of an entity that hasn't been saved yet must change.

1. **Foreign keys copied from a new entity.** About 20 factories copy another entity's id into a foreign key, for example `UserId = contact.Value.Id` in `Client.Create`, or `UserId = user.Id` in `BusinessMember` and `ClientAccount`. The same pattern appears in `MeasurementRecord`, `MeasurementGoal`, `OrderLine`, `ClassPackPurchase`, `ProductImage`, `ClassPackImage`, `ClassGroupMaterialFile` and `Student`. When the related entity is new, the factory sets the navigation property instead, and EF Core fills the foreign key on save. An entity without that navigation gets one. When the related entity is already saved (an existing product in an order line), copying the id keeps working.
2. **Comparing ids of entities that haven't been saved.** In `RegisterClientUseCase.JoinSamePeopleAsync`, the check `student.UserId == contact.Id` would be true for every student, because both ids are `0`. Code like that compares the objects themselves (`ReferenceEquals(student.User, contact)`) instead of their ids. The 27 `.Id ==` comparisons in `src/Core` get the same review, as do dictionaries, `HashSet`s and `GroupBy`s keyed by the id of an entity created in the same command.
3. **Raw SQL that inserts a row.** `UserRepository` inserts users with `Guid.CreateVersion7()` as `[Id]`. The `INSERT` drops the `[Id]` column and lets `IDENTITY` assign it.
4. **Document file names.** `DocumentStorageService.StoreAsync` names each file after the new document's id before the document is saved. The file name becomes a random `Guid.NewGuid()` in `"N"` format, separate from the `Documents.Id` key, and `Document.Create` no longer takes an id. The folder still uses the tenant id and the owner's id, and every owner (a class group, a product, a class pack) has already been saved by then.
5. **`Guid.Empty` used as "no value".** There are 16 of these in `src`. Each one becomes a nullable `long?`, or a check for `0` where `0` can only mean "not saved yet".
6. **Advisory locks.** `PeopleLock` and `StockLock` build their lock names from ids with `ToString("N")`. They switch to `ToString(CultureInfo.InvariantCulture)`.

Use cases already return their response after `SaveChangesAsync`, and the team notifier and the background task outbox queue their work after the business change is saved. Both read the new ids from the database.

## Libraries

- **Tenancy:** `ITenantOwned.TenantId`, `ITenantContext.TenantId` and `ITenantScope.Establish(...)` become `long`. `ClaimsTenantContext` reads the `tenant_id` claim with `long.TryParse`, and treats `0` as no tenant, as it does with `Guid.Empty` today.
- **Records:** `IAuditable.CreatedByUserId` and `UpdatedByUserId`, and `ISoftDeletable.DeletedByUserId`, become `long?`.
- **Subscriptions:** `Subscription.Id`, `SubscriberId`, `SubscriptionFeature` and `ISubscriberResolver.ResolveCurrentSubscriberIdAsync` become `long`.
- **Security:** `TokenSubject.TenantId`, `RefreshToken.TenantId`, `LastTokenSession`, `ITokenIssuer.RefreshAsync`, `ITokenIssuer.SwitchAsync` and `ITokenSubjectResolver.ResolveAsync` take a `long` tenant id. Account ids stay `Guid`.

The libraries switch to `long` directly. They are only used by this app, and a generic id type would be complexity nobody needs today. If one of them becomes a NuGet package, its id type can be decided then ([security](../../security.md#extracting-it-to-a-nuget-package-later)).

## API

- The 33 `{...:guid}` route constraints in `ApiRoutes` become `{...:long}`.
- Ids travel as JSON numbers. JavaScript numbers are exact up to 2⁵³ − 1 (about 9 × 10¹⁵), far beyond what `IDENTITY` will reach, so ids don't need to be sent as strings.
- Links built from ids (`TeamNotificationLinks`, push payloads) carry numeric ids. Links sent by email don't change: invitations, guardian consent and password resets use token hashes, not ids.

## App and e2e

- About 550 ids are typed `string` in `app/src` (`id: string`, `clientId: string`, …). They become `number`, and the typecheck finds every one.
- Expo Router passes route parameters as strings. A single helper in `src/navigation` turns a parameter into a number, and a value that isn't a positive integer shows the screen's not-found state, as an unknown id does today.
- `keyExtractor` and React Query keys convert the id with `String(id)` where they need a string.
- The e2e helpers (`e2e/support/businessApi.ts`) and the screenshot seeders (`e2e/screenshots`) read and send numeric ids.

## Ids that can be counted

Sequential ids can be guessed: whoever sees `/students/clients/48213` knows `48212` probably exists.

- **Reading another business's data is still blocked** by the tenant query filter, which is applied centrally to every query ([tenancy](../../tenancy.md)). Each tenant-owned entity already has a test proving that business A can't read business B's data, and those tests keep running.
- **Links that grant access without signing in** (invitations, guardian consent, password reset, email change) use random tokens, never ids. That doesn't change.
- **Public photo URLs** stay unguessable, because the file name is a random GUID and not the document id (see point 4 above).
- **What an id still reveals** is roughly how many rows the platform has, for example that a new client gets id 48213. That is accepted.

## Database, migrations and existing data

The pilot database only holds the owner's test data, so it is dropped and created again. No data is converted.

- **Migrations:** the migrations of `AppDbContext`, the backoffice context and `SecurityDbContext` are deleted. Each context gets one new `Initial` migration. The Security context needs one too, because its `TenantId` columns change type. The data-fix migrations (`UsersBackfill`, `WhoDidWhatBackfill`, `PeopleCopiesRemoval`) go with them: there is no old data left to fix.
- **Pilot on Azure:**
  1. Delete the database and create it again with the same name and tier ([pilot deployment](../../pilot-deployment.md)). The deploy pipeline then applies the new migrations.
  2. Empty the photos container. Its paths contain the old tenant ids.
  3. Empty the command log tables. Security event blobs can't be deleted while their retention policy holds, so they stay with the old ids until they expire. The backoffice reader already ignores a partition key it can't parse.
  4. Sign up again, run `scripts/enable-first-operator.sql` and `scripts/set-subscription.sql`.
- **Local and e2e databases:** drop them and migrate again ([local development](../local-development.md), `cd e2e; pnpm db:migrate`).
- **Rollback:** a rollback to an image from before this change can't work, because that image expects `uniqueidentifier` columns. [Rolling back a deploy](../../rollback.md) gets a note saying so.
- **Installed apps:** the PWA and the Android APK hold a refresh token from the old database. It no longer exists, so the API answers 401 and the person signs in again.

## Tests

- Unit tests create entities in memory, which keeps their `Id` at `0`. Tests that need distinct ids (a mocked repository that finds by id, two entities that must differ) set them with a helper in the test projects, `EntityIds.Assign(entity, id)`, which writes the private setter through reflection. Core doesn't get a public setter for tests. The roughly 300 `Guid.CreateVersion7()` and `Guid.NewGuid()` calls in `tests` become numeric constants.
- New tests:
  - **Unit:** registering a client whose student is the contact still links the student to the same person (point 2).
  - **Integration:** two entities saved in one `SaveChanges` get different ids, assigned by the database.
  - **Integration, model test:** every entity's key is a `long` generated on add, except the entities listed as staying `Guid`. It follows the same idea as the column convention tests for records.
- Every existing test keeps passing, the tenant isolation tests included. None is deleted or weakened.

## Steps

Ids are foreign keys everywhere, so the types can't change in pieces while `main` stays deployable. The change is one pull request, built in this order:

1. Libraries (Tenancy, Records, Subscriptions, Security), entities, EF Core configuration and the new `Initial` migrations. Then fix every compile error in Core, Infrastructure, Api and the backoffice, and the cases in [What changes because the id doesn't exist until the save](#what-changes-because-the-id-doesnt-exist-until-the-save).
2. Tests: the `EntityIds` helper, the switch to numeric ids, and the new tests above. Everything passes with `-warnaserror`.
3. App and e2e: numeric ids, the route parameter helper, and the seeders.
4. Docs: [domain model](../domain-model.md) (identifiers), [tenancy](../../tenancy.md), [security](../../security.md), [storage](../../storage.md) (file names), [deleting data](../../deleting-data.md) (`DeletedByUserId`), [rollback](../../rollback.md), [local development](../local-development.md).
5. After it is merged: recreate the pilot database and storage as described in [Pilot on Azure](#database-migrations-and-existing-data).

Size: about 1,600 `Guid` occurrences in `src`, 700 in `tests` and 550 ids in `app`. Most of the change is mechanical, and the compiler and the TypeScript typecheck find almost all of it. The review should focus on the cases above, where code reads an id before the save.
