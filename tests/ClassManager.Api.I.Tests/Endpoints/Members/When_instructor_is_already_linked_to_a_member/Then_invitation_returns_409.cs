using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_instructor_is_already_linked_to_a_member;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_invitation_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_invitation_returns_409_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Business.HttpClient.PostInvitationAsync(
            MemberRequests.UniqueInviteeEmail(), BusinessRole.Coach, scenario.CoachInstructorId);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
