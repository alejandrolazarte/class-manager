using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_creating_pack_with_duration_and_material;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_are_returned(ApiFixture fixture)
{
    private const string MaterialUrl = "https://dfswimmingteam.com/material/adg-de-autor.pdf";

    [Fact]
    public async Task Then_they_are_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostAsJsonAsync(
            ApiRoutes.ClassPacks,
            new CreateClassPackCommand("ADG De Autor", 10, 450m, 3, 45, MaterialUrl),
            ApiRequests.JsonOptions);

        var pack = await response.Content.ReadFromJsonAsync<ClassPackResponse>(ApiRequests.JsonOptions);
        (pack!.ClassDurationMinutes, pack.MaterialUrl).ShouldBe((45, MaterialUrl));
    }
}
