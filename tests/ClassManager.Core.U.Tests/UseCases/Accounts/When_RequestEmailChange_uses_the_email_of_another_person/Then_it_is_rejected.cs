using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_RequestEmailChange_uses_the_email_of_another_person;

public sealed class Then_it_is_rejected
{
    private const string NewEmail = "ana@example.com";

    [Fact]
    public async Task Then_it_is_rejected_Run()
    {
        var accountId = Guid.CreateVersion7();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.AccountId).Returns(accountId);
        var users = new Mock<IUserRepository>();
        users
            .Setup(repository => repository.IsEmailTakenByAnotherPersonOfTheAccountsBusinessesAsync(accountId, NewEmail, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var identity = new Mock<IIdentityService>();
        var useCase = new RequestEmailChangeUseCase(
            currentUser.Object, identity.Object, users.Object, new Mock<IEmailSender>().Object, new Mock<IWebAppLinks>().Object);

        var response = await useCase.ExecuteAsync(new RequestEmailChangeCommand(NewEmail, "a long passphrase"), CancellationToken.None);

        response.Error!.Code.ShouldBe(UserErrorCodes.EmailTaken);
        identity.Verify(service => service.RequestEmailChangeAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
