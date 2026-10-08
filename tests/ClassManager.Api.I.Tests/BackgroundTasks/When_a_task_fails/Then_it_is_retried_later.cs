using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Api.I.Tests.BackgroundTasks.When_a_task_fails;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_retried_later(ApiFixture fixture)
{
    private const int FirstAttempt = 1;

    [Fact]
    public async Task Then_it_is_retried_later_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var recipient = BackgroundTaskRequests.UniqueRecipient();
        fixture.ApiFactory.EmailTransport.FailDeliveriesTo(recipient);

        await fixture.EnqueueEmailAsync(business.Business.Id, recipient);

        var task = await fixture.WaitForTaskToAsync(recipient, candidate => candidate.LastError is not null);
        task.NextAttemptOn.ShouldBe(BusinessApiFactory.Now + BackgroundTask.FirstRetryDelay);
        task.Attempts.ShouldBe(FirstAttempt);
        task.LastError.ShouldBe(RecordingEmailTransport.FailedDeliveryMessage);
        task.FailedOn.ShouldBeNull();
    }
}
