using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_up_without_opening_the_link;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_account_or_business_exists(ApiFixture fixture)
{
    [Fact]
    public async Task Then_no_account_or_business_exists_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();

        using var response = await client.PostSignUpAsync(command);

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        await using var securityContext = fixture.CreateSecurityDbContext();
        (await securityContext.Users.AnyAsync(user => user.Email == command.Email)).ShouldBeFalse();
        await using var context = fixture.CreateDbContext(0L);
        (await context.Organizations.AnyAsync(organization => organization.Name == command.BusinessName)).ShouldBeFalse();
    }
}
