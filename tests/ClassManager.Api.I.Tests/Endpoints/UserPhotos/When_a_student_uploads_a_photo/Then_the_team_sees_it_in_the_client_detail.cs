namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_student_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_in_the_client_detail(ApiFixture fixture)
{
    private const string AdultStudentFullName = "Julia Pérez";

    [Fact]
    public async Task Then_the_team_sees_it_in_the_client_detail_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var instructors = scenario.Instructors;
        var team = instructors.Business.HttpClient;
        using var adultStudent = await fixture.SignInFamilyMemberAsync(
            team, scenario.ClientId, AdultStudentFullName, StudentAppRequests.AdultMemberBirthDate);

        await adultStudent.App.ChangeMyPhotoAsync();

        var client = await team.GetClientAsync(scenario.ClientId);
        client.Students.Single(student => student.Id == adultStudent.Student.Id).PhotoUrl.ShouldNotBeNull();
    }
}
