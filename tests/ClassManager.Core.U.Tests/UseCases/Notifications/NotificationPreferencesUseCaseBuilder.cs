using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Notifications;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Core.U.Tests.UseCases.Notifications;

internal sealed class NotificationPreferencesUseCaseBuilder
{
    public Guid AccountId { get; } = Guid.CreateVersion7();

    public InMemoryNotificationOptOutRepository OptOuts { get; } = new();

    public Mock<IBranchDirectory> Branches { get; } = new();

    public Mock<IStudentAppDirectory> StudentApps { get; } = new();

    public NotificationPreferencesUseCaseBuilder()
    {
        Branches.Setup(directory => directory.ListAsync(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync([]);
        StudentApps.Setup(directory => directory.ListAsync(AccountId, It.IsAny<CancellationToken>())).ReturnsAsync([]);
    }

    public NotificationPreferencesUseCaseBuilder AsTeamMember()
    {
        Branches
            .Setup(directory => directory.ListAsync(AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new BranchAccess(TestIds.Next(), TestData.BusinessName, TestIds.Next(), null, IsBrandOwner: true)]);
        return this;
    }

    public NotificationPreferencesUseCaseBuilder WithFamilyAccess()
    {
        StudentApps
            .Setup(directory => directory.ListAsync(AccountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new StudentAppLink(TestIds.Next(), TestData.BusinessName, TestIds.Next())]);
        return this;
    }

    public ListMyNotificationPreferencesUseCase BuildList() =>
        new(CurrentUser(), Branches.Object, StudentApps.Object, OptOuts);

    public UpdateMyNotificationPreferenceUseCase BuildUpdate() =>
        new(CurrentUser(), OptOuts, Mock.Of<IUnitOfWork>(), new FakeTimeProvider(TestData.Now));

    private ICurrentUser CurrentUser()
    {
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.AccountId).Returns(AccountId);
        return currentUser.Object;
    }
}
