namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_searches_students;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_students_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_students_are_returned_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        var students = await scenario.Coach.SearchStudentsAsync();

        students!.Select(student => student.Id).ShouldBe([scenario.CoachStudentId]);
    }
}
