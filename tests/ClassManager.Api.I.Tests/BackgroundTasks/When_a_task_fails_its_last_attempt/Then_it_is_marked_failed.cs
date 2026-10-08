using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Api.I.Tests.BackgroundTasks.When_a_task_fails_its_last_attempt;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_marked_failed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_marked_failed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var recipient = BackgroundTaskRequests.UniqueRecipient();
        fixture.ApiFactory.EmailTransport.FailDeliveriesTo(recipient);
        await fixture.EnqueueEmailAsync(business.Business.Id, recipient);
        var firstFailure = await fixture.WaitForTaskToAsync(recipient, candidate => candidate.LastError is not null);

        await fixture.MakeLastAttemptDueAsync(firstFailure.Id);

        var task = await fixture.WaitForTaskToAsync(recipient, candidate => candidate.FailedOn is not null);
        task.FailedOn.ShouldBe(BusinessApiFactory.Now);
        task.Attempts.ShouldBe(BackgroundTaskRunner.MaxAttempts);
    }
}
