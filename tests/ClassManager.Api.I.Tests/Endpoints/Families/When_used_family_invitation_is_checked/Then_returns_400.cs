namespace ClassManager.Api.I.Tests.Endpoints.Families.When_used_family_invitation_is_checked;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostCheckFamilyInvitationAsync(fixture.ApiFactory.EmailTransport.FamilyInvitationTokenSentTo(scenario.Email));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
