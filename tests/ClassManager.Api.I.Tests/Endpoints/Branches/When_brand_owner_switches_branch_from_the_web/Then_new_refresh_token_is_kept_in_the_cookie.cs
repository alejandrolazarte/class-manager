using System.Text.Json;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.Branches;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_brand_owner_switches_branch_from_the_web;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_new_refresh_token_is_kept_in_the_cookie(ApiFixture fixture)
{
    private const string AccessTokenProperty = "accessToken";

    [Fact]
    public async Task Then_new_refresh_token_is_kept_in_the_cookie_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        using var signInResponse = await client.SignUpAndSignInFromWebAsync(fixture.ApiFactory.EmailTransport);
        var signInBody = await signInResponse.Content.ReadFromJsonAsync<JsonElement>();
        using var ownerClient = fixture.CreateClientWithToken(signInBody.GetProperty(AccessTokenProperty).GetString()!);
        await fixture.ChangePlanAsync((await ownerClient.GetCurrentMemberAsync()).BusinessId, PlanCodes.Enterprise);
        var branch = await ownerClient.CreateBranchAsync("Sede centro");
        var signInCookieValue = signInResponse.RefreshTokenCookieOf().Value.Value!;

        using var switchResponse = await client.PostFromWebAsync(
            BranchRequests.SwitchBranchRoute, new SwitchBranchCommand(null, branch.BusinessId), signInCookieValue);

        switchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        switchResponse.RefreshTokenCookieOf().Value.Value.ShouldNotBe(signInCookieValue);
    }
}
