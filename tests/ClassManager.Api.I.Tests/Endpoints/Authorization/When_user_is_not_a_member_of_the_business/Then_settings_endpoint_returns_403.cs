namespace ClassManager.Api.I.Tests.Endpoints.Authorization.When_user_is_not_a_member_of_the_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_settings_endpoint_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_settings_endpoint_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var httpClient = fixture.CreateClientWithToken(fixture.CreateAccessToken(business.Business.Id));

        using var response = await httpClient.PutBusinessAsync(SettingsRequests.SettingsOf(business));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
