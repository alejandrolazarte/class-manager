using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_ConfirmEmailChange_succeeds;

public sealed class Then_the_users_email_follows
{
    private const string Token = "change-token";
    private const string NewEmail = "laura.nueva@example.com";

    [Fact]
    public async Task Then_the_users_email_follows_Run()
    {
        var userId = Guid.CreateVersion7();
        var identity = new Mock<IIdentityService>();
        identity
            .Setup(service => service.ConfirmEmailChangeAsync(Token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConfirmedEmailChange(userId, TestData.OwnerEmail, NewEmail));
        identity
            .Setup(service => service.ListAccountsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        var clients = new Mock<IClientRepository>();
        var instructors = new Mock<IInstructorRepository>();
        var users = new Mock<IUserRepository>();
        var useCase = new ConfirmEmailChangeUseCase(identity.Object, clients.Object, instructors.Object, users.Object, new Mock<IEmailSender>().Object);

        await useCase.ExecuteAsync(new ConfirmEmailChangeCommand(Token), CancellationToken.None);

        users.Verify(repository => repository.ChangeEmailInEveryBusinessByAccountAsync(userId, NewEmail, It.IsAny<CancellationToken>()));
    }
}
