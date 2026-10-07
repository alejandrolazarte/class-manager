using ClassManager.Infrastructure.Email;

namespace ClassManager.Api.I.Tests.BackgroundTasks.When_a_task_is_queued;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_stored_under_its_class_name(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_stored_under_its_class_name_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var recipient = BackgroundTaskRequests.UniqueRecipient();
        fixture.ApiFactory.EmailTransport.FailDeliveriesTo(recipient);

        await fixture.EnqueueEmailAsync(business.Business.Id, recipient);

        var task = await fixture.WaitForTaskToAsync(recipient, _ => true);
        task.Type.ShouldBe(nameof(SendEmailBackgroundTaskCommand));
    }
}
