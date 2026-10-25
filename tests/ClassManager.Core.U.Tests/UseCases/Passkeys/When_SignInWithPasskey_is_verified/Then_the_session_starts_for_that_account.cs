using System.Text.Json;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Passkeys;

namespace ClassManager.Core.U.Tests.UseCases.Passkeys.When_SignInWithPasskey_is_verified;

public sealed class Then_the_session_starts_for_that_account
{
    private const string CeremonyId = "ceremony";

    [Fact]
    public async Task Then_the_session_starts_for_that_account_Run()
    {
        var verification = CredentialVerification.Verified(Guid.CreateVersion7(), TestData.OwnerEmail);
        var passkeys = new Mock<IPasskeyIdentityService>();
        passkeys
            .Setup(service => service.VerifyPasskeyAsync(CeremonyId, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(verification);
        var sessions = new Mock<ISignInSessionService>();
        sessions
            .Setup(service => service.StartAsync(verification, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TokenResponse.From(TestData.IssuedTokens()));
        var useCase = new SignInWithPasskeyUseCase(passkeys.Object, sessions.Object);

        var response = await useCase.ExecuteAsync(
            new SignInWithPasskeyCommand(CeremonyId, JsonSerializer.SerializeToElement(new { id = "credential" })),
            CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        sessions.Verify(service => service.StartAsync(verification, It.IsAny<CancellationToken>()), Times.Once);
    }
}
