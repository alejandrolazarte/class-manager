namespace ClassManager.Api.I.Tests.BackgroundTasks.When_a_task_is_queued;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_deleted_after_running(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_deleted_after_running_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var recipient = BackgroundTaskRequests.UniqueRecipient();
        await fixture.EnqueueEmailAsync(business.Business.Id, recipient);
        await fixture.ApiFactory.EmailTransport.WaitForEmailToAsync(recipient, _ => true);

        await fixture.WaitUntilNoTaskToAsync(recipient);

        (await fixture.FindTaskToAsync(recipient)).ShouldBeNull();
    }
}
