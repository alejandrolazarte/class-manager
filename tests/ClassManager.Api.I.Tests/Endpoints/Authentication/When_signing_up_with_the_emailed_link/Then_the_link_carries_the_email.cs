namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_up_with_the_emailed_link;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_link_carries_the_email(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_link_carries_the_email_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        using var response = await client.PostSignUpAsync(command);

        var link = fixture.ApiFactory.EmailTransport.SignUpLinkSentTo(command.Email!);

        AuthenticationRequests.EmailOf(link).ShouldBe(command.Email);
    }
}
