using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_up_with_a_used_link;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var token = await client.RequestSignUpLinkAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        (await client.PostCompleteSignUpAsync(token)).EnsureSuccessStatusCode();

        using var response = await client.PostCompleteSignUpAsync(token);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(AuthenticationErrorCodes.InvalidSignUpToken);
    }
}
