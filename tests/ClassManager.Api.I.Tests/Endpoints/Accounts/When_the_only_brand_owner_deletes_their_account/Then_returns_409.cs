using ClassManager.Core.UseCases.Branches;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_the_only_brand_owner_deletes_their_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var tokens = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(tokens.AccessToken);

        using var response = await owner.PostAccountDeletionAsync(AuthenticationRequests.OwnerPassword);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(BranchErrorCodes.LastBrandOwner);
    }
}
