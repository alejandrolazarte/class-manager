using ClassManager.Core.UseCases.Members;

namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_team_member_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_in_the_team_list(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_sees_it_in_the_team_list_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        await scenario.Instructor.ChangeMyPhotoAsync();

        var team = await scenario.Business.HttpClient.GetFromJsonAsync<TeamResponse>(
            new Uri(ApiRoutes.Members, UriKind.Relative), ApiRequests.JsonOptions);
        team.ShouldNotBeNull();
        team.Members.Single(member => member.InstructorId == scenario.InstructorId).PhotoUrl.ShouldNotBeNull();
        team.Members.Single(member => member.IsCurrentUser).PhotoUrl.ShouldBeNull();
    }
}
