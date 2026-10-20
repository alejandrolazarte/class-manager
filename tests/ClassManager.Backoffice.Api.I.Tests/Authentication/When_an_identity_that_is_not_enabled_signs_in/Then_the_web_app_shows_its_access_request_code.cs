using Microsoft.EntityFrameworkCore;

namespace ClassManager.Backoffice.Api.I.Tests.Authentication.When_an_identity_that_is_not_enabled_signs_in;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_web_app_shows_its_access_request_code(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_the_web_app_shows_its_access_request_code_Run()
    {
        var platformOperator = await fixture.SeedOperatorAsync(isEnabled: false);
        using var client = fixture.ApiFactory.CreateHttpsClient();

        using var response = await client.SignInWithGoogleAsync(platformOperator.Subject, platformOperator.Email);

        await using var context = fixture.CreateBackofficeDbContext();
        var accessRequest = await context.BackofficeAccessRequests.SingleAsync(request => request.ProviderKey == platformOperator.Subject);
        response.Headers.Location!.ToString().ShouldEndWith(BackofficeRequests.AccessRequestCodeParameter + accessRequest.Code);
        BackofficeRequests.RefreshTokenCookieOf(response).ShouldBeNull();
    }
}
