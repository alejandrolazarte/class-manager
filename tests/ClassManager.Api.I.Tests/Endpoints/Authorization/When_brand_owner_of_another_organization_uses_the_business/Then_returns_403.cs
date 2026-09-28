namespace ClassManager.Api.I.Tests.Endpoints.Authorization.When_brand_owner_of_another_organization_uses_the_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        using var httpClient = fixture.CreateClientFor(business.Business.Id, otherBusiness.OwnerUserId);

        using var response = await httpClient.GetAsync(new Uri(ApiRoutes.Clients, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
