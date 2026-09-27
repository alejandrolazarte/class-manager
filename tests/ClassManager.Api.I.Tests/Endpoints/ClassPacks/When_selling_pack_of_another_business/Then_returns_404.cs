namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_selling_pack_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherPack = await otherBusiness.HttpClient.CreateClassPackAsync();
        var client = await business.HttpClient.RegisterClientAsync();

        using var response = await business.HttpClient.PostClassPackSaleAsync(client.Id, otherPack.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
