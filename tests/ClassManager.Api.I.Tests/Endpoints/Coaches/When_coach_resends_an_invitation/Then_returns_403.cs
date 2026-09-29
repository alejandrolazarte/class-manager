using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_resends_an_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();
        var invitation = await scenario.Business.HttpClient.InviteAsync(MemberRequests.UniqueInviteeEmail(), BusinessRole.Viewer);

        using var response = await scenario.Coach.PostAsync(
            new Uri($"{MemberRequests.InvitationsRoute}/{invitation.Id}{ApiRoutes.Resend}", UriKind.Relative), null);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
