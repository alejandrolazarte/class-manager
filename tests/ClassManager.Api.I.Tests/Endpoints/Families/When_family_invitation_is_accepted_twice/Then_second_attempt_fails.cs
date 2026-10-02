namespace ClassManager.Api.I.Tests.Endpoints.Families.When_family_invitation_is_accepted_twice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_second_attempt_fails(ApiFixture fixture)
{
    [Fact]
    public async Task Then_second_attempt_fails_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostAcceptFamilyInvitationAsync(fixture.ApiFactory.EmailTransport.FamilyInvitationTokenSentTo(scenario.Email));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
