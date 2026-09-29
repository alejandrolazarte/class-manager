namespace ClassManager.Api.I.Tests.Endpoints.Products.When_creating_product_with_a_taken_name;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.CreateProductAsync();

        using var response = await business.HttpClient.PostProductAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
