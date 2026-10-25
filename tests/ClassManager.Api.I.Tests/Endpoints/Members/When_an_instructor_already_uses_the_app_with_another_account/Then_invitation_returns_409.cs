using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_an_instructor_already_uses_the_app_with_another_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_invitation_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_invitation_returns_409_Run()
    {
        using var scenario = await fixture.SeedOwnerUnlinkedFromTheirInstructorAsync();

        using var response = await scenario.OwnerClient.PostInvitationAsync(
            MemberRequests.UniqueInviteeEmail(), BusinessRole.Instructor, scenario.OwnerInstructorId);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
