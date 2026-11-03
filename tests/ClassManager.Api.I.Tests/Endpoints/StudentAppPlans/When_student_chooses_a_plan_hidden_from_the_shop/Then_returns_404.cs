namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPlans.When_student_chooses_a_plan_hidden_from_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var hidden = await scenario.Instructors.Business.HttpClient.CreateHiddenMonthlyPlanAsync();

        using var response = await scenario.Student.PutStudentAppPlanAsync(scenario.Instructors.InstructorStudentId, hidden.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
