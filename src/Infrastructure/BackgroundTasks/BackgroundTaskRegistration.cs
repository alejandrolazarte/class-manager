namespace ClassManager.Infrastructure.BackgroundTasks;

public sealed record BackgroundTaskRegistration(
    string TypeName,
    Type CommandType,
    Func<IServiceProvider, BackgroundTaskCommand, CancellationToken, Task> RunAsync);
