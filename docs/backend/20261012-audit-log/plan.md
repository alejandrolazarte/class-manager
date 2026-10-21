# Audit log

Status: steps 1 to 5 built (step 3's Azure setup done on 2026-10-19, see the [runbook](../../audit-log-storage.md)); the rest is a proposal.

Three pieces answer "who did what, and who tried what they were not allowed to":

1. **Audit columns** on every table: who created and who last changed a row, and who deleted it when it is soft deleted.
2. **Command log**: every command (`ICommand`) that ran, by whom, with its input after removing sensitive fields, written by a use case behavior to Azure Table Storage.
3. **Security events**: 403 responses and the 401 that matter (failed sign-ins, invalid refresh tokens), written by a middleware to append blobs under a time-based retention policy, so nobody can change or delete them until they expire.

The history lives in the command log, not in SQL. The columns only keep the latest state. Both logs live in their own storage account that only accepts Microsoft Entra identities (no account key, no anonymous access), and the API can add entries but never change or delete them ([Access](#access-private-and-append-only)).

## Why Azure Table Storage and not SQL

| Option | Cost at pilot scale | Notes |
|---|---|---|
| Azure Table Storage (LRS) | Cents per month | About USD 0.045 per GB per month and about USD 0.00036 per 10,000 writes. 10 businesses × 2,000 commands a day ≈ 600,000 rows a month ≈ 0.6 GB: well under USD 0.10 a month |
| A table in Azure SQL | Uses the free offer | Every write keeps the serverless database busy and uses its 100,000 vCore-seconds and its 32 GB ([hosting plan](../../hosting-plan.md)). An audit row on every command roughly doubles the writes |
| Logs in Log Analytics | About USD 2–3 per GB ingested | Good for searching, expensive for a permanent record |

Prices are approximate (checked 2026-10-12). Check the Azure Table Storage pricing page before relying on them.

The logs get their own storage account, separate from the one with the photos ([storage](../../storage.md)); an account costs nothing by itself, only what it stores, and the reasons are in [Access](#access-private-and-append-only). In development and tests it runs on Azurite, which already runs for blob tests.

The trade-off: the command log is written outside the SQL transaction. If the process dies between the commit and the write, the row is lost; if the write fails, the command still succeeded. That is acceptable for an operational audit. If an audit with legal weight is ever needed, the [background tasks](../20261007-background-tasks/plan.md) already give the guarantee: queue a `WriteAuditEntryBackgroundTaskCommand` and its handler writes the row to Table Storage.

### Why not the background tasks today

The app already has a durable queue: the `BackgroundTasks` table, its worker and the doorbell. Reusing it was considered and left for later:

- **It costs SQL writes on every command.** A task is inserted, claimed (`ExecuteUpdate`) and deleted: three writes in Azure SQL per command, in the free offer whose allowance is the reason the log goes to Table Storage. Today the table only gets order emails and pushes, a few per day; the log would be every command.
- **In the transaction it can't see the outcome.** To commit with the command, the audit behavior would have to run inside `CommandTransactionBehavior`. Then an exception rolls back the entry too, and the log never shows the commands that threw. Outside the transaction it is a separate save and the guarantee is gone, so it is only the cost.
- **Losing a few seconds of entries in a crash is acceptable** for an operational log. Losing an order email is not, which is why emails use the table.

The in-memory channel follows the same shape as the background tasks (queued by the request, written by one worker, drained on shutdown) without touching the database. If the guarantee becomes necessary, switching is one change in the behavior: queue a background task instead of writing to the channel.

## 1. Audit columns

### Names: `*At` for events, `*On` for records that are replaced

The code already uses one convention for events: a moment something happened is `<Verb>At` (`CreatedAt` in 33 entities, `CancelledAt`, `RevokedAt`, `AcceptedAt`, `UpdatedAt`), and the user who did it is `<Verb>ByUserId` (`CreatedByUserId`, `CancelledByUserId`, `RecordedByUserId`).

`src/Records` uses `*On` for records that are replaced instead of edited: `CreatedOn` (applies from), `DeletedOn` (another record replaced it), `ExpiredOn`.

Soft delete broke this. `ISoftDeletable.DeletedOn` was an event (a user deleted the row at that moment), but it had the same name as `IDeletedOn.DeletedOn` (another record replaced it), and `ISoftDeletable` had no field for who deleted the row.

**Decision (built): one way to delete, `ISoftDeletable` with `IsDeleted`, `DeletedAt` and `DeletedByUserId`.**

| Interface | Before | After | Meaning |
|---|---|---|---|
| `ISoftDeletable` | `IsDeleted`, `DeletedOn` | `IsDeleted`, `DeletedAt`, `DeletedByUserId` | The row was deleted or replaced. Hidden by the `SoftDelete` filter |
| `IDeletedOn` (records) | `DeletedOn` | Removed | Replacing a record is now a soft delete of the current one plus the new row |
| `IAuditable` (new) | — | `CreatedAt`, `CreatedByUserId`, `UpdatedAt`, `UpdatedByUserId` | Who created and who last changed the row |

Nothing read replaced subscriptions: `IsActiveOn(date)` already required the record to be current, and every query used `WhereCurrent()`. So the replaced records (`Subscription`, `SubscriptionFeature`) became soft deleted rows, and the app has one way to delete. The history stays in the table and can be read by skipping the named `SoftDelete` filter.

What the change touched (13 entities: `BusinessMember`, `OrganizationMember`, `Payment`, `ClientBillingPlanChange`, `DefaultMonthlyFeeChange`, `MeasurementRecord`, `MeasurementDefinition`, `CustomRole`, `ClassPackPurchase`, `Announcement`, `ClassFeedback`, `Subscription`, `SubscriptionFeature`):

- `ISoftDeletable`: `DeletedOn` → `DeletedAt`, `DeletedByUserId` added, `Delete(DateTimeOffset deletedAt)`. `IDeletedOn`, `WhereCurrent()` and `IsCurrent()` removed; `IsActiveOn(date)` checks `IsDeleted`; `WhereNotDeleted()` for the subscriptions library, which doesn't rely on the app's filter.
- `SoftDeleteModelBuilderExtensions`: the check constraint became `CK_<Table>_IsDeleted_DeletedAt`.
- One migration (`UseDeletedAtForSoftDelete`): `RenameColumn` (`sp_rename`, instant, no data copy), drop and create each check constraint, add `DeletedByUserId` (nullable, `null` for rows deleted before the change), and for `billing.Subscriptions` and `billing.SubscriptionFeatures` add `IsDeleted`, set it to 1 where `DeletedAt` has a value, and filter their unique indexes on `[IsDeleted] = 0`.
- The record columns test maps `IsDeleted`, `DeletedAt` and `DeletedByUserId` to `ISoftDeletable` and rejects a column named `DeletedOn`.
- `DeletedByUserId` was left empty until step 2 filled it.

### `IAuditable` (built)

```csharp
namespace ClassManager.Records;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }
    Guid? CreatedByUserId { get; }
    DateTimeOffset UpdatedAt { get; }
    Guid? UpdatedByUserId { get; }
}
```

- Lives in `src/Records` (no dependencies). Named `IAuditable`, as agreed in the [users plan](../20261017-users/plan.md#audit-columns).
- Every tenant-owned entity implements it (47 tables, migration `AddAuditColumns`).
- `UpdatedAt` is not nullable: it is the last write, so a new row has `UpdatedAt` = `CreatedAt` and `UpdatedByUserId` = `CreatedByUserId`. `ClassFeedback.UpdatedAt` (the comment's date, filtered and sorted by it) and `BrandLogo.UpdatedAt` (the logo's `Last-Modified`) already worked that way and became the `IAuditable` ones.
- `AuditSaveChangesInterceptor` in `src/Infrastructure/Persistence` fills them, registered after `SoftDeleteSaveChangesInterceptor`:
  - `Added`: `CreatedAt` = now (from `TimeProvider`) and `CreatedByUserId` = current user, unless the entity set them (`Order.CreatedByUserId` is the buyer or the staff member who placed it); `UpdatedAt` and `UpdatedByUserId` = the same.
  - `Modified`: `UpdatedAt` = now and `UpdatedByUserId` = current user, unless the entity changed them itself.
  - Soft deleted (`SoftDeleteSaveChangesInterceptor` turns the remove into `Delete(now)`): `DeletedByUserId` = current user when empty, on every `ISoftDeletable`. A delete doesn't count as an update: `UpdatedAt` and `UpdatedByUserId` keep the last edit. `Restore()` clears `DeletedByUserId`.
- The current user is `ICurrentUser.UserId`: the session's `Users` row in the branch (claim `user_id`), not the account. All three `...ByUserId` columns are foreign keys to `Users` (`ApplyWhoDidWhatForeignKeys`). Work with no user (the background worker, sign-up before the user exists, seeding) leaves them `null`.
- Existing rows: `CreatedAt` = the migration's time in the 19 tables that didn't have it, `UpdatedAt` = `CreatedAt`, both `...ByUserId` `null`.
- Set-based writes (`ExecuteUpdate`, `ExecuteDelete`) skip interceptors and set the columns themselves, as `AccountLinkRepository` does with `DeletedByUserId`.
- Business columns with their own meaning stay: `RecordedByUserId` (who took the payment), `CancelledByUserId`, `InvitedByUserId`. `UpdatedByUserId` says who last touched the row, not who did a business action.
- Tests: every tenant-owned entity implements `IAuditable`; the record columns test maps `CreatedByUserId`, `UpdatedAt` and `UpdatedByUserId` to it; and through the API, a new row records its creator, an edit by another owner records them as the last editor, and a deleted payment records who deleted it.

## 2. Command log

### The behavior

`CommandAuditBehavior<TCommand, TResponse>` applies to inputs that implement `ICommand` (`where TCommand : ICommand`) and is registered **first**, outside `CommandTransactionBehavior`, so it sees the result after the commit and also sees exceptions:

```csharp
services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(CommandAuditBehavior<,>));
services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(QueryAuditBehavior<,>));
services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(CommandTransactionBehavior<,>));
services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(QueryNoTrackingBehavior<,>));
```

- It measures the duration, runs `nextStep()`, builds the audit entry and **queues** it in an in-memory `Channel<CommandAuditEntry>`. It never waits for Table Storage.
- `AuditLogWorker<CommandAuditEntry>` (a `BackgroundService`) reads the channel and writes in batches (`SubmitTransactionAsync`, up to 100 rows of the same partition per batch). If Table Storage fails it logs a warning and drops the batch: the command already succeeded.
- On shutdown the worker drains the channel. With `minReplicas 0` the container stops after five minutes without requests, which gives it time; a crash loses what was in memory (seconds of entries).
- If the use case throws, the behavior writes the entry with `Outcome = Exception` and the exception type, and rethrows.
- Queries are not logged. A few queries that read a lot of personal data (exports of students) opt in with `[AuditedQuery]` on the query type, and `QueryAuditBehavior` logs only those, in the same table and with the same row. Both behaviors run through `UseCaseAuditRecorder`. Today the only one is `ExportQuery` (the students and instructors downloads). The row stores what was asked (the module) and how many rows went out, never the exported data: copying it would keep the same personal data a second time, for 24 months, and a big export doesn't fit in a row (1 MB).

### Row

Table `Commands{yyyyMM}` (one table per month, see [Retention](#retention)).

| Property | Value |
|---|---|
| `PartitionKey` | `TenantId` (`none` for commands without a tenant, such as sign-up) |
| `RowKey` | Inverted ticks + `_` + a new `Guid` (newest first, unique) |
| `Command` | `typeof(TCommand).Name`, for example `RecordPaymentCommand`, or the audited query's, such as `ExportQuery` |
| `AccountId` | The account (`sub` claim), or empty |
| `UserId` | The session's user in the branch (`ICurrentUser.UserId`), or empty. Both are kept: the account says who the person is across branches, the user matches the `...ByUserId` columns of the rows |
| `Outcome` | `Succeeded`, `Failed` or `Exception` |
| `ErrorKind`, `ErrorCode` | From `Result` when it failed, or the exception type |
| `DurationMs` | Duration of the use case with its transaction |
| `TraceId` | `Activity.Current.TraceId`, the same one as in the logs and in security events |
| `Payload` | The command as JSON, after removing sensitive fields (below) |
| `PayloadTruncated` | `true` when the JSON was cut at 32 KB |
| `RowCount` | Rows the use case returned, when its response implements `ICountedRows` (an export's file); empty otherwise |

### Payload: yes, but without sensitive fields

The payload is what makes the log useful ("changed the fee of client X to 25,000"), so it is stored. It goes through a serializer that removes fields by category, decided per property with an attribute on the command:

| Category | Examples | What is stored |
|---|---|---|
| Secret | Passwords, refresh tokens, reset and confirmation codes, VAPID keys, payment tokens | Never stored: the property is left out |
| Personal | Names, email, phone, birth date, address, measurements, guardian data, free text written about a student | `"[personal]"`. The ids in the command (`StudentId`, `ClientId`) are kept, so the row still says who was affected |
| File | Uploaded files: `Stream` and `byte[]` | Only the size: `"[file: 12345 bytes]"` |
| Everything else | Ids, amounts, dates, enums, flags | As is |

```csharp
public sealed record ResetPasswordCommand(
    [property: AuditSecret] string? Token,
    [property: AuditSecret] string? NewPassword) : ICommand;

public sealed record AddStudentCommand(
    Guid ClientId,
    [property: AuditPersonal] string? FullName,
    [property: AuditPersonal] DateOnly? BirthDate,
    [property: AuditPersonal] string? Notes,
    [property: AuditPersonal] string? Email) : ICommand;
```

The attributes live in `src/Core/Audit`; `CommandAuditPayload` (`src/Infrastructure/Audit`) is a `System.Text.Json` contract modifier that drops `[AuditSecret]` properties, writes `"[personal]"` for `[AuditPersonal]` ones that have a value, and describes files by size. The JSON is cut at 32 KB (`PayloadTruncated`).

Why personal data is masked too, not only secrets:

- Many students are minors ([ages and minors](../../ages-and-minors.md)), and businesses in Spain fall under the GDPR. A copy of every name and birth date in a log that can't be edited is a second database of personal data that has to be erased when a person asks, and Table Storage rows are hard to find by content. Keeping ids makes the log point to the person without copying them.
- The current value is in SQL; the log only needs to say which row changed and what non-personal values were set.
- Decided 2026-10-19: names and emails are masked too. The log says who did what through `AccountId` and `UserId`, and `Users` keeps the name even after the account is deleted; what is lost is the previous value of a personal field, which a business rarely needs.

What is marked (built, 144 properties of the 138 commands and their nested records):

| Category | Properties |
|---|---|
| Secret | `Token`, `Password`, `CurrentPassword`, `NewPassword`, `RefreshToken`, and the web push `Endpoint`, `P256dh` and `Auth` |
| Personal | `FullName`, `OwnerFullName`, `Email`, `NewEmail`, `PhoneNumber`, `BirthDate`, `OwnerBirthDate`, `Notes`, `Note`, class comments (`Text`), cancellation `Reason`, announcement `Title` and `Body` (they can name people), measurement `Value`s and goals, and the `Location` of a private lesson (it can be a home) |
| Plain | Names of businesses, branches, classes, packs, products, roles, measurements and levels; descriptions; codes (country, currency, time zone, language, module); times, months, colors, sizes, units, URLs, permissions |

How it stays correct as commands are added (attributes alone would be forgotten); the tests scan every `ICommand` and the records nested in it:

- **Every text property** (`string` or a collection of `string`) has exactly one of `[AuditSecret]`, `[AuditPersonal]` or `[AuditPlain]`, so new free text can't reach the log without a decision (`Core.U.Tests/Audit`).
- **A name like a secret** (`Password`, `Token`, `Secret`, `Key`, `Endpoint`, `Auth`, `P256dh`) must be `[AuditSecret]`, and **a name like personal data** (`FullName`, `Email`, `Phone`, `Birth`, `Address`, `Note`, `Comment`, `Reason`, `Text`, `Body`, `Value`) must be `[AuditPersonal]`, whatever its type.
- **A sample of every command** is built with a distinct value in each secret and personal property and in each file, serialized, and the test fails if any of those values appears in the JSON (`Api.I.Tests/Audit`). Both kinds of test were checked to fail with the masking turned off and with a mark removed.
- `Stream` and `byte[]` are recognized by type; no attribute needed.

Alternative considered: an allowlist (only properties marked `[Audited]` are stored). Safer by default, but every command has to be annotated before the log says anything, and a forgotten attribute silently hides data. Requiring a mark on every text property gives the same safety for free text and fails the build instead.

## 3. Security events

### What is recorded

| Event | Source | Why |
|---|---|---|
| `Forbidden` | Any 403 that is not a plan limit | An authenticated user tried something their role doesn't allow. Rare and worth reading |
| `PlanLimit` | A 403 for a plan limit (`FeatureNotInPlan`, `SubscriptionInactive`, `feature.limit_reached`) | A commercial limit, not an attack, but a direct API call to a feature that wasn't paid for shows here. Its own type so it doesn't bury `Forbidden` |
| `SignInFailed` | A 401 or 423 from the sign-in endpoint (wrong password, locked account) | Password guessing |
| `RefreshTokenRejected` | A 401 from the refresh endpoint (unknown, expired, reused) | A reused refresh token can mean a stolen one |
| `UnauthorizedBurst` | Middleware, 401 counted per IP | Many 401 from the same IP in a minute: one row with the count, not one row per 401 |

Not recorded:

- **A single 401.** Every expired access token returns 401 before the app refreshes it, so most 401s are normal traffic. Worse, anyone can produce 401s without an account: one row per 401 lets a bot fill the table and the bill.

Every 403 is recorded. Plan limits were first left out as a commercial limit, then recorded with their own type (decided 2026-10-19): with one rule ("every 403") a case that slips through unmarked is still recorded, and a business that hits its limits or calls a feature it doesn't pay for is worth seeing. `PlanLimitResponses.Mark` (`src/Subscriptions.AspNetCore`) marks the request in `HttpContext.Items` wherever a plan limit is answered: the `RequireFeature` filter, `ActiveSubscriptionMiddleware`, `PlanAwareAuthorizationResultHandler`, `FeatureLimitExceptionHandler` (a limit reached while saving) and `ResultHttpExtensions` (a use case returning `FeatureErrors.LimitReached`, such as an import). A marked 403 is `PlanLimit`; an unmarked one is `Forbidden`. A business with an inactive subscription gets a 403 on every change, so the per-address cap below is what keeps it to a few lines a minute.

### The middleware

- `SecurityEventMiddleware` (`src/Api/Audit`) runs **before** `UseExceptionHandler`, `UseAuthentication` and `UseAuthorization`, calls `await next()`, then reads `Response.StatusCode`. Placed after them it would not see the 401 and 403 they return, nor the 403 the exception handler writes for a limit reached while saving. It reads the endpoint before calling `next()`, because the exception handler clears it.
- A 403 is queued as `Forbidden` (or `PlanLimit` when marked) with the route template (`/api/students/{id}`, not the real path), the method, the user and the tenant.
- A 401 only increments an in-memory counter per IP (`SecurityEventThrottle`, at most 10,000 addresses tracked). Every minute `SecurityEventBurstWorker` writes one `UnauthorizedBurst` row for every IP with 20 or more 401s, or with events over its cap, and resets the counters.
- Each IP writes at most 10 events a minute (`AuditLog:MaximumSecurityEventsPerAddressPerMinute`); beyond it events are counted into its burst row, not written. Nothing a client sends can make the API write without limit.
- `SignInFailed` and `RefreshTokenRejected` come from the same middleware: the sign-in and refresh endpoints carry `SecurityEventOnRejection` metadata naming the event, so a 401 there is told apart from an expired access token elsewhere without touching the use cases. (Built this way instead of an `ISecurityEventRecorder` called by the use cases.) Neither the email typed nor the token is recorded: the command log already has the failed command with personal fields masked.
- The sign-in, refresh and password reset endpoints keep their rate limiting (`UseRateLimiter`); the events don't replace it.

### Entry

One append blob per day in the container `security-events`: `{yyyy}/{MM}/{dd}.jsonl`, one JSON object per line. Blobs and not a table because only blobs can be immutable in Azure Storage ([Access](#access-private-and-append-only)). There are few events a day, so reading a whole day is cheap.

| Field | Value |
|---|---|
| `occurredAt` | Date and time, UTC |
| `tenantId` | When known, otherwise empty |
| `event` | `Forbidden`, `PlanLimit`, `SignInFailed`, `RefreshTokenRejected`, `UnauthorizedBurst` |
| `accountId`, `userId` | When known, the same ids as the command log |
| `statusCode` | The response status |
| `route`, `method` | Route template and HTTP method |
| `ipAddress` | The client IP (from `X-Forwarded-For` as configured for Container Apps) |
| `userAgent` | Cut to 256 characters |
| `count` | For `UnauthorizedBurst` |
| `traceId` | Same as the command log |

- `AppendBlobSecurityEventStore` (`src/Infrastructure/Audit`) appends each batch as one block (`AppendBlockAsync`). When the day's blob doesn't exist yet (404) it creates it with `If-None-Match: *` and appends again, so the API never needs to read or overwrite a blob, which the writer role doesn't allow. An append blob takes up to 50,000 blocks, far more than a day of batches, and the per-IP cap keeps it that way.
- The IP address is personal data under the GDPR; security logs are a legitimate interest, which is why these entries have a short retention.

## Retention

| Log | Where | Kept | Why |
|---|---|---|---|
| Commands | Table `Commands{yyyyMM}` | 24 months | Enough to answer a business's questions about payments and fees of the previous year |
| Security events | Container `security-events`, a blob per day | 6 months | Enough to investigate an incident; IPs are not kept longer than needed |

- **Commands.** Table Storage has no expiry per row, and deleting rows one by one costs a write each. So each month has its own table (`Commands202610`) and a whole table is deleted when it gets old, which is one operation. The worker creates the table of the month on its first write of that month. It does **not** delete old tables: the API's identity can't delete anything ([Access](#access-private-and-append-only)). The operator deletes them with their own identity (`az storage table delete --auth-mode login`), from a runbook. The first table to delete is 24 months old, so the runbook is first needed at the end of 2028; a scheduled Container Apps job with its own identity can replace it if remembering becomes a problem.
- **Security events.** The container has a time-based retention policy of 183 days with protected appends: a blob can't be changed or deleted, by anyone, until 183 days after its last append. The lifecycle management rule this plan first proposed doesn't work: Azure's lifecycle delete action skips every blob in a container with an immutability policy, even after its retention ends (checked 2026-10-19 in the lifecycle management docs). So the owner deletes each month's blobs once they are more than 190 days old, from the [runbook](../../audit-log-storage.md#7-deleting-old-security-events), first in April 2027; a scheduled Container Apps job with its own identity can replace it, like for the command tables. Until then nothing expires on its own, and a missed month only keeps a few KB longer.

When a person asks to be erased, their ids stay in the command log (no personal values are stored there) and their security events go with their day's blob when it is deleted.

## Access: private and append only

The logs answer "who did what" after something went wrong, so whoever compromises the API, or steals its credentials, must not be able to read other businesses' logs through them or erase their own tracks.

### A storage account of its own

`classmanageraudit`, in France Central like the API, created on 2026-10-19. Its settings and commands are in the [runbook](../../audit-log-storage.md).

- **Separate from the photos account.** The photos account allows anonymous reads on its public container (shop photos) and the API reaches it with a connection string, which carries the account key: full access to everything, tables included. The logs account has neither.
- **No anonymous access** (`--allow-blob-public-access false`). Tables never have anonymous access; the setting covers the `security-events` container.
- **No account key** (`--allow-shared-key-access false`). Only Microsoft Entra identities with a role can reach the data, so there is no key that can leak. The API uses its system-assigned managed identity, the one it already uses for SQL ([pilot deployment](../../pilot-deployment.md)), through `DefaultAzureCredential` (new package `Azure.Identity`). The configuration holds only the service URIs, never a secret. Azurite in development and tests keeps its connection string.
- **Public endpoint, no private endpoint.** A private endpoint costs about USD 7 a month plus a virtual network for Container Apps. Without keys and anonymous access, the public endpoint only answers identities with a role.
- **LRS**, like the photos account. Geo-redundancy doubles the price and a lost region is not a risk worth paying for in an operational log.

### Roles: the API can only add

Built-in roles such as Storage Table Data Contributor allow updating and deleting. The API gets one custom role, `Audit Log Writer`, assigned on the logs account only:

| Permission | Why |
|---|---|
| `Microsoft.Storage/storageAccounts/tableServices/tables/write` | Create the table of the month. No `.../tables/delete`, and no `.../tables/read`: the writer never lists tables |
| Data action `Microsoft.Storage/storageAccounts/tableServices/tables/entities/add/action` | Insert rows. No `read`, `write` (upsert), `update/action` or `delete`, so a row can't be read, replaced or removed through the API |
| Data action `Microsoft.Storage/storageAccounts/blobServices/containers/blobs/add/action` | Create the day's blob (Put Blob of a blob that doesn't exist) and append to it (Append Block). No `.../blobs/write`, so a blob can't be overwritten even before the retention policy exists |

- Rows are inserted with `TableTransactionActionType.Add` (`AddEntity`), never upsert: the `RowKey` is new every time, and an upsert would need the update permission.
- The exact permission names were checked against Azure's list of storage operations, and the role is proved by hand with a temporary service principal that has only this role: inserting and appending work, while reading, updating and deleting a row or a blob, and listing or deleting tables, answer 403 ([runbook](../../audit-log-storage.md#3-run-the-setup-script)).
- The API can't create containers: the owner creates `security-events` once ([runbook](../../audit-log-storage.md#3-run-the-setup-script)).
- Reading the logs is a separate person's role: the owner gets Storage Table Data Reader and Storage Blob Data Reader on the account and reads with Azure Storage Explorer signed in with Entra.

### Immutable security events

Azure Storage only has immutable storage (WORM) for blobs, not for tables, which is why security events are append blobs:

- Container-level **time-based retention policy** of 183 days with **protected append writes** allowed (`--allow-protected-append-writes true`): new blocks can be appended, existing ones can't be changed, and the blob can't be deleted until the period ends.
- The policy starts **unlocked**, so it can be tested and changed. Once the writer has been proved in production it is **locked**: from then on the period can only be extended, never shortened or removed, not even by the subscription owner, and the account can't be deleted while it holds protected blobs. That is the reason the logs account is separate: the photos account stays free to change or be deleted.
- Azurite has no immutability policies, so integration tests cover the format and the writer, and the policy is set by the setup script and checked from the [runbook](../../audit-log-storage.md#the-retention-policy-of-security-events).

### What stays mutable

- **Command rows are protected from the API, not from the operator.** Whoever holds Owner on the subscription can delete a table. That is acceptable for an operational log; if commands ever need to be immutable too, they move to append blobs like security events, at the cost of filtering by business and date.
- **The in-memory channel** still loses the last seconds of entries in a crash (see [the trade-off](#why-azure-table-storage-and-not-sql)).

## Why a background service and not an Azure Function

The writer is a `BackgroundService` inside the API, like the [background tasks](../20261007-background-tasks/plan.md) worker.

- **A Function needs a hop.** The entries are in the API's memory. To reach a Function, the API would first write them to a Storage Queue, which costs one write per entry, the same as writing the row to Table Storage directly. The Function would then pay a second write. Double the operations, for the same result.
- **Cost.** Both are close to free at pilot scale: the Functions consumption plans include a monthly free grant of executions, and the background service runs in the container that is already paid for. The difference is not money, it is one more resource to deploy, configure, monitor and keep on a supported plan.
- **Scale to zero is not a problem for the writer.** Entries are only produced while the API handles requests, so the container is running exactly when there is something to write, and it drains the channel on shutdown.
- **No timed job in the API.** Old command tables and old security event days are deleted by the operator, who has the permission the API lacks (see [Retention](#retention)).

Move to a Function (fed by a queue) only if the API runs several replicas that should not each write, or if the log has to survive a crash of the API, which the in-memory channel can't promise. In that second case the background tasks described above are the better fit: they already exist and survive a crash.

## Reading it

Platform operators read both logs in the backoffice's **Registros** tab, through the backoffice's own identity with the reader roles ([backoffice plan](../20261017-backoffice/plan.md#steps)); the API's writer still can't read. Before the backoffice, only the owner read it, from Azure Storage Explorer signed in with Entra (there is no account key): commands filtered by `PartitionKey` (the business) and by `RowKey` range (dates), security events by downloading a day's blob. A screen in the app ("Actividad" for the business owner) can come later and reads one partition, newest first, which is the order the `RowKey` already gives; it needs a read role for a separate identity, never for the API's writer.

## Checking the log in tests

The e2e suite writes the audit log to Azurite and ends by checking it against a set of rules (no exceptions, no unexpected failures or security events, no personal data in payloads, every command with its business and user), so every e2e test is checked without doing anything: [audit log rules](../../e2e/running-e2e-tests.md#audit-log-rules).

Ideas for later, not planned yet:

- **Authorization between businesses, proved for every endpoint.** The log can't show what security let through: a request that reads another business's data looks like any successful request. An integration test seeds two businesses and calls every mapped endpoint with an id in its route as business A with business B's ids, expecting 404 or 403 and never 2xx; the same without a session and with a role that lacks the permission. It covers each new endpoint by itself, which is where these bugs appear. `Then_every_endpoint_requires_a_permission_or_is_anonymous` already checks the metadata; this checks the behavior.
- **OWASP ZAP against the API in CI, never production.** An API scan from the OpenAPI document (`MapOpenApi`), run with `docker run` (only `actions/*` are allowed), on demand or weekly rather than on every pull request. After the scan, the log proves detection works: the wrong passwords show as `SignInFailed`, the 401 sweep as `UnauthorizedBurst`, and no command ends with `Exception`.

## Steps

1. Rename soft delete to `DeletedAt`, add `DeletedByUserId` and make replaced records soft deleted, with the migration and the record columns test. One PR, no behavior change. **Built.**
2. `IAuditable` and `AuditSaveChangesInterceptor`, with the test that every tenant-owned entity implements it. **Built.**
3. The logs storage account (no anonymous access, no account key), the `Audit Log Writer` role for the API's managed identity, and a runbook with audit commands (**Azure setup done**: account, `security-events` container, role proved and assigned, see the [runbook](../../audit-log-storage.md)). Table client (`Azure.Data.Tables`) through `DefaultAzureCredential` with Azurite in the integration tests, `AuditLogWorker` with the monthly tables. **Built**, without producers yet:
   - `src/Infrastructure/Audit`: `CommandAuditEntry` (the row), `ICommandAuditService.Record` (queues an entry, never waits), `AuditLogChannel<CommandAuditEntry>` (bounded to 10,000 entries; when full, new entries are dropped rather than holding the request), `TableCommandAuditStore` (creates the table of the month once, then `Add` transactions of up to 100 rows per partition) and an unconfigured store that drops entries when neither `ConnectionStrings:AuditLog` nor `AuditLog:TableServiceUri` is set.
   - `src/Api/Audit/AuditLogWorker<CommandAuditEntry>`: writes the queue in batches of 100, logs and drops a batch that fails, and writes what is left when the API stops.
   - Nothing calls `Record` until step 4, which is where the payload is built with personal fields masked: no entry exists before the masking does.
   - The blob client for security events comes with step 5.
4. `CommandAuditBehavior`, the attributes, the payload serializer and its tests (secret never in the JSON, name rules). **Built.** Queries opt in with `[AuditedQuery]` (`ExportQuery`), with the exported row count in `RowCount`.
5. `SecurityEventMiddleware`, sign-in and refresh rejections, the plan-limit marker, the daily append blobs and the retention policy (unlocked). **Built**: rejections are recognized by endpoint metadata instead of an `ISecurityEventRecorder`, and old days are deleted from the runbook instead of a lifecycle rule (see [Retention](#retention)).
6. Lock the retention policy once security events have been written in production for a while and the runbook's audit commands pass.

Each step starts with its failing tests, as every change in the repository.
