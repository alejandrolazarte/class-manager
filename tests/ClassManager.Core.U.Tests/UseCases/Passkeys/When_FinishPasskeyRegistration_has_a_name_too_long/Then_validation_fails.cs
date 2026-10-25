using System.Text.Json;
using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Passkeys;

namespace ClassManager.Core.U.Tests.UseCases.Passkeys.When_FinishPasskeyRegistration_has_a_name_too_long;

public sealed class Then_validation_fails
{
    [Fact]
    public async Task Then_validation_fails_Run()
    {
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.AccountId).Returns(Guid.CreateVersion7());
        var passkeys = new Mock<IPasskeyIdentityService>();
        var useCase = new FinishPasskeyRegistrationUseCase(
            currentUser.Object, passkeys.Object, new Mock<IIdentityService>().Object, new Mock<IEmailSender>().Object);
        var name = new string('a', FinishPasskeyRegistrationUseCase.NameMaxLength + 1);

        var response = await useCase.ExecuteAsync(
            new FinishPasskeyRegistrationCommand("ceremony", JsonSerializer.SerializeToElement(new { id = "credential" }), name),
            CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(FinishPasskeyRegistrationCommand.Name));
        passkeys.VerifyNoOtherCalls();
    }
}
