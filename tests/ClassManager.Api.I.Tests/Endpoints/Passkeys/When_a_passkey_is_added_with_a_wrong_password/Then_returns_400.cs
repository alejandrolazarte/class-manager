using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_a_passkey_is_added_with_a_wrong_password;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(signUp.AccessToken);

        using var response = await owner.PostRegistrationOptionsAsync(AuthenticationRequests.WrongPassword);

        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(AuthenticationErrorCodes.InvalidCurrentPassword);
    }
}
