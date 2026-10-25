using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Passkeys;

namespace ClassManager.Core.U.Tests.UseCases.Passkeys.When_BeginPasskeyRegistration_has_no_password;

public sealed class Then_validation_fails
{
    [Fact]
    public async Task Then_validation_fails_Run()
    {
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.AccountId).Returns(Guid.CreateVersion7());
        var passkeys = new Mock<IPasskeyIdentityService>();
        var useCase = new BeginPasskeyRegistrationUseCase(currentUser.Object, passkeys.Object);

        var response = await useCase.ExecuteAsync(new BeginPasskeyRegistrationCommand(string.Empty), CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(BeginPasskeyRegistrationCommand.CurrentPassword));
        passkeys.VerifyNoOtherCalls();
    }
}
