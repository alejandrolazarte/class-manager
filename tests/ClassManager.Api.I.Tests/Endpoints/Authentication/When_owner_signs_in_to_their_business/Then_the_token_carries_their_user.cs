namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_owner_signs_in_to_their_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_token_carries_their_user(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_token_carries_their_user_Run()
    {
        var email = AuthenticationRequests.UniqueEmail();
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand(email));
        using var owner = fixture.CreateClientWithToken(signUp.AccessToken);
        var businessId = (await owner.GetCurrentMemberAsync()).BusinessId;

        var tokens = await anonymous.SignInAsync(email);

        var userId = await fixture.FindUserIdByAccountAsync(businessId, UserRows.AccountIdOf(tokens.AccessToken));
        UserRows.UserIdOf(tokens.AccessToken).ShouldBe(userId!.Value);
    }
}
