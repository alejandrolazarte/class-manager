namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_accepts_the_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_see_their_student_and_next_classes(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_see_their_student_and_next_classes_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        var home = await scenario.Student.GetStudentAppHomeAsync();

        var student = home!.Students.ShouldHaveSingleItem();
        student.FullName.ShouldBe(CoachScenario.CoachStudentFullName);
        student.NextClasses[0].Name.ShouldBe(CoachScenario.CoachClassGroupName);
        student.NextClasses[0].Date.ShouldBe(CoachScenario.ClassDate);
    }
}
