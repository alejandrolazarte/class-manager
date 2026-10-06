using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Instructors;
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
        var client = TestData.Client();
        var instructor = Instructor.Create(TestData.InstructorFullName, TestData.OwnerEmail).Value!;
        var identity = new Mock<IIdentityService>();
        identity
            .Setup(service => service.ConfirmEmailChangeAsync(Token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConfirmedEmailChange(userId, TestData.OwnerEmail, NewEmail));
        var clients = new Mock<IClientRepository>();
        clients.Setup(repository => repository.ListForUpdateInAnyBusinessByAccountUserAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync([client]);
        var instructors = new Mock<IInstructorRepository>();
        instructors.Setup(repository => repository.ListForUpdateInAnyBusinessByMemberUserAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync([instructor]);
        var useCase = new ConfirmEmailChangeUseCase(
            identity.Object, clients.Object, instructors.Object, new Mock<IUnitOfWork>().Object, new Mock<IEmailSender>().Object);

        await useCase.ExecuteAsync(new ConfirmEmailChangeCommand(Token), CancellationToken.None);

        new[] { client.Email, instructor.Email }.ShouldAllBe(email => email == NewEmail);
    }
}
