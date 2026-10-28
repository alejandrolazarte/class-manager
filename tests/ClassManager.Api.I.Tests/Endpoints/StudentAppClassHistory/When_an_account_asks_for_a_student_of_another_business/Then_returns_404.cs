namespace ClassManager.Api.I.Tests.Endpoints.StudentAppClassHistory.When_an_account_asks_for_a_student_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var otherBusiness = await fixture.SeedStudentAppScenarioAsync();

        using var response = await scenario.Student.GetClassHistoryResponseAsync(otherBusiness.Instructors.InstructorStudentId, ClassHistoryRequests.CurrentMonth);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
