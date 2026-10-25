namespace ClassManager.Api.I.Tests.Endpoints.Members.When_someone_else_accepts_the_invitation_of_an_instructor_with_an_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_accepting_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_accepting_returns_409_Run()
    {
        using var scenario = await fixture.SeedOwnerUnlinkedFromTheirInstructorAsync();
        var token = await fixture.SeedInvitationForOwnerInstructorAsync(scenario, MemberRequests.UniqueInviteeEmail());
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        using var response = await anonymousClient.PostAcceptInvitationAsync(token);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
