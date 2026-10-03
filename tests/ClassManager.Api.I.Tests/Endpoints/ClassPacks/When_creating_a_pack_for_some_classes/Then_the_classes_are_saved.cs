using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_creating_a_pack_for_some_classes;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_classes_are_saved(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_classes_are_saved_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();

        await business.HttpClient.CreateClassPackAsync(classGroupIds: [classGroup.Id]);

        var classPacks = await business.HttpClient.GetFromJsonAsync<List<ClassPackResponse>>(
            new Uri(ApiRoutes.ClassPacks, UriKind.Relative), ApiRequests.JsonOptions);
        classPacks!.Single().ClassGroupIds.ShouldBe([classGroup.Id]);
    }
}
