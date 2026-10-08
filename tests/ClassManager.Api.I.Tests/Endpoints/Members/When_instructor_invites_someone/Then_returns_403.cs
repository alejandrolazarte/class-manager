using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_instructor_invites_someone;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.PostInvitationAsync(MemberRequests.UniqueInviteeEmail(), BusinessRole.Viewer);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
