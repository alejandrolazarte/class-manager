# Background tasks stored in the database

Status: built.

## Why

Order emails and every push notification go through an in-memory outbox (`EmailOutbox`, `PushOutbox`) read by two background workers (`EmailWorker`, `PushWorker`). The API runs on Azure Container Apps with `minReplicas 0`, so the container stops five minutes after the last request, and a deploy or a crash also restarts it. Whatever was still in memory is lost, and a failed send is only logged: nothing tries again.

Microsoft's guidance for this case is the transactional outbox ([Azure Architecture Center](https://learn.microsoft.com/azure/architecture/databases/guide/transactional-out-box-cosmos#solution)): save the message in the same database transaction as the business change, and let a worker send it and mark it done. The queued background task sample in the ASP.NET Core docs ([hosted services](https://learn.microsoft.com/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-10.0#queued-background-tasks)) uses an in-memory `Channel<T>`, which is what the app has today.

Invitation and password emails stay as they are: they are sent within the request, so the screen can tell the user whether the email went out ([notifications](../../notifications.md#when-emails-are-sent)).

## Why not Hangfire or a queue service

- **Hangfire**: its SQL Server storage polls the database and writes server heartbeats on a timer. The pilot database is Azure SQL serverless on the free offer: it only pauses when it has no sessions and no CPU for the whole auto-pause delay ([auto-pause](https://learn.microsoft.com/azure/azure-sql/database/serverless-tier-auto-pause-resume?view=azuresql#auto-pause)), and every minute it stays online uses the free allowance. A timer that queries more often than the delay keeps the database online.
- **Azure Storage Queues or Service Bus**: another service to deploy and pay for, and a message can't be written in the same transaction as the order. The task table can.

What we need is small: a handful of task types, one replica, retries. A table and one worker cover it.

## Design

### Tasks

- `BackgroundTaskCommand` is the base record of every task. It carries the `TenantId` (nullable, for tasks that don't belong to a business). Each task is a record that inherits from it:
  - `SendEmailBackgroundTaskCommand(EmailMessage Message)`: an order email.
  - `SendPushBackgroundTaskCommand(PushJob Push)`: a push to students (`StudentAppPush`) or to the team (`TeamPush`).
- Each task has a handler, `IBackgroundTaskHandler<TCommand>`, resolved from DI in its own scope.
- Each task type is registered once, `services.AddBackgroundTask<SendEmailBackgroundTaskCommand, SendEmailBackgroundTaskHandler>()`, and stored under its class name (`SendEmailBackgroundTaskCommand`). No string to keep in sync: the registration reads `typeof(TCommand).Name` (`nameof(TCommand)` inside the generic method would give the literal `"TCommand"`). The registration keeps a typed delegate to call the handler, so the worker doesn't use reflection.
- **Renaming a task class** changes the stored name. Tasks only live in the table until they run (minutes, or about 85 minutes with every retry), so the only tasks at risk are the ones still pending when the rename is deployed: the new code doesn't know their old name, they are marked failed at once, without retries that could never work, with `Background task type is not registered: <name>` as their error and an error in the log. The row keeps its payload, so it can be run again by updating its `Type` to the new name and clearing `FailedOn`. Rename when the table has no pending task of that type, or keep the old class until they drain; if renames become common, a former-name attribute on the class can map the old name (what Hangfire solves with a custom type resolver).

### The `BackgroundTasks` table

| Column | Meaning |
|---|---|
| `Id` | Task id |
| `Type` | The class name of the command (`SendEmailBackgroundTaskCommand`, `SendPushBackgroundTaskCommand`) |
| `Payload` | The command serialized as JSON |
| `TenantId` | The business the task runs for, or `null` |
| `Attempts` | How many times the worker took it |
| `NextAttemptOn` | When it may run next |
| `EnqueuedOn` | When it was queued |
| `LastError` | The last exception message, shortened |
| `FailedOn` | When it ran out of attempts, or `null` |

- It is an infrastructure table, **not tenant-owned**: the worker reads tasks of every business, so the tenant query filter would hide them. The task establishes its tenant (`ITenantScope.Establish`) before its handler runs, as the in-memory workers did.
- `EnqueuedOn` instead of `CreatedOn`: a `CreatedOn` column means the [record conventions](../../../CLAUDE.md#records-that-change-over-time), and a task is not a record that gets replaced.
- A task that ran is deleted. Rows with no value once used are deleted physically ([deleting data](../../deleting-data.md)).
- A filtered index on `NextAttemptOn` where `FailedOn IS NULL` serves the worker's two queries.

### Queuing

`IBackgroundTaskOutbox.EnqueueAsync(command)` adds the row and saves the `AppDbContext`, like `TeamNotifier` already does after the business change. Inside a use case, that save joins the transaction of `CommandTransactionBehavior`, so the task commits or rolls back with the order. If the transaction rolls back after queuing (an exception, or a step that abandons its transaction), nothing is sent, where the in-memory outbox would have sent it anyway.

### Waking the worker: a doorbell, not a timer

The worker never queries the database on a fixed interval. It sleeps until one of two things happens:

1. **New work.** `BackgroundTaskSignal` is a singleton with a `Channel<bool>` of capacity 1 (extra rings are dropped, so ten orders wake the worker once). `BackgroundTaskSignalInterceptor`, a scoped EF Core interceptor, notes when a save adds a task, and rings **after the commit**: on `TransactionCommitted` when the save was inside a transaction, or right after `SavedChanges` when it wasn't. Ringing before the commit would wake the worker before the row is visible.
2. **A retry is due.** After a pass, the worker reads the earliest `NextAttemptOn` of the pending tasks and waits until then. With nothing pending it only waits for the doorbell.

With no tasks, the worker sends no queries at all. When the container is stopped, queued tasks stay in the table and run on the next start, because the worker does a pass when it starts.

The doorbell only reaches the worker of the same process. With one replica that is always the one that queued the task. With several, the task runs when that replica's worker wakes, or on the next start.

### Running a task

For each due task (in batches of 20, oldest first):

1. **Claim** it with one `ExecuteUpdate` that only matches the task if it is still due: it adds one to `Attempts` and moves `NextAttemptOn` five minutes ahead. If no row changed, another worker took it (two replicas overlap briefly during a deploy).
2. Run the handler in a new scope, with the task's tenant established.
3. **Success**: delete the row.
4. **Failure**: save the error and wait 1, 4, 16 and 64 minutes before attempts 2 to 5. After the fifth failed attempt, set `FailedOn` and log an error. Failed tasks stay in the table to be reviewed.

A process that dies in the middle of a task leaves it claimed; it runs again once the five minutes pass. If the database itself can't be reached, the worker logs the error and tries again after a minute while the container is up.

### Delivery guarantee

At least once. If the process dies after the email left but before the row was deleted, the email is sent again. A push that fails halfway is retried as a whole, so some devices may receive it twice. Both are acceptable for notices; a task whose duplicate would do harm needs an idempotent handler.

## What changes

- New: `src/Infrastructure/BackgroundTasks` (command base, handler interface, registration, outbox, signal, interceptor, runner, entity) and its EF configuration and migration (`AddBackgroundTasks`).
- New: `BackgroundTaskWorker` in `src/Api/BackgroundTasks`, replacing `EmailWorker` and `PushWorker`.
- `OrderNotificationService` and `PushPublisher` queue tasks instead of writing to the in-memory outboxes. `PushPublisher` becomes async because queuing saves.
- Removed: `EmailOutbox`, `PushOutbox`, `EmailWorker`, `PushWorker`.

## Tests

Integration tests (`tests/ClassManager.Api.I.Tests/BackgroundTasks`), against SQL Server, with the worker running as in production:

- A queued task runs and its row is deleted.
- A task queued in a transaction that rolls back is never stored, and its email is never sent.
- A task whose handler fails stays queued with one attempt, the error and its next attempt one minute later.
- A task that fails its last attempt is marked failed.
- A task whose type is not registered (renamed or removed) is marked failed at once.
- A task is stored under its command class name.
- The doorbell: a ring before the wait ends the wait at once, and a wait with a due time ends at that time.

The existing email and push tests (orders, team notifications, student app) keep passing unchanged: they now go through the table.

## Later

- Delete failed tasks after a while, or show them somewhere, once the pilot shows how many there are.
- Move invitation and password emails to tasks if the screens stop needing to know the result.
- `ExpiredOrderCancellationWorker` runs every hour on a timer. With `minReplicas 0` it only runs while the container is up, but with `minReplicas 1` it would keep the database from pausing if the auto-pause delay is an hour or more. Move it to a due time stored in the database before changing `minReplicas`.
- Extract the engine to its own library (like `Notifications`) if another app needs it.
