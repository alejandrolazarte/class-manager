namespace ClassManager.Api.I.Tests.Endpoints.Families.When_two_people_of_the_same_family_are_invited;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_first_invitation_still_works(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_first_invitation_still_works_Run()
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var familyId = fees!.Clients.Single(client => client.StudentNames.Contains(CoachScenario.CoachStudentFullName)).ClientId;
        var firstEmail = FamilyRequests.UniqueFamilyEmail();
        var secondEmail = FamilyRequests.UniqueFamilyEmail();
        using var firstInvitation = await coaches.Business.HttpClient.PostFamilyInvitationAsync(familyId, firstEmail);
        using var secondInvitation = await coaches.Business.HttpClient.PostFamilyInvitationAsync(familyId, secondEmail);

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var response = await anonymous.PostAcceptFamilyInvitationAsync(fixture.ApiFactory.EmailTransport.FamilyInvitationTokenSentTo(firstEmail));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
