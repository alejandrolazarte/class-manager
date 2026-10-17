using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_an_account_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_push_subscriptions_are_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_push_subscriptions_are_deleted_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();

        await scenario.DeleteAccountAsync();

        await using var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id);
        (await context.PushSubscriptions.AnyAsync(subscription => subscription.Endpoint == endpoint)).ShouldBeFalse();
    }
}
