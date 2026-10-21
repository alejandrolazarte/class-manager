using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_creating_a_pack_without_classes_or_duration;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostAsJsonAsync(
            ApiRoutes.ClassPacks,
            new CreateClassPackCommand(ClassPackRequests.PackName, 4, 80m, 1),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
