using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Authorization.When_token_role_is_not_owner;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_settings_endpoint_returns_403(ApiFixture fixture)
{
    private const BusinessRole RoleWithoutSettingsAccess = (BusinessRole)99;

    [Fact]
    public async Task Then_settings_endpoint_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var httpClient = fixture.CreateClientWithToken(fixture.CreateAccessToken(business.Business.Id, role: RoleWithoutSettingsAccess));

        using var response = await httpClient.PutBusinessAsync(SettingsRequests.SettingsOf(business));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
