namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_resetting_password_with_a_link;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_link_carries_the_email(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_link_carries_the_email_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        await client.SignUpAsync(fixture.ApiFactory.EmailTransport, command);

        var link = await client.RequestPasswordResetLinkAsync(fixture.ApiFactory.EmailTransport, command.Email!);

        AuthenticationRequests.EmailOf(link).ShouldBe(command.Email);
    }
}
