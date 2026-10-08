namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_two_students_exist;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_sees_only_their_own_students(ApiFixture fixture)
{
    [Fact]
    public async Task Then_each_sees_only_their_own_students_Run()
    {
        var instructors = await fixture.SeedInstructorScenarioAsync();
        var firstClient = await fixture.InviteStudentAppOfAsync(instructors, InstructorScenario.InstructorStudentFullName);
        var secondClient = await fixture.InviteStudentAppOfAsync(instructors, InstructorScenario.OtherStudentFullName);

        var firstHome = await firstClient.Student.GetStudentAppHomeAsync();
        var secondHome = await secondClient.Student.GetStudentAppHomeAsync();

        firstHome!.Students.Select(student => student.FullName).ShouldBe([InstructorScenario.InstructorStudentFullName]);
        secondHome!.Students.Select(student => student.FullName).ShouldBe([InstructorScenario.OtherStudentFullName]);
    }
}
