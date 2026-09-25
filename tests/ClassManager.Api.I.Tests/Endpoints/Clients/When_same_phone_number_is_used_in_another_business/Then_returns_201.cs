namespace ClassManager.Api.I.Tests.Endpoints.Clients.When_same_phone_number_is_used_in_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_201(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_201_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        await otherBusiness.HttpClient.RegisterClientAsync();

        using var response = await business.HttpClient.PostClientAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
