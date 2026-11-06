using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;
using ClassManager.Tenancy;

namespace ClassManager.Core.U.Tests.UseCases.Notifications;

internal sealed class EmailUnsubscribeUseCaseBuilder
{
    public const string ValidToken = "valid-token";

    public EmailUnsubscription Unsubscription { get; } = new(TestIds.Next(), Guid.CreateVersion7(), NotificationCategory.Orders);

    public InMemoryNotificationOptOutRepository OptOuts { get; } = new();

    public Mock<ITenantScope> TenantScope { get; } = new();

    public Mock<IEmailUnsubscribeTokenService> Tokens { get; } = new();

    public Mock<IBusinessRepository> Businesses { get; } = new();

    public EmailUnsubscribeUseCaseBuilder()
    {
        Tokens.Setup(service => service.Read(ValidToken)).Returns(Unsubscription);
        Businesses.Setup(repository => repository.GetCurrentAsync(It.IsAny<CancellationToken>())).ReturnsAsync(TestData.Business());
    }

    public EmailUnsubscribeUseCaseBuilder ForCategory(NotificationCategory category)
    {
        Tokens.Setup(service => service.Read(ValidToken)).Returns(Unsubscription with { Category = category });
        return this;
    }

    public GetEmailUnsubscriptionUseCase BuildGet() => new(Tokens.Object, TenantScope.Object, Businesses.Object, OptOuts);

    public UnsubscribeFromEmailUseCase BuildUnsubscribe() => new(Tokens.Object, TenantScope.Object, Businesses.Object, OptOuts, Mock.Of<IUnitOfWork>());

    public ResubscribeToEmailUseCase BuildResubscribe() => new(Tokens.Object, TenantScope.Object, Businesses.Object, OptOuts, Mock.Of<IUnitOfWork>());
}
