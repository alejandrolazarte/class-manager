namespace ClassManager.Backoffice.Api.I.Tests.Authentication.When_the_session_is_refreshed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_access_token_opens_the_backoffice(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_its_access_token_opens_the_backoffice_Run()
    {
        var platformOperator = await fixture.SeedOperatorAsync();
        using var client = fixture.ApiFactory.CreateHttpsClient();
        using var signIn = await client.SignInWithGoogleAsync(platformOperator.Subject, platformOperator.Email);

        using var refresh = await client.PostRefreshAsync();

        refresh.StatusCode.ShouldBe(HttpStatusCode.OK);
        using var currentOperator = await client.GetCurrentOperatorAsync(await BackofficeRequests.AccessTokenOfAsync(refresh));
        currentOperator.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
