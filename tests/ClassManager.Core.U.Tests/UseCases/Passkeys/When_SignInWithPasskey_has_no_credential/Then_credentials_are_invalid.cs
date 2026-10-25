using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Passkeys;

namespace ClassManager.Core.U.Tests.UseCases.Passkeys.When_SignInWithPasskey_has_no_credential;

public sealed class Then_credentials_are_invalid
{
    [Fact]
    public async Task Then_credentials_are_invalid_Run()
    {
        var passkeys = new Mock<IPasskeyIdentityService>();
        var useCase = new SignInWithPasskeyUseCase(passkeys.Object, new Mock<ISignInSessionService>().Object);

        var response = await useCase.ExecuteAsync(new SignInWithPasskeyCommand("ceremony", null), CancellationToken.None);

        response.Error!.Code.ShouldBe(AuthenticationErrorCodes.InvalidCredentials);
        passkeys.VerifyNoOtherCalls();
    }
}
