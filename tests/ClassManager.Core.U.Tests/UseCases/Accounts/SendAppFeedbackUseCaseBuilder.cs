using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Accounts;

namespace ClassManager.Core.U.Tests.UseCases.Accounts;

internal sealed class SendAppFeedbackUseCaseBuilder
{
    public const string AppFeedbackInboxAddress = "feedback@example.com";
    public const string SenderFullName = "Ana Pérez";

    public SendAppFeedbackUseCaseBuilder()
    {
        CurrentUser.Setup(user => user.AccountId).Returns(AccountId);
        Identity
            .Setup(service => service.ListAccountsAsync(It.Is<IReadOnlyCollection<Guid>>(ids => ids.Contains(AccountId)), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UserAccount(AccountId, TestData.OwnerEmail, SenderFullName)]);
        Users
            .Setup(repository => repository.FindByAccountAsync(AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(User.CreateForAccount(AccountId, SenderFullName, TestData.OwnerEmail, DateTimeOffset.UnixEpoch));
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(Business);
        CurrentMember
            .Setup(member => member.GetAccessAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MemberAccess(AccountId, null, Business.Id, BusinessRole.BranchOwner, null, false, new HashSet<string>()));
        AppFeedbackInbox.Setup(inbox => inbox.Address).Returns(AppFeedbackInboxAddress);
    }

    public Guid AccountId { get; } = Guid.CreateVersion7();

    public Business Business { get; } = TestData.Business();

    public Mock<ICurrentUser> CurrentUser { get; } = new();

    public Mock<IIdentityService> Identity { get; } = new();

    public Mock<IUserRepository> Users { get; } = new();

    public Mock<IBusinessRepository> Businesses { get; } = new();

    public Mock<ICurrentMember> CurrentMember { get; } = new();

    public Mock<IAppFeedbackInbox> AppFeedbackInbox { get; } = new();

    public Mock<IEmailSender> EmailSender { get; } = new();

    public SendAppFeedbackUseCase Build() =>
        new(CurrentUser.Object, Identity.Object, Users.Object, Businesses.Object, CurrentMember.Object, AppFeedbackInbox.Object, EmailSender.Object);
}
