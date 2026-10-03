namespace ClassManager.Api.I.Tests.Endpoints.ClassPackImages.When_business_B_adds_a_photo_to_a_class_pack_of_business_A;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var businessA = await fixture.SeedBusinessAsync();
        var businessB = await fixture.SeedBusinessAsync();
        var classPackOfA = await businessA.HttpClient.CreateClassPackAsync();

        using var response = await businessB.HttpClient.PostClassPackImageAsync(classPackOfA.Id, CatalogImageRequests.PngImage);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
