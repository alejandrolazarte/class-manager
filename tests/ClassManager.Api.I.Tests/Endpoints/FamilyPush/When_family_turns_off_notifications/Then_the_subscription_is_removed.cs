using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyPush.When_family_turns_off_notifications;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_subscription_is_removed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_subscription_is_removed_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();

        using var response = await scenario.Family.DeletePushSubscriptionAsync(endpoint);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        await using var context = fixture.CreateDbContext(scenario.Coaches.Business.Business.Id);
        (await context.PushSubscriptions.AnyAsync(saved => saved.Endpoint == endpoint)).ShouldBeFalse();
    }
}
