using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_subscription_has_ended;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_changes_are_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_changes_are_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await fixture.EndSubscriptionAsync(business.Business.Id);

        using var response = await business.HttpClient.PostClientAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(SubscriptionErrorCodes.Inactive);
    }
}
