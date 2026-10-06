using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts.When_ConfirmEmailChange_succeeds;

public sealed class Then_client_and_instructor_emails_follow
{
    private const string Token = "change-token";
    private const string NewEmail = "laura.nueva@example.com";

    [Fact]
    public async Task Then_client_and_instructor_emails_follow_Run()
    {
        var userId = Guid.CreateVersion7();
        var identity = new Mock<IIdentityService>();
        identity
            .Setup(service => service.ConfirmEmailChangeAsync(Token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConfirmedEmailChange(userId, TestData.OwnerEmail, NewEmail));
        var clients = new Mock<IClientRepository>();
        var instructors = new Mock<IInstructorRepository>();
        var useCase = new ConfirmEmailChangeUseCase(identity.Object, clients.Object, instructors.Object, new Mock<IEmailSender>().Object);

        await useCase.ExecuteAsync(new ConfirmEmailChangeCommand(Token), CancellationToken.None);

        clients.Verify(repository => repository.ChangeEmailInEveryBusinessByAccountUserAsync(userId, NewEmail, It.IsAny<CancellationToken>()));
        instructors.Verify(repository => repository.ChangeEmailInEveryBusinessByMemberUserAsync(userId, NewEmail, It.IsAny<CancellationToken>()));
    }
}
