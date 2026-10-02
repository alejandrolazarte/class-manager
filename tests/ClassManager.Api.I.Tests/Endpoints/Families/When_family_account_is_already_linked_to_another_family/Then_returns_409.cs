namespace ClassManager.Api.I.Tests.Endpoints.Families.When_family_account_is_already_linked_to_another_family;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var firstFamily = await fixture.InviteFamilyOfAsync(coaches, CoachScenario.CoachStudentFullName);
        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var otherFamilyId = fees!.Clients.Single(client => client.StudentNames.Contains(CoachScenario.OtherStudentFullName)).ClientId;
        using (var invitation = await coaches.Business.HttpClient.PostFamilyInvitationAsync(otherFamilyId, firstFamily.Email))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var response = await anonymous.PostAcceptFamilyInvitationAsync(fixture.ApiFactory.EmailTransport.FamilyInvitationTokenSentTo(firstFamily.Email));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
