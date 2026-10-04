using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Core.UseCases.TeamNotifications;

public sealed record GetTeamPushKeyQuery : IQuery;

public sealed record TeamPushKeyResponse(string? PublicKey);

public sealed record SaveTeamPushSubscriptionCommand(string? Endpoint, string? P256dh, string? Auth) : ICommand;

public sealed record RemoveTeamPushSubscriptionCommand(string? Endpoint) : ICommand;

public sealed class GetTeamPushKeyUseCase(ICurrentMember currentMember, IWebPushKeyProvider keyProvider)
    : IUseCase<GetTeamPushKeyQuery, TeamPushKeyResponse>
{
    public async Task<Result<TeamPushKeyResponse>> ExecuteAsync(GetTeamPushKeyQuery command, CancellationToken cancellationToken) =>
        await currentMember.GetAccessAsync(cancellationToken) is null
            ? MemberRules.MemberNotFound()
            : new TeamPushKeyResponse(keyProvider.PublicKey);
}

public sealed class SaveTeamPushSubscriptionUseCase(
    ICurrentMember currentMember,
    IMemberPushSubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<SaveTeamPushSubscriptionCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(SaveTeamPushSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (access is null)
        {
            return MemberRules.MemberNotFound();
        }

        var subscription = MemberPushSubscription.Create(
            access.UserId, command.Endpoint, command.P256dh, command.Auth, timeProvider.GetUtcNow());
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

public sealed class RemoveTeamPushSubscriptionUseCase(
    ICurrentMember currentMember,
    IMemberPushSubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<RemoveTeamPushSubscriptionCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(RemoveTeamPushSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (access is null)
        {
            return MemberRules.MemberNotFound();
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
