namespace ClassManager.Api.I.Tests.Endpoints.FamilyAbsences.When_family_warns_for_a_student_of_another_family;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();

        using var response = await scenario.Family.PutAbsenceAsync(
            scenario.Coaches.OtherStudentId, scenario.Coaches.OtherClassGroup.Id, CoachScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
