namespace ClassManager.Api.I.Tests.BackgroundTasks.When_a_task_is_queued;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_handler_runs(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_handler_runs_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var recipient = BackgroundTaskRequests.UniqueRecipient();

        await fixture.EnqueueEmailAsync(business.Business.Id, recipient);

        var sentEmail = await fixture.ApiFactory.EmailTransport.WaitForEmailToAsync(recipient, _ => true);
        sentEmail.Subject.ShouldBe(BackgroundTaskRequests.EmailSubject);
    }
}
