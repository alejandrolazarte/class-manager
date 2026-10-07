using ClassManager.Infrastructure.BackgroundTasks;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.BackgroundTasks.When_a_task_type_is_not_registered;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_marked_failed_at_once(ApiFixture fixture)
{
    private const string RenamedTypeName = "RenamedBackgroundTaskCommand";
    private const int FirstAttempt = 1;

    [Fact]
    public async Task Then_it_is_marked_failed_at_once_Run()
    {
        var recipient = BackgroundTaskRequests.UniqueRecipient();
        var payload = $$$"""{"message":{"to":"{{{recipient}}}"}}""";
        await using (var context = fixture.CreateDbContext(Guid.Empty))
        {
            context.BackgroundTasks.Add(BackgroundTask.Create(RenamedTypeName, payload, null, BusinessApiFactory.Now));
            await context.SaveChangesAsync();
        }

        fixture.ApiFactory.Services.GetRequiredService<BackgroundTaskSignal>().Ring();

        var task = await fixture.WaitForTaskToAsync(recipient, candidate => candidate.FailedOn is not null);
        task.Attempts.ShouldBe(FirstAttempt);
        task.LastError.ShouldBe(BackgroundTaskTypes.UnknownTypeMessage + RenamedTypeName);
    }
}
