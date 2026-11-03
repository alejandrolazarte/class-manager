namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPlans.When_student_chooses_a_pack_as_a_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var pack = await scenario.Instructors.Business.HttpClient.CreateClassPackAsync();

        using var response = await scenario.Student.PutStudentAppPlanAsync(scenario.Instructors.InstructorStudentId, pack.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
