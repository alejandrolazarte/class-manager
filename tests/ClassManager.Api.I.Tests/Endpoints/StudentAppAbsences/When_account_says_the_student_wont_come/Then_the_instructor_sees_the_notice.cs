namespace ClassManager.Api.I.Tests.Endpoints.StudentAppAbsences.When_account_says_the_student_wont_come;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_instructor_sees_the_notice(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_instructor_sees_the_notice_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var classGroupId = scenario.Instructors.InstructorClassGroup.Id;
        var nextWeek = InstructorScenario.ClassDate.AddDays(7);

        using var response = await scenario.Student.PutAbsenceAsync(scenario.Instructors.InstructorStudentId, classGroupId, nextWeek);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var session = await scenario.Instructors.Instructor.GetSessionAsync(classGroupId, nextWeek);
        session!.Students.Single().AbsenceNotified.ShouldBeTrue();
        var home = await scenario.Student.GetStudentAppHomeAsync();
        home!.Students.Single().NextClasses.Single(nextClass => nextClass.Date == nextWeek).AbsenceNotified.ShouldBeTrue();
    }
}
