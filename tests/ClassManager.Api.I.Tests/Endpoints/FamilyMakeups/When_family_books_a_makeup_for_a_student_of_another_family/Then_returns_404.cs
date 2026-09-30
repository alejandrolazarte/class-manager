namespace ClassManager.Api.I.Tests.Endpoints.FamilyMakeups.When_family_books_a_makeup_for_a_student_of_another_family;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        using var response = await scenario.Family.PutMakeupAsync(
            scenario.Coaches.OtherStudentId, scenario.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
