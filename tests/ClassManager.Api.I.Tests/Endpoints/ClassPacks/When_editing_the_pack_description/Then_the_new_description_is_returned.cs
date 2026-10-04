using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_editing_the_pack_description;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_new_description_is_returned(ApiFixture fixture)
{
    private const string Description = "Técnica de los cuatro estilos, virajes y resistencia.";

    [Fact]
    public async Task Then_the_new_description_is_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var pack = await business.HttpClient.CreateClassPackAsync();

        using var response = await business.HttpClient.PutAsJsonAsync(
            $"{ApiRoutes.ClassPacks}/{pack.Id}",
            new UpdateClassPackRequest(pack.Name, pack.ClassCount, pack.Price, pack.ValidityMonths, Description: Description),
            ApiRequests.JsonOptions);

        var updatedPack = await response.Content.ReadFromJsonAsync<ClassPackResponse>(ApiRequests.JsonOptions);
        updatedPack!.Description.ShouldBe(Description);
    }
}
