# Audit log

Status: proposal.

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

Soft delete breaks this. `ISoftDeletable.DeletedOn` is an event (a user deleted the row at that moment), but it has the same name as `IDeletedOn.DeletedOn` (another record replaced it). Today the test `Then_it_implements_the_record_interfaces` accepts either interface for a `DeletedOn` column, so a column named `DeletedOn` doesn't say which of the two it means, and `ISoftDeletable` has no field for who deleted the row.

**Proposal: rename the soft delete column to `DeletedAt` and add `DeletedByUserId`.**

| Interface | Before | After | Meaning |
|---|---|---|---|
| `IDeletedOn` (records) | `DeletedOn` | `DeletedOn` (unchanged) | Another record replaced this one. `null` = current |
| `ISoftDeletable` | `IsDeleted`, `DeletedOn` | `IsDeleted`, `DeletedAt`, `DeletedByUserId` | A user deleted the row. Hidden by the `SoftDelete` filter |
| `IAudited` (new) | — | `CreatedAt`, `CreatedByUserId`, `UpdatedAt`, `UpdatedByUserId` | Who created and who last changed the row |

After the change, the column name alone says what it is: `DeletedOn` only on replaced records, `DeletedAt` only on soft deleted rows. The record columns test maps `DeletedOn` to `IDeletedOn` only and adds `DeletedAt` → `ISoftDeletable`.

What the rename touches (11 entities: `BusinessMember`, `OrganizationMember`, `Payment`, `ClientBillingPlanChange`, `DefaultMonthlyFeeChange`, `MeasurementRecord`, `MeasurementDefinition`, `CustomRole`, `ClassPackPurchase`, `Announcement`, `ClassFeedback`):

- `ISoftDeletable`: `DeletedOn` → `DeletedAt`, `Delete(DateTimeOffset deletedOn)` → `Delete(DateTimeOffset deletedAt)`.
- `SoftDeleteModelBuilderExtensions`: the check constraint becomes `CK_<Table>_IsDeleted_DeletedAt` with `[DeletedAt]`. Filtered indexes use `[IsDeleted] = 0`, so they don't change.
- One migration: `RenameColumn` (`sp_rename`, instant, no data copy), drop and create each check constraint, add `DeletedByUserId` (nullable, `null` for rows deleted before the change).
- Responses or exports that expose `DeletedOn` of a soft deleted row, and [deleting data](../../deleting-data.md), `CLAUDE.md` and the records section of the docs.

Alternative considered: keep soft delete as `DeletedOn` and rename `IDeletedOn.DeletedOn` to `ReplacedOn`. It touches only 2 entities (`Subscription`, `SubscriptionFeature`), but it leaves soft delete outside the `*At` convention that every other event uses, and the records convention (`CreatedOn`, `DeletedOn`, `ExpiredOn`) is already documented and used by the subscriptions library. Rejected.

### `IAudited`

```csharp
namespace ClassManager.Records;

public interface IAudited
{
    DateTimeOffset CreatedAt { get; }
    Guid? CreatedByUserId { get; }
    DateTimeOffset? UpdatedAt { get; }
    Guid? UpdatedByUserId { get; }
}
```

- Lives in `src/Records` (no dependencies).
- `AuditSaveChangesInterceptor` in `src/Infrastructure/Persistence` fills them, the same way `TenantStampingSaveChangesInterceptor` fills `TenantId`:
  - `Added`: `CreatedAt` = now (from `TimeProvider`) when the entity didn't set it, `CreatedByUserId` = current user.
  - `Modified`: `UpdatedAt` = now, `UpdatedByUserId` = current user.
  - Soft deleted (`SoftDeleteSaveChangesInterceptor` turns the remove into `Delete(now)`): `DeletedByUserId` = current user. The audit interceptor runs after the soft delete one.
- The current user comes from the `sub` claim through a small `ICurrentUser` in `Core`. Work with no user (the background worker, sign-up before the account exists, seeding) leaves the `ByUserId` columns `null`.
- Entities that already set `CreatedAt` in their constructor keep doing it; the interceptor doesn't overwrite it. `ClassFeedback.UpdatedAt` and `BrandLogo.UpdatedAt` become the `IAudited` ones.
- Business columns with their own meaning stay: `RecordedByUserId` (who took the payment), `CancelledByUserId`, `InvitedByUserId`. `UpdatedByUserId` says who last touched the row, not who did a business action.
- A test fails if a tenant-owned entity doesn't implement `IAudited`, like the record columns test.

## 2. Command log

### The behavior

`CommandAuditBehavior<TCommand, TResponse>` applies to inputs that implement `ICommand` (`where TCommand : ICommand`) and is registered **first**, outside `CommandTransactionBehavior`, so it sees the result after the commit and also sees exceptions:

```csharp
services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(CommandAuditBehavior<,>));
services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(CommandTransactionBehavior<,>));
services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(QueryNoTrackingBehavior<,>));
```

- It measures the duration, runs `nextStep()`, builds the audit entry and **queues** it in an in-memory `Channel<CommandAuditEntry>`. It never waits for Table Storage.
- `AuditWorker` (a `BackgroundService`) reads the channel and writes in batches (`SubmitTransactionAsync`, up to 100 rows of the same partition per batch). If Table Storage fails it logs a warning and drops the batch: the command already succeeded.
- On shutdown the worker drains the channel. With `minReplicas 0` the container stops after five minutes without requests, which gives it time; a crash loses what was in memory (seconds of entries).
- If the use case throws, the behavior writes the entry with `Outcome = Exception` and the exception type, and rethrows.
- Queries are not logged. A few queries that read a lot of personal data (exports of students) can opt in with `[AuditedQuery]` on the query type; a second behavior for `IQuery` logs only those.

### Row

Table `Commands{yyyyMM}` (one table per month, see [Retention](#retention)).

| Property | Value |
|---|---|
| `PartitionKey` | `TenantId` (`none` for commands without a tenant, such as sign-up) |
| `RowKey` | Inverted ticks + `_` + a new `Guid` (newest first, unique) |
| `Command` | `typeof(TCommand).Name`, for example `RecordPaymentCommand` |
| `UserId` | `sub` claim, or empty |
| `Outcome` | `Succeeded`, `Failed` or `Exception` |
| `ErrorKind`, `ErrorCode` | From `Result` when it failed, or the exception type |
| `DurationMs` | Duration of the use case with its transaction |
| `TraceId` | `Activity.Current.TraceId`, the same one as in the logs and in security events |
| `Payload` | The command as JSON, after removing sensitive fields (below) |
| `PayloadTruncated` | `true` when the JSON was cut at 32 KB |

### Payload: yes, but without sensitive fields

The payload is what makes the log useful ("changed the fee of client X to 25,000"), so it is stored. It goes through a serializer that removes fields by category, decided per property with an attribute on the command:

| Category | Examples | What is stored |
|---|---|---|
| Secret | Passwords, refresh tokens, reset and confirmation codes, VAPID keys, payment tokens | Never stored: the property is left out |
| Personal | Names, email, phone, birth date, address, measurements, guardian data, free text written about a student | `"[personal]"`. The ids in the command (`StudentId`, `ClientId`) are kept, so the row still says who was affected |
| File | Uploaded files, streams, `byte[]` | Only name, content type and size |
| Everything else | Ids, amounts, dates, enums, flags | As is |

```csharp
public sealed record ChangePasswordCommand(
    [property: AuditSecret] string CurrentPassword,
    [property: AuditSecret] string NewPassword) : ICommand;

public sealed record AddStudentCommand(
    Guid ClientId,
    [property: AuditPersonal] string FirstName,
    [property: AuditPersonal] string LastName,
    [property: AuditPersonal] DateOnly BirthDate,
    Guid ClassGroupId) : ICommand;
```

Why personal data is masked too, not only secrets:

- Many students are minors ([ages and minors](../../ages-and-minors.md)), and businesses in Spain fall under the GDPR. A copy of every name and birth date in a log that can't be edited is a second database of personal data that has to be erased when a person asks, and Table Storage rows are hard to find by content. Keeping ids makes the log point to the person without copying them.
- The current value is in SQL; the log only needs to say which row changed and what non-personal values were set.

How it stays correct as commands are added (attributes alone would be forgotten):

- **A test scans every `ICommand`**: a property whose name contains `Password`, `Token`, `Secret`, `Code` or `Key` must have `[AuditSecret]`, and a property named like a person's data (`Name`, `Email`, `Phone`, `BirthDate`, `Address`, `Note`, `Comment`) must have `[AuditPersonal]` or `[AuditPlain]` (explicitly not personal, for example `PlanName`). Nested records are scanned too.
- **A test serializes a sample of every command** and fails if a value marked secret appears in the JSON.
- `IFormFile`, `Stream` and `byte[]` are recognized by type; no attribute needed.

Alternative considered: an allowlist (only properties marked `[Audited]` are stored). Safer by default, but every command has to be annotated before the log says anything, and a forgotten attribute silently hides data. The denylist with the name test fails the build instead.

## 3. Security events

### What is recorded

| Event | Source | Why |
|---|---|---|
| `Forbidden` | Any 403 that is not a plan limit | An authenticated user tried something their role doesn't allow. Rare and worth reading |
| `SignInFailed` | Sign-in use case (wrong password, locked account) | Password guessing |
| `RefreshTokenRejected` | Refresh use case (unknown, expired, reused) | A reused refresh token can mean a stolen one |
| `UnauthorizedBurst` | Middleware, 401 counted per IP | Many 401 from the same IP in a minute: one row with the count, not one row per 401 |

Not recorded:

- **A single 401.** Every expired access token returns 401 before the app refreshes it, so most 401s are normal traffic. Worse, anyone can produce 401s without an account: one row per 401 lets a bot fill the table and the bill.
- **403 for plan limits** (`FeatureNotInPlan`, `SubscriptionInactive` from `PlanAwareAuthorizationResultHandler`). They are a commercial limit, not an attack. The handler marks the request (`HttpContext.Items`) and the middleware skips it.

### The middleware

- `SecurityEventMiddleware` runs **before** `UseAuthentication` and `UseAuthorization`, calls `await next()`, then reads `Response.StatusCode`. Placed after them it would not see the 401 and 403 they return.
- A 403 is queued as `Forbidden` with the route template (`/api/students/{id}`, not the real path), the method, the user and the tenant.
- A 401 only increments an in-memory counter per IP. A timer writes one `UnauthorizedBurst` row for every IP that went over a threshold (for example 20 in a minute) and resets the counters.
- Each IP has a cap of rows per minute; beyond it events are counted, not written. Nothing a client sends can make the API write without limit.
- `SignInFailed` and `RefreshTokenRejected` are queued by the use cases themselves through `ISecurityEventRecorder`, because the middleware can't tell a wrong password from an expired token.
- The sign-in, refresh and password reset endpoints keep their rate limiting (`UseRateLimiter`); the events don't replace it.

### Entry

One append blob per day in the container `security-events`: `{yyyy}/{MM}/{dd}.jsonl`, one JSON object per line. Blobs and not a table because only blobs can be immutable in Azure Storage ([Access](#access-private-and-append-only)). There are few events a day, so reading a whole day is cheap.

| Field | Value |
|---|---|
| `occurredAt` | Date and time, UTC |
| `tenantId` | When known, otherwise empty |
| `event` | `Forbidden`, `SignInFailed`, `RefreshTokenRejected`, `UnauthorizedBurst` |
| `userId` | When known |
| `route`, `method` | Route template and HTTP method |
| `ipAddress` | The client IP (from `X-Forwarded-For` as configured for Container Apps) |
| `userAgent` | Cut to 256 characters |
| `count` | For `UnauthorizedBurst` |
| `traceId` | Same as the command log |

- The worker appends each batch as one block (`AppendBlockAsync`), creating the day's blob on its first write of the day. An append blob takes up to 50,000 blocks, far more than a day of batches, and the per-IP cap keeps it that way.
- The IP address is personal data under the GDPR; security logs are a legitimate interest, which is why these entries have a short retention.

## Retention

| Log | Where | Kept | Why |
|---|---|---|---|
| Commands | Table `Commands{yyyyMM}` | 24 months | Enough to answer a business's questions about payments and fees of the previous year |
| Security events | Container `security-events`, a blob per day | 6 months | Enough to investigate an incident; IPs are not kept longer than needed |

- **Commands.** Table Storage has no expiry per row, and deleting rows one by one costs a write each. So each month has its own table (`Commands202610`) and a whole table is deleted when it gets old, which is one operation. The worker creates the table of the month on its first write of that month. It does **not** delete old tables: the API's identity can't delete anything ([Access](#access-private-and-append-only)). The operator deletes them with their own identity (`az storage table delete --auth-mode login`), from a runbook. The first table to delete is 24 months old, so the runbook is first needed at the end of 2028; a scheduled Container Apps job with its own identity can replace it if remembering becomes a problem.
- **Security events.** The container has a time-based retention policy of 183 days: a blob can't be changed or deleted, by anyone, until 183 days after it was created. A lifecycle management rule deletes blobs older than 190 days in `security-events/`, so nothing needs to run in the API or by hand.

When a person asks to be erased, their ids stay in the command log (no personal values are stored there) and their security events expire with their day's blob.

## Access: private and append only

The logs answer "who did what" after something went wrong, so whoever compromises the API, or steals its credentials, must not be able to read other businesses' logs through them or erase their own tracks.

### A storage account of its own

- **Separate from the photos account.** The photos account allows anonymous reads on its public container (shop photos) and the API reaches it with a connection string, which carries the account key: full access to everything, tables included. The logs account has neither.
- **No anonymous access** (`--allow-blob-public-access false`). Tables never have anonymous access; the setting covers the `security-events` container.
- **No account key** (`--allow-shared-key-access false`). Only Microsoft Entra identities with a role can reach the data, so there is no key that can leak. The API uses its system-assigned managed identity, the one it already uses for SQL ([pilot deployment](../../pilot-deployment.md)), through `DefaultAzureCredential` (new package `Azure.Identity`). The configuration holds only the service URIs, never a secret. Azurite in development and tests keeps its connection string.
- **Public endpoint, no private endpoint.** A private endpoint costs about USD 7 a month plus a virtual network for Container Apps. Without keys and anonymous access, the public endpoint only answers identities with a role.
- **LRS**, like the photos account. Geo-redundancy doubles the price and a lost region is not a risk worth paying for in an operational log.

### Roles: the API can only add

Built-in roles such as Storage Table Data Contributor allow updating and deleting. The API gets one custom role, `Audit Log Writer`, assigned on the logs account only:

| Permission | Why |
|---|---|
| `Microsoft.Storage/storageAccounts/tableServices/tables/read`, `.../tables/write` | Create the table of the month. No `.../tables/delete` |
| Data action `Microsoft.Storage/storageAccounts/tableServices/tables/entities/add/action` | Insert rows. No `read`, `write` (upsert), `update/action` or `delete`, so a row can't be read, replaced or removed through the API |
| Data actions `Microsoft.Storage/storageAccounts/blobServices/containers/blobs/write`, `.../blobs/add/action` | Create the day's blob and append to it. Overwriting or deleting a blob under the retention policy is refused anyway |

- Rows are inserted with `TableTransactionActionType.Add` (`AddEntity`), never upsert: the `RowKey` is new every time, and an upsert would need the update permission.
- The exact permission names are checked against Azure's list of storage operations when the role is created, and the role is proved by hand: inserting works, while reading, updating and deleting a row, and deleting a table, answer 403.
- Reading the logs is a separate person's role: the owner gets Storage Table Data Reader and Storage Blob Data Reader on the account and reads with Azure Storage Explorer signed in with Entra.

### Immutable security events

Azure Storage only has immutable storage (WORM) for blobs, not for tables, which is why security events are append blobs:

- Container-level **time-based retention policy** of 183 days with **protected append writes** allowed (`--allow-protected-append-writes true`): new blocks can be appended, existing ones can't be changed, and the blob can't be deleted until the period ends.
- The policy starts **unlocked**, so it can be tested and changed. Once the writer has been proved in production it is **locked**: from then on the period can only be extended, never shortened or removed, not even by the subscription owner, and the account can't be deleted while it holds protected blobs. That is the reason the logs account is separate: the photos account stays free to change or be deleted.
- Azurite has no immutability policies, so integration tests cover the format and the writer, and the policy is checked with the audit commands of the runbook.

### What stays mutable

- **Command rows are protected from the API, not from the operator.** Whoever holds Owner on the subscription can delete a table. That is acceptable for an operational log; if commands ever need to be immutable too, they move to append blobs like security events, at the cost of filtering by business and date.
- **The in-memory channel** still loses the last seconds of entries in a crash (see [the trade-off](#why-azure-table-storage-and-not-sql)).

## Why a background service and not an Azure Function

The writer is a `BackgroundService` inside the API, like the [background tasks](../20261007-background-tasks/plan.md) worker.

- **A Function needs a hop.** The entries are in the API's memory. To reach a Function, the API would first write them to a Storage Queue, which costs one write per entry, the same as writing the row to Table Storage directly. The Function would then pay a second write. Double the operations, for the same result.
- **Cost.** Both are close to free at pilot scale: the Functions consumption plans include a monthly free grant of executions, and the background service runs in the container that is already paid for. The difference is not money, it is one more resource to deploy, configure, monitor and keep on a supported plan.
- **Scale to zero is not a problem for the writer.** Entries are only produced while the API handles requests, so the container is running exactly when there is something to write, and it drains the channel on shutdown.
- **No timed job.** Security events expire through the retention policy and a lifecycle rule; old command tables are deleted by the operator, who has the permission the API lacks (see [Retention](#retention)).

Move to a Function (fed by a queue) only if the API runs several replicas that should not each write, or if the log has to survive a crash of the API, which the in-memory channel can't promise. In that second case the background tasks described above are the better fit: they already exist and survive a crash.

## Reading it

At first, only the owner reads it, from Azure Storage Explorer signed in with Entra (there is no account key): commands filtered by `PartitionKey` (the business) and by `RowKey` range (dates), security events by downloading a day's blob. A screen in the app ("Actividad" for the business owner) can come later and reads one partition, newest first, which is the order the `RowKey` already gives; it needs a read role for a separate identity, never for the API's writer.

## Steps

1. Rename soft delete to `DeletedAt` and add `DeletedByUserId`, with the migration and the record columns test. One PR, no behavior change.
2. `IAudited`, `ICurrentUser` and `AuditSaveChangesInterceptor`, with the test that every tenant-owned entity implements it.
3. The logs storage account (no anonymous access, no account key), the `Audit Log Writer` role for the API's managed identity, and a runbook with audit commands. Table client (`Azure.Data.Tables`) and blob client through `DefaultAzureCredential` with Azurite in the integration tests, `AuditWorker` with the monthly tables.
4. `CommandAuditBehavior`, the attributes, the payload serializer and its tests (secret never in the JSON, name rules).
5. `SecurityEventMiddleware`, `ISecurityEventRecorder` in sign-in and refresh, the plan-limit marker, the daily append blobs, the retention policy (unlocked) and the lifecycle rule.
6. Lock the retention policy once security events have been written in production for a while and the runbook's audit commands pass.

Each step starts with its failing tests, as every change in the repository.
