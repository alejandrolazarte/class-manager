using System.Net.Http.Json;

namespace ClassManager.Backoffice.Api.I.Tests.Operators.When_an_operator_disables_another;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_their_session_ends(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_their_session_ends_Run()
    {
        var disabled = await fixture.SeedOperatorAsync();
        using var disabledClient = fixture.ApiFactory.CreateHttpsClient();
        using (var signIn = await disabledClient.SignInWithGoogleAsync(disabled.Subject, disabled.Email))
        {
            BackofficeRequests.RefreshTokenCookieOf(signIn).ShouldNotBeNull();
        }

        using var operatorClient = fixture.ApiFactory.CreateHttpsClient();
        var accessToken = await operatorClient.OpenSessionAsync(await fixture.SeedOperatorAsync());
        using var list = await operatorClient.GetAsync(BackofficeRequests.OperatorsRoute, accessToken);
        var login = (await list.Content.ReadFromJsonAsync<OperatorListResponse>(BackofficeRequests.JsonOptions))!.Operators
            .Single(candidate => candidate.Email == disabled.Email);
        using var disable = await operatorClient.SendAsync(HttpMethod.Delete, $"{BackofficeRequests.OperatorsRoute}/{login.LoginId}", accessToken);
        disable.StatusCode.ShouldBe(HttpStatusCode.OK);

        using var refresh = await disabledClient.PostRefreshAsync();

        refresh.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
