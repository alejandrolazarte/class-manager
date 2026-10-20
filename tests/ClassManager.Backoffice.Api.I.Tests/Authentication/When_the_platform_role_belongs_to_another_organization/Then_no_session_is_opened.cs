namespace ClassManager.Backoffice.Api.I.Tests.Authentication.When_the_platform_role_belongs_to_another_organization;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_session_is_opened(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_no_session_is_opened_Run()
    {
        var outsider = await fixture.SeedOperatorOfAnotherOrganizationAsync();
        using var client = fixture.ApiFactory.CreateHttpsClient();

        using var response = await client.SignInWithGoogleAsync(outsider.Subject, outsider.Email);

        BackofficeRequests.RefreshTokenCookieOf(response).ShouldBeNull();
        response.Headers.Location!.ToString().ShouldContain(BackofficeRequests.AccessRequestCodeParameter);
    }
}
