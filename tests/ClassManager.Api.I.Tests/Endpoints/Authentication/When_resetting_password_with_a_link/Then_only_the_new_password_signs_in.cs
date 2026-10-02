using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_resetting_password_with_a_link;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_the_new_password_signs_in(ApiFixture fixture)
{
    private const string NewPassword = "a brand new passphrase";

    [Fact]
    public async Task Then_only_the_new_password_signs_in_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        var tokens = await client.SignUpAsync(command);
        var link = await client.RequestPasswordResetLinkAsync(fixture.ApiFactory.EmailTransport, command.Email!);

        using var resetResponse = await client.PostPasswordResetAsync(AuthenticationRequests.TokenOf(link), NewPassword);

        resetResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        link.ShouldStartWith(AuthenticationRequests.WebAppResetPasswordUrl);
        using var oldPasswordResponse = await client.PostSignInAsync(command.Email!, AuthenticationRequests.OwnerPassword);
        oldPasswordResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        using var newPasswordResponse = await client.PostSignInAsync(command.Email!, NewPassword);
        newPasswordResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        using var oldSessionResponse = await client.PostRefreshAsync(tokens.RefreshToken);
        (await oldSessionResponse.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(AuthenticationErrorCodes.InvalidRefreshToken);
    }
}
