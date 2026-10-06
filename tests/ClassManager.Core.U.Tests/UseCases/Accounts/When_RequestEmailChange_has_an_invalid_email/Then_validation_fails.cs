using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_RequestEmailChange_has_an_invalid_email;

public sealed class Then_validation_fails
{
    [Fact]
    public async Task Then_validation_fails_Run()
    {
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.UserId).Returns(Guid.CreateVersion7());
        var useCase = new RequestEmailChangeUseCase(
            currentUser.Object, new Mock<IIdentityService>().Object, new Mock<IEmailSender>().Object, new Mock<IWebAppLinks>().Object);

        var response = await useCase.ExecuteAsync(new RequestEmailChangeCommand("not-an-email", "a long passphrase"), CancellationToken.None);

        response.Error!.FieldName.ShouldBe(nameof(RequestEmailChangeCommand.NewEmail));
    }
}
