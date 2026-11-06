using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.UseCases.Notifications;
using ClassManager.Tenancy;

namespace ClassManager.Core.U.Tests.UseCases.Notifications;

internal sealed class NotificationPreferencesUseCaseBuilder
{
    public Guid AccountId { get; } = Guid.CreateVersion7();

    public long BusinessId { get; } = TestIds.Next();

    public InMemoryNotificationOptOutRepository OptOuts { get; } = new();

    public Mock<IBranchDirectory> Branches { get; } = new();

    public Mock<IStudentAppDirectory> StudentApps { get; } = new();

    public NotificationPreferencesUseCaseBuilder AsTeamMember()
    {
        Branches
            .Setup(directory => directory.FindAsync(AccountId, BusinessId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BranchAccess(BusinessId, TestData.BusinessName, TestIds.Next(), null, IsBrandOwner: true));
        return this;
    }

    public NotificationPreferencesUseCaseBuilder WithFamilyAccess()
    {
        StudentApps
            .Setup(directory => directory.FindAsync(AccountId, BusinessId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StudentAppLink(BusinessId, TestData.BusinessName, TestIds.Next()));
        return this;
    }

    public ListMyNotificationPreferencesUseCase BuildList() =>
        new(CurrentUser(), TenantContext(), Branches.Object, StudentApps.Object, OptOuts);

    public UpdateMyNotificationPreferenceUseCase BuildUpdate() =>
        new(CurrentUser(), OptOuts, Mock.Of<IUnitOfWork>());

    private ICurrentUser CurrentUser()
    {
        var currentUser = new Mock<ICurrentUser>();
        currentUser.Setup(user => user.AccountId).Returns(AccountId);
        return currentUser.Object;
    }

    private ITenantContext TenantContext()
    {
        var tenantContext = new Mock<ITenantContext>();
        tenantContext.Setup(context => context.TenantId).Returns(BusinessId);
        return tenantContext.Object;
    }
}
