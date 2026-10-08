using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_instructor_is_already_linked_to_a_member;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_invitation_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_invitation_returns_409_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Business.HttpClient.PostInvitationAsync(
            MemberRequests.UniqueInviteeEmail(), BusinessRole.Instructor, scenario.InstructorId);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
