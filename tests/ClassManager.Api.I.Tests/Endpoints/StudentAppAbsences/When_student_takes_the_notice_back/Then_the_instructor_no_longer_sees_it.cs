namespace ClassManager.Api.I.Tests.Endpoints.StudentAppAbsences.When_student_takes_the_notice_back;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_instructor_no_longer_sees_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_instructor_no_longer_sees_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var classGroupId = scenario.Instructors.InstructorClassGroup.Id;
        var studentId = scenario.Instructors.InstructorStudentId;
        (await scenario.Student.PutAbsenceAsync(studentId, classGroupId, InstructorScenario.ClassDate)).EnsureSuccessStatusCode();

        using var response = await scenario.Student.DeleteAbsenceAsync(studentId, classGroupId, InstructorScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var session = await scenario.Instructors.Instructor.GetSessionAsync(classGroupId, InstructorScenario.ClassDate);
        session!.Students.Single().AbsenceNotified.ShouldBeFalse();
    }
}
