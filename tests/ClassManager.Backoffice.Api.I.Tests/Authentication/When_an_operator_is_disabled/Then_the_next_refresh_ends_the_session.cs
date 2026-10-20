using Microsoft.EntityFrameworkCore;

namespace ClassManager.Backoffice.Api.I.Tests.Authentication.When_an_operator_is_disabled;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_next_refresh_ends_the_session(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_the_next_refresh_ends_the_session_Run()
    {
        var platformOperator = await fixture.SeedOperatorAsync();
        using var client = fixture.ApiFactory.CreateHttpsClient();
        using var signIn = await client.SignInWithGoogleAsync(platformOperator.Subject, platformOperator.Email);
        await using (var context = fixture.CreateBackofficeDbContext())
        {
            await context.BackofficeLogins.Where(login => login.AccountId == platformOperator.AccountId).ExecuteDeleteAsync();
        }

        using var refresh = await client.PostRefreshAsync();

        refresh.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
