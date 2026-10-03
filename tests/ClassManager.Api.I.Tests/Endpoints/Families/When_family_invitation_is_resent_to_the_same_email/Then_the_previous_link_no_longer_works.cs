namespace ClassManager.Api.I.Tests.Endpoints.Families.When_family_invitation_is_resent_to_the_same_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_previous_link_no_longer_works(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_previous_link_no_longer_works_Run()
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var familyId = fees!.Clients.Single(client => client.StudentNames.Contains(CoachScenario.CoachStudentFullName)).ClientId;
        var email = FamilyRequests.UniqueFamilyEmail();
        using var previousInvitation = await coaches.Business.HttpClient.PostFamilyInvitationAsync(familyId, email);
        var previousToken = fixture.ApiFactory.EmailTransport.FamilyInvitationTokenSentTo(email);
        using var resentInvitation = await coaches.Business.HttpClient.PostFamilyInvitationAsync(familyId, email.ToUpperInvariant());

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var response = await anonymous.PostAcceptFamilyInvitationAsync(previousToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
