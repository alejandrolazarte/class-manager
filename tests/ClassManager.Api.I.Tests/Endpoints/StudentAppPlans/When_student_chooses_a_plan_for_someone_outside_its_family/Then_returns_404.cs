namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPlans.When_student_chooses_a_plan_for_someone_outside_its_family;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var plan = await owner.CreateMonthlyPlanAsync();
        var otherFamily = await owner.RegisterClientAsync("Marta Gómez", "+5491155550199", [new Core.UseCases.Students.NewStudent("Marta Gómez", null, null)]);

        using var response = await scenario.Student.PutStudentAppPlanAsync(otherFamily.Students[0].Id, plan.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
