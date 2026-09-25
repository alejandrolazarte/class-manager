using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Api.I.Tests.Endpoints.Tenancy.When_token_is_expired;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_401(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_401_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var anHourAgo = new FakeTimeProvider(BusinessApiFactory.Now.AddHours(-1));
        using var client = fixture.CreateClientWithToken(fixture.CreateAccessToken(business.Business.Id, anHourAgo));

        using var response = await client.GetAsync(new Uri(ApiRoutes.Clients, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
