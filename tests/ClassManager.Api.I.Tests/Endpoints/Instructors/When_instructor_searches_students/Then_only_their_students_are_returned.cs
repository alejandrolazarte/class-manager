namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_searches_students;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_students_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_students_are_returned_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        var students = await scenario.Instructor.SearchStudentsAsync();

        students!.Select(student => student.Id).ShouldBe([scenario.InstructorStudentId]);
    }
}
