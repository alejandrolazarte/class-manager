using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_an_instructor_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_in_the_instructor_list(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_sees_it_in_the_instructor_list_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        await scenario.Instructor.ChangeMyPhotoAsync();

        var instructors = await scenario.Business.HttpClient.GetFromJsonAsync<List<InstructorResponse>>(
            new Uri(ApiRoutes.Instructors, UriKind.Relative), ApiRequests.JsonOptions) ?? [];
        instructors.Single(instructor => instructor.Id == scenario.InstructorId).PhotoUrl.ShouldNotBeNull();
        instructors.Single(instructor => instructor.Id == scenario.OtherInstructorId).PhotoUrl.ShouldBeNull();
    }
}
