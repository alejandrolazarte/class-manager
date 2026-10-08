namespace ClassManager.Api.I.Tests.Endpoints.StudentAppAbsences.When_account_warns_for_a_student_of_another_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        using var response = await scenario.Student.PutAbsenceAsync(
            scenario.Instructors.OtherStudentId, scenario.Instructors.OtherClassGroup.Id, InstructorScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
