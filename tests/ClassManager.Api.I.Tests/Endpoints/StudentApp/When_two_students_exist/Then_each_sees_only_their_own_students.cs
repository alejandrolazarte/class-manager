namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_two_students_exist;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_each_sees_only_their_own_students(ApiFixture fixture)
{
    [Fact]
    public async Task Then_each_sees_only_their_own_students_Run()
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var firstClient = await fixture.InviteStudentAppOfAsync(coaches, CoachScenario.CoachStudentFullName);
        var secondClient = await fixture.InviteStudentAppOfAsync(coaches, CoachScenario.OtherStudentFullName);

        var firstHome = await firstClient.Student.GetStudentAppHomeAsync();
        var secondHome = await secondClient.Student.GetStudentAppHomeAsync();

        firstHome!.Students.Select(student => student.FullName).ShouldBe([CoachScenario.CoachStudentFullName]);
        secondHome!.Students.Select(student => student.FullName).ShouldBe([CoachScenario.OtherStudentFullName]);
    }
}
