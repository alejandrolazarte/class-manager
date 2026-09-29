namespace ClassManager.Api.I.Tests.Endpoints.Families.When_coach_invites_a_family_they_cannot_see;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedCollectorScenarioAsync();

        using var response = await scenario.Coaches.Coach.PostFamilyInvitationAsync(scenario.CollectorFamilyId, FamilyRequests.UniqueFamilyEmail());

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
