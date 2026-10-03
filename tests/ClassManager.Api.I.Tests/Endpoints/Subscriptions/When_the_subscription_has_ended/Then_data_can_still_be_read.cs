namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_subscription_has_ended;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_data_can_still_be_read(ApiFixture fixture)
{
    [Fact]
    public async Task Then_data_can_still_be_read_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync();
        await fixture.EndSubscriptionAsync(business.Business.Id);

        using var response = await business.HttpClient.GetAsync(new Uri($"{ApiRoutes.Clients}/{client.Id}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
