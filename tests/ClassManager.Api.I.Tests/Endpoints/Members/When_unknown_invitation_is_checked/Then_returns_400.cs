namespace ClassManager.Api.I.Tests.Endpoints.Members.When_unknown_invitation_is_checked;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        using var response = await anonymousClient.PostCheckInvitationAsync("a-token-that-was-never-issued");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
