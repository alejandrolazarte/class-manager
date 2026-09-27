namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_creating_pack_with_a_taken_name;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.CreateClassPackAsync();

        using var response = await business.HttpClient.PostClassPackAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
