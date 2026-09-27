using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_reusing_a_password_reset_link;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    private const string FirstNewPassword = "a brand new passphrase";
    private const string SecondNewPassword = "another new passphrase";

    [Fact]
    public async Task Then_returns_400_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        await client.SignUpAsync(command);
        var token = AuthenticationRequests.TokenOf(await client.RequestPasswordResetLinkAsync(fixture.ApiFactory.EmailSender, command.Email!));
        using var firstResponse = await client.PostPasswordResetAsync(token, FirstNewPassword);

        using var secondResponse = await client.PostPasswordResetAsync(token, SecondNewPassword);

        firstResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        secondResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await secondResponse.ReadProblemAsync()).GetProperty("code").GetString()
            .ShouldBe(AuthenticationErrorCodes.InvalidPasswordResetToken);
    }
}
