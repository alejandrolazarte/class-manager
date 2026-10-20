namespace ClassManager.Backoffice.Api.I.Tests.Authentication.When_the_session_is_refreshed_hours_later;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_still_ends_when_it_was_going_to(BackofficeApiFixture fixture)
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(8);
    private static readonly TimeSpan TimeUntilRefresh = TimeSpan.FromHours(2);

    [Fact]
    public async Task Then_it_still_ends_when_it_was_going_to_Run()
    {
        var platformOperator = await fixture.SeedOperatorAsync();
        using var client = fixture.ApiFactory.CreateHttpsClient();
        var signedInAt = fixture.ApiFactory.TimeProvider.GetUtcNow();
        using var signIn = await client.SignInWithGoogleAsync(platformOperator.Subject, platformOperator.Email);
        fixture.ApiFactory.TimeProvider.Advance(TimeUntilRefresh);

        using var refresh = await client.PostRefreshAsync();

        refresh.StatusCode.ShouldBe(HttpStatusCode.OK);
        BackofficeRequests.RefreshTokenCookieOf(refresh)!.Expires.ShouldBe(signedInAt + SessionLifetime);
    }
}
