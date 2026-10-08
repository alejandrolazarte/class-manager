namespace ClassManager.Api.I.Tests.BackgroundTasks.When_a_task_is_queued_in_a_transaction_that_rolls_back;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_stored(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_stored_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var recipient = BackgroundTaskRequests.UniqueRecipient();

        await fixture.EnqueueEmailAndRollBackAsync(business.Business.Id, recipient);

        (await fixture.FindTaskToAsync(recipient)).ShouldBeNull();
        fixture.ApiFactory.EmailTransport.SentTo(recipient).ShouldBeEmpty();
    }
}
