using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record GetStudentAppPushKeyQuery : IQuery;

public sealed record StudentAppPushKeyResponse(string? PublicKey);

public sealed record SavePushSubscriptionCommand(string? Endpoint, string? P256dh, string? Auth) : ICommand;

public sealed record RemovePushSubscriptionCommand(string? Endpoint) : ICommand;

public sealed class GetStudentAppPushKeyUseCase(IStudentAppAccess studentAppAccess, IWebPushKeyProvider keyProvider)
    : IUseCase<GetStudentAppPushKeyQuery, StudentAppPushKeyResponse>
{
    public async Task<Result<StudentAppPushKeyResponse>> ExecuteAsync(GetStudentAppPushKeyQuery command, CancellationToken cancellationToken) =>
        await studentAppAccess.GetAsync(cancellationToken) is null
            ? StudentAppFailures.NoAccess()
            : new StudentAppPushKeyResponse(keyProvider.PublicKey);
}

public sealed class SavePushSubscriptionUseCase(
    IStudentAppAccess studentAppAccess,
    IPushSubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<SavePushSubscriptionCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(SavePushSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return StudentAppFailures.NoAccess();
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
    IStudentAppAccess studentAppAccess,
    IPushSubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<RemovePushSubscriptionCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(RemovePushSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var access = await studentAppAccess.GetAsync(cancellationToken);
        if (access is null)
        {
            return StudentAppFailures.NoAccess();
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
