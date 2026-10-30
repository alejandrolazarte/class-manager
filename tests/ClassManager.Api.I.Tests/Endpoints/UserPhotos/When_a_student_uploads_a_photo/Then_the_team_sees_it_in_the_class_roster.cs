namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_student_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_in_the_class_roster(ApiFixture fixture)
{
    private const string AdultStudentFullName = "Julia Pérez";

    [Fact]
    public async Task Then_the_team_sees_it_in_the_class_roster_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var instructors = scenario.Instructors;
        var team = instructors.Business.HttpClient;
        using var adultStudent = await fixture.SignInFamilyMemberAsync(
            team, scenario.ClientId, AdultStudentFullName, StudentAppRequests.AdultMemberBirthDate);
        await team.EnrollAsync(instructors.InstructorClassGroup.Id, adultStudent.Student.Id, InstructorScenario.ClassDate);

        await adultStudent.App.ChangeMyPhotoAsync();

        var roster = await team.GetRosterAsync(instructors.InstructorClassGroup.Id) ?? [];
        roster.Single(entry => entry.StudentId == adultStudent.Student.Id).StudentPhotoUrl.ShouldNotBeNull();
    }
}
