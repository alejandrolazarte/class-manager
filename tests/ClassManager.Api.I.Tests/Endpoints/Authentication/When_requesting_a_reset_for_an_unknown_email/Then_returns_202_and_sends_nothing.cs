namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_requesting_a_reset_for_an_unknown_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_202_and_sends_nothing(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_202_and_sends_nothing_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var unknownEmail = AuthenticationRequests.UniqueEmail();

        using var response = await client.PostPasswordResetRequestAsync(unknownEmail);

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        fixture.ApiFactory.EmailTransport.SentTo(unknownEmail).ShouldBeEmpty();
    }
}
