using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Api.I.Tests.Endpoints.ClassGroups.When_listing_class_groups;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_current_business_groups_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_current_business_groups_are_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var ownClassGroup = await business.HttpClient.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor((await business.HttpClient.CreateInstructorAsync()).Id));
        await otherBusiness.HttpClient.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor((await otherBusiness.HttpClient.CreateInstructorAsync()).Id));

        var classGroups = await business.HttpClient.GetFromJsonAsync<List<ClassGroupResponse>>(
            new Uri(ApiRoutes.ClassGroups, UriKind.Relative), ApiRequests.JsonOptions);

        classGroups!.Select(classGroup => classGroup.Id).ShouldBe([ownClassGroup.Id]);
    }
}
