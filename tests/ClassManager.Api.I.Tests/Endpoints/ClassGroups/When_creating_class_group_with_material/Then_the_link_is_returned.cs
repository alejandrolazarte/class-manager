using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Api.I.Tests.Endpoints.ClassGroups.When_creating_class_group_with_material;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_link_is_returned(ApiFixture fixture)
{
    private const string MaterialUrl = "https://dfswimmingteam.com/material/adultos.pdf";

    [Fact]
    public async Task Then_the_link_is_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();

        using var response = await business.HttpClient.PostClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(instructor.Id) with { MaterialUrl = MaterialUrl });

        var classGroup = await response.Content.ReadFromJsonAsync<ClassGroupResponse>(ApiRequests.JsonOptions);
        classGroup!.MaterialUrl.ShouldBe(MaterialUrl);
    }
}
