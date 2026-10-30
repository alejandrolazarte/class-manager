namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_student_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_in_the_student_list(ApiFixture fixture)
{
    private const string AdultStudentFullName = "Julia Pérez";

    [Fact]
    public async Task Then_the_team_sees_it_in_the_student_list_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var instructors = scenario.Instructors;
        var team = instructors.Business.HttpClient;
        using var adultStudent = await fixture.SignInFamilyMemberAsync(
            team, scenario.ClientId, AdultStudentFullName, StudentAppRequests.AdultMemberBirthDate);

        await adultStudent.App.ChangeMyPhotoAsync();

        var students = await team.SearchStudentsAsync() ?? [];
        students.Single(student => student.Id == adultStudent.Student.Id).PhotoUrl.ShouldNotBeNull();
        students.Single(student => student.Id == instructors.OtherStudentId).PhotoUrl.ShouldBeNull();
    }
}
