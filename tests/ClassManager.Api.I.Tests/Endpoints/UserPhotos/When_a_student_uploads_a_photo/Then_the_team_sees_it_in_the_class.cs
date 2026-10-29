namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_student_uploads_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_in_the_class(ApiFixture fixture)
{
    private const string AdultStudentFullName = "Julia Pérez";

    [Fact]
    public async Task Then_the_team_sees_it_in_the_class_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var instructors = scenario.Instructors;
        var team = instructors.Business.HttpClient;
        using var adultStudent = await fixture.SignInFamilyMemberAsync(
            team, scenario.ClientId, AdultStudentFullName, StudentAppRequests.AdultMemberBirthDate);
        await team.EnrollAsync(instructors.InstructorClassGroup.Id, adultStudent.Student.Id, InstructorScenario.ClassDate);

        await adultStudent.App.ChangeMyPhotoAsync();

        var session = await instructors.Instructor.GetSessionAsync(instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate);
        session!.Students.Single(student => student.StudentId == adultStudent.Student.Id).PhotoUrl.ShouldNotBeNull();
    }
}
