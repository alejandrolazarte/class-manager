using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Infrastructure.BackgroundTasks;
using ClassManager.Infrastructure.Email;
using ClassManager.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class BackgroundTaskRequests
{
    public const string EmailSubject = "Background task test";

    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);

    public static string UniqueRecipient() => $"task-{Guid.NewGuid():N}@example.com";

    public static SendEmailTask EmailTaskTo(string recipient, Guid businessId) =>
        new(new EmailMessage(recipient, EmailSubject, new EmailContent("Test", EmailSubject, "Intro", "Footer"), businessId));

    public static async Task EnqueueEmailAsync(this ApiFixture fixture, Guid businessId, string recipient)
    {
        await using var scope = fixture.ApiFactory.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ITenantScope>().Establish(businessId);
        await scope.ServiceProvider.GetRequiredService<IBackgroundTaskOutbox>()
            .EnqueueAsync(EmailTaskTo(recipient, businessId), CancellationToken.None);
    }

    public static async Task EnqueueEmailAndRollBackAsync(this ApiFixture fixture, Guid businessId, string recipient)
    {
        await using var scope = fixture.ApiFactory.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ITenantScope>().Establish(businessId);
        await using var transaction = await scope.ServiceProvider.GetRequiredService<IUnitOfWork>().BeginTransactionAsync(CancellationToken.None);
        await scope.ServiceProvider.GetRequiredService<IBackgroundTaskOutbox>()
            .EnqueueAsync(EmailTaskTo(recipient, businessId), CancellationToken.None);
    }

    public static async Task<BackgroundTask?> FindTaskToAsync(this ApiFixture fixture, string recipient)
    {
        await using var context = fixture.CreateDbContext(Guid.Empty);
        return await context.BackgroundTasks.AsNoTracking().SingleOrDefaultAsync(task => task.Payload.Contains(recipient));
    }

    public static async Task<BackgroundTask> WaitForTaskToAsync(this ApiFixture fixture, string recipient, Func<BackgroundTask, bool> matches)
    {
        var deadline = DateTimeOffset.UtcNow + WaitTimeout;
        while (DateTimeOffset.UtcNow < deadline)
        {
            if (await fixture.FindTaskToAsync(recipient) is { } task && matches(task))
            {
                return task;
            }

            await Task.Delay(PollInterval);
        }

        throw new TimeoutException($"No matching background task for {recipient}.");
    }

    public static async Task WaitUntilNoTaskToAsync(this ApiFixture fixture, string recipient)
    {
        var deadline = DateTimeOffset.UtcNow + WaitTimeout;
        while (DateTimeOffset.UtcNow < deadline)
        {
            if (await fixture.FindTaskToAsync(recipient) is null)
            {
                return;
            }

            await Task.Delay(PollInterval);
        }

        throw new TimeoutException($"The background task for {recipient} is still stored.");
    }

    public static async Task MakeLastAttemptDueAsync(this ApiFixture fixture, Guid taskId)
    {
        await using (var context = fixture.CreateDbContext(Guid.Empty))
        {
            await context.BackgroundTasks
                .Where(task => task.Id == taskId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(task => task.Attempts, BackgroundTaskRunner.MaxAttempts - 1)
                    .SetProperty(task => task.NextAttemptOn, BusinessApiFactory.Now));
        }

        fixture.ApiFactory.Services.GetRequiredService<BackgroundTaskSignal>().Ring();
    }
}
