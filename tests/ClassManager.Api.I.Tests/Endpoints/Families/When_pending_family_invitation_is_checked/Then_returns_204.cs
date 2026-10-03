namespace ClassManager.Api.I.Tests.Endpoints.Families.When_pending_family_invitation_is_checked;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_204(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_204_Run()
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var familyId = fees!.Clients.Single(client => client.StudentNames.Contains(CoachScenario.CoachStudentFullName)).ClientId;
        var email = FamilyRequests.UniqueFamilyEmail();
        using (var invitation = await coaches.Business.HttpClient.PostFamilyInvitationAsync(familyId, email))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();

        using var response = await anonymous.PostCheckFamilyInvitationAsync(fixture.ApiFactory.EmailTransport.FamilyInvitationTokenSentTo(email));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
