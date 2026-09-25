using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_in_fails_five_times;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_account_is_locked(ApiFixture fixture)
{
    private const int MaximumFailedAttempts = 5;

    [Fact]
    public async Task Then_account_is_locked_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        await client.SignUpAsync(command);
        for (var attempt = 0; attempt < MaximumFailedAttempts; attempt++)
        {
            using var failedResponse = await client.PostSignInAsync(command.Email!, AuthenticationRequests.WrongPassword);
            failedResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        using var response = await client.PostSignInAsync(command.Email!, AuthenticationRequests.OwnerPassword);

        response.StatusCode.ShouldBe(HttpStatusCode.Locked);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(AuthenticationErrorCodes.LockedOut);
    }
}
