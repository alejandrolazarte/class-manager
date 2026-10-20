namespace ClassManager.Backoffice.Api.I.Tests.Authentication.When_an_enabled_operator_signs_in_with_google;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_web_app_opens_with_a_session(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_the_web_app_opens_with_a_session_Run()
    {
        var platformOperator = await fixture.SeedOperatorAsync();
        using var client = fixture.ApiFactory.CreateHttpsClient();

        using var response = await client.SignInWithGoogleAsync(platformOperator.Subject, platformOperator.Email);

        response.Headers.Location.ShouldBe(new Uri(BackofficeApiFactory.WebAppUrl));
        var cookie = BackofficeRequests.RefreshTokenCookieOf(response).ShouldNotBeNull();
        (cookie.HttpOnly, cookie.Secure).ShouldBe((true, true));
    }
}
