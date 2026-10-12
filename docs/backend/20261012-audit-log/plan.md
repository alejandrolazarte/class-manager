# Audit log

Status: proposal.

Three pieces answer "who did what, and who tried what they were not allowed to":

1. **Audit columns** on every table: who created and who last changed a row, and who deleted it when it is soft deleted.
2. **Command log**: every command (`ICommand`) that ran, by whom, with its input after removing sensitive fields, written by a use case behavior to Azure Table Storage.
3. **Security events**: 403 responses and the 401 that matter (failed sign-ins, invalid refresh tokens), written by a middleware to Azure Table Storage.

The history lives in the command log, not in SQL. The columns only keep the latest state.

## Why Azure Table Storage and not SQL

| Option | Cost at pilot scale | Notes |
|---|---|---|
| Azure Table Storage (LRS) | Cents per month | About USD 0.045 per GB per month and about USD 0.00036 per 10,000 writes. 10 businesses × 2,000 commands a day ≈ 600,000 rows a month ≈ 0.6 GB: well under USD 0.10 a month |
| A table in Azure SQL | Uses the free offer | Every write keeps the serverless database busy and uses its 100,000 vCore-seconds and its 32 GB ([hosting plan](../../hosting-plan.md)). An audit row on every command roughly doubles the writes |
| Logs in Log Analytics | About USD 2–3 per GB ingested | Good for searching, expensive for a permanent record |

Prices are approximate (checked 2026-10-12). Check the Azure Table Storage pricing page before relying on them.

The account is the same storage account as Blob Storage ([storage](../../storage.md)), so there is nothing new to deploy except the tables. In development and tests it runs on Azurite, which already runs for blob tests.

The trade-off: the command log is written outside the SQL transaction. If the process dies between the commit and the write, the row is lost; if the write fails, the command still succeeded. That is acceptable for an operational audit. If an audit with legal weight is ever needed, write the row to an outbox table in the same transaction (as the [background tasks](../20261007-background-tasks/plan.md) do) and copy it to Table Storage from the worker.

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

### Row

Table `SecurityEvents{yyyyMM}`.

| Property | Value |
|---|---|
| `PartitionKey` | `TenantId`, or `anonymous` |
| `RowKey` | Inverted ticks + `_` + `Guid` |
| `Event` | `Forbidden`, `SignInFailed`, `RefreshTokenRejected`, `UnauthorizedBurst` |
| `UserId` | When known |
| `Route`, `Method` | Route template and HTTP method |
| `IpAddress` | The client IP (from `X-Forwarded-For` as configured for Container Apps) |
| `UserAgent` | Cut to 256 characters |
| `Count` | For `UnauthorizedBurst` |
| `TraceId` | Same as the command log |

The IP address is personal data under the GDPR; security logs are a legitimate interest, which is why these tables have a short retention.

## Retention

Table Storage has no expiry per row, and deleting rows one by one costs a write each. So each month has its own table (`Commands202610`, `SecurityEvents202610`) and a whole table is deleted when it gets old, which is one operation.

| Table | Kept | Why |
|---|---|---|
| `Commands{yyyyMM}` | 24 months | Enough to answer a business's questions about payments and fees of the previous year |
| `SecurityEvents{yyyyMM}` | 6 months | Enough to investigate an incident; IPs are not kept longer than needed |

The worker creates the table of the month on its first write and, once a day, deletes the tables older than the retention. When a person asks to be erased, their ids stay in the command log (no personal values are stored there) and their rows in security events expire with the table.

## Reading it

At first, only the owner reads it, from Azure Storage Explorer or the portal, filtering by `PartitionKey` (the business) and by `RowKey` range (dates). A screen in the app ("Actividad" for the business owner) can come later and reads one partition, newest first, which is the order the `RowKey` already gives.

## Steps

1. Rename soft delete to `DeletedAt` and add `DeletedByUserId`, with the migration and the record columns test. One PR, no behavior change.
2. `IAudited`, `ICurrentUser` and `AuditSaveChangesInterceptor`, with the test that every tenant-owned entity implements it.
3. Table Storage client (`Azure.Data.Tables`) next to the blob client, Azurite in the integration tests, `AuditWorker` with the monthly tables and retention.
4. `CommandAuditBehavior`, the attributes, the payload serializer and its tests (secret never in the JSON, name rules).
5. `SecurityEventMiddleware`, `ISecurityEventRecorder` in sign-in and refresh, the plan-limit marker.

Each step starts with its failing tests, as every change in the repository.
