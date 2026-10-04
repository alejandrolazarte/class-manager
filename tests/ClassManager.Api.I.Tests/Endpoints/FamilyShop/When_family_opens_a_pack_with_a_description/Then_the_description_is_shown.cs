using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.FamilyShop.When_family_opens_a_pack_with_a_description;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_description_is_shown(ApiFixture fixture)
{
    private const string Description = "Clases grupales para quienes empiezan: flotación, respiración y patada.";

    [Fact]
    public async Task Then_the_description_is_shown_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        using (var created = await scenario.Coaches.Business.HttpClient.PostAsJsonAsync(
            ApiRoutes.ClassPacks,
            new CreateClassPackCommand(ClassPackRequests.PackName, 4, 80m, 1, Description: Description),
            ApiRequests.JsonOptions))
        {
            created.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var shop = await scenario.Family.GetFamilyShopAsync();

        shop.Packs.Single().Description.ShouldBe(Description);
    }
}
