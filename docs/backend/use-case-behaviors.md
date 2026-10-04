# Use case behaviors

Code that runs before and after every use case, such as opening a transaction, lives in a behavior instead of being repeated in each use case. Endpoints and use cases don't change: they keep injecting and implementing `IUseCase<TCommand, TResponse>`.

## How it works

- Every use case input is either a command (`ICommand`) or a query (`IQuery`). A command changes data, a query only reads it. A unit test fails if an input is neither or both.
- `src/Api/UseCaseServiceCollectionExtensions.cs` registers each use case with `AddUseCase<TCommand, TResponse, TUseCase>()`. Whoever asks for `IUseCase<TCommand, TResponse>` gets a `UseCasePipeline` that wraps the use case with its behaviors. An integration test fails if a use case is registered any other way.
- A behavior implements `IUseCaseBehavior<TCommand, TResponse>`. It receives the input and the next step: code before `nextStep()` runs before the use case and code after it runs after. A behavior can also return a failure without calling `nextStep()`, and then the use case doesn't run.
- Behaviors run in the order they are registered, the first one outermost. A behavior registered with open generic types applies to every use case whose input satisfies its constraints. One registered with closed types applies to that use case only.

```csharp
services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(CommandTransactionBehavior<,>));
services.AddScoped(typeof(IUseCaseBehavior<,>), typeof(QueryNoTrackingBehavior<,>));

services.AddUseCase<AddStudentCommand, StudentResponse, AddStudentUseCase>();
services.AddUseCase<GetStudentQuery, StudentSummaryResponse, GetStudentUseCase>();
```

## Commands run in a transaction

`CommandTransactionBehavior` applies to inputs that implement `ICommand`. It turns tracking on for `AppDbContext`, opens a transaction through `IUnitOfWork`, which also enlists `SecurityDbContext`, runs the use case and commits.

- **The transaction commits when the use case returns, success or failure.** Use cases already decide what to save, and some save on purpose before returning a failure: a wrong password increments the failed sign-in count that locks the account. Rolling back every failure would undo that.
- **It rolls back when the use case throws.** Nothing the use case saved before the exception is kept.
- **A use case can still open its own transaction** with `IUnitOfWork.BeginTransactionAsync` to make a block all or nothing. Inside a command, that transaction joins the command's transaction: committing it does nothing yet, and disposing it without committing rolls back the whole command when the command ends. This keeps the meaning those use cases had before the behavior existed.
- **A failed `SaveChanges` inside a command is undone on its own.** EF Core sets a savepoint before saving inside a transaction and rolls back to it on failure, so use cases that catch a unique constraint violation and read the winner keep working. SQL Server doesn't support savepoints with `MultipleActiveResultSets`, so keep it off in the connection string.
- **Emails and web push are queued when the use case sends them, not when the transaction commits.** If a command throws after sending, the message still goes out.
- Code outside `IUnitOfWork` that opens a transaction on `SecurityDbContext`, like the password reset service, must join the open transaction instead of starting another one: SQL Server doesn't allow two transactions on the same connection.

## Queries don't track

`QueryNoTrackingBehavior` applies to inputs that implement `IQuery`. It sets `AppDbContext` to `NoTracking`, so entities read by a query are not tracked and nothing a query loads can be saved by mistake. It doesn't open a transaction.

A query must never save. If a use case needs to write, its input is a command.

## Adding a behavior

1. Write its tests first, against the pipeline with fakes in `ClassManager.Core.U.Tests` or against real services in `ClassManager.Api.I.Tests` when it touches the database.
2. Implement `IUseCaseBehavior<TCommand, TResponse>`. Put it in `Core` if it only needs Core abstractions, otherwise in `Infrastructure` or `Api`. Use a generic constraint (`where TCommand : ICommand`) to limit it to commands or queries.
3. Register it in `AddUseCases`, in the position it should run.
