using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_up_with_the_emailed_link;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_email_is_confirmed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_email_is_confirmed_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();

        await client.SignUpAsync(fixture.ApiFactory.EmailTransport, command);

        await using var securityContext = fixture.CreateSecurityDbContext();
        (await securityContext.Users.SingleAsync(user => user.Email == command.Email)).EmailConfirmed.ShouldBeTrue();
    }
}
