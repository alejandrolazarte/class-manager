using ClassManager.Core.UseCases;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Api.I.Tests.Endpoints.ClassGroups.When_deactivating_class_group;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_hidden_from_active_list(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_hidden_from_active_list_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var classGroup = await business.HttpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));

        using var response = await business.HttpClient.PutAsJsonAsync(
            $"{ApiRoutes.ClassGroups}/{classGroup.Id}{ApiRoutes.Active}", new SetActiveRequest(false), ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var activeClassGroups = await business.HttpClient.GetFromJsonAsync<List<ClassGroupResponse>>(
            new Uri(ApiRoutes.ClassGroups, UriKind.Relative), ApiRequests.JsonOptions);
        activeClassGroups.ShouldBeEmpty();
    }
}
