using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_up_with_taken_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var email = AuthenticationRequests.UniqueEmail();
        await client.SignUpAsync(AuthenticationRequests.SignUpCommand(email));

        using var response = await client.PostSignUpAsync(AuthenticationRequests.SignUpCommand(email.ToUpperInvariant()));

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(AuthenticationErrorCodes.EmailTaken);
    }
}
