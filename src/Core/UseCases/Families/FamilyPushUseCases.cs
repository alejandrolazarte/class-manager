using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.UseCases.Families;

public sealed record GetFamilyPushKeyQuery;

public sealed record FamilyPushKeyResponse(string? PublicKey);

public sealed record SavePushSubscriptionCommand(string? Endpoint, string? P256dh, string? Auth);

public sealed record RemovePushSubscriptionCommand(string? Endpoint);

public sealed class GetFamilyPushKeyUseCase(IFamilyAccess familyAccess, IWebPushKeyProvider keyProvider)
    : IUseCase<GetFamilyPushKeyQuery, FamilyPushKeyResponse>
{
    public async Task<Result<FamilyPushKeyResponse>> ExecuteAsync(GetFamilyPushKeyQuery command, CancellationToken cancellationToken) =>
        await familyAccess.GetAsync(cancellationToken) is null
            ? FamilyFailures.NoAccess()
            : new FamilyPushKeyResponse(keyProvider.PublicKey);
}

public sealed class SavePushSubscriptionUseCase(
    IFamilyAccess familyAccess,
    IPushSubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<SavePushSubscriptionCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(SavePushSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var access = await familyAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return FamilyFailures.NoAccess();
        }

        var subscription = PushSubscription.Create(
            access.ClientId, access.UserId, command.Endpoint, command.P256dh, command.Auth, timeProvider.GetUtcNow());
        if (subscription.IsFailure)
        {
            return subscription.Error!;
        }

        var existing = await subscriptionRepository.FindByEndpointAsync(subscription.Value!.Endpoint, cancellationToken);
        if (existing is not null)
        {
            subscriptionRepository.Remove(existing);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        subscriptionRepository.Add(subscription.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class RemovePushSubscriptionUseCase(
    IFamilyAccess familyAccess,
    IPushSubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<RemovePushSubscriptionCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(RemovePushSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var access = await familyAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return FamilyFailures.NoAccess();
        }

        var existing = string.IsNullOrWhiteSpace(command.Endpoint)
            ? null
            : await subscriptionRepository.FindByEndpointAsync(command.Endpoint, cancellationToken);
        if (existing is not null && existing.UserId == access.UserId)
        {
            subscriptionRepository.Remove(existing);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return true;
    }
}
