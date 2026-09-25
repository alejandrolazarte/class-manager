using ClassManager.Core.UseCases;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_deactivating_instructor_with_active_class_groups;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        await business.HttpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));

        using var response = await business.HttpClient.PutAsJsonAsync(
            $"{ApiRoutes.Instructors}/{instructor.Id}{ApiRoutes.Active}", new SetActiveRequest(false), ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
