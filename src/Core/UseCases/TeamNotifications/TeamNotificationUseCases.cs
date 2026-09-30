using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Core.UseCases.TeamNotifications;

public sealed record GetTeamNotificationsQuery;

public sealed record MarkTeamNotificationsSeenCommand;

public sealed record TeamNotificationResponse(Guid Id, string Title, string Body, string Url, DateTimeOffset CreatedAt, bool IsUnread)
{
    public static TeamNotificationResponse From(TeamNotification notification) =>
        new(notification.Id, notification.Title, notification.Body, notification.Url, notification.CreatedAt, notification.SeenAt is null);
}

public sealed record TeamNotificationsResponse(IReadOnlyList<TeamNotificationResponse> Items, int UnreadCount);

public sealed class GetTeamNotificationsUseCase(ICurrentMember currentMember, ITeamNotificationRepository notificationRepository)
    : IUseCase<GetTeamNotificationsQuery, TeamNotificationsResponse>
{
    public const int Limit = 50;

    public async Task<Result<TeamNotificationsResponse>> ExecuteAsync(GetTeamNotificationsQuery command, CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (access is null)
        {
            return MemberRules.MemberNotFound();
        }

        var notifications = await notificationRepository.ListLatestAsync(access.UserId, Limit, cancellationToken);
        var unreadCount = await notificationRepository.CountUnseenAsync(access.UserId, cancellationToken);
        return new TeamNotificationsResponse([.. notifications.Select(TeamNotificationResponse.From)], unreadCount);
    }
}

public sealed class MarkTeamNotificationsSeenUseCase(
    ICurrentMember currentMember,
    ITeamNotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IUseCase<MarkTeamNotificationsSeenCommand, bool>
{
    public async Task<Result<bool>> ExecuteAsync(MarkTeamNotificationsSeenCommand command, CancellationToken cancellationToken)
    {
        var access = await currentMember.GetAccessAsync(cancellationToken);
        if (access is null)
        {
            return MemberRules.MemberNotFound();
        }

        var now = timeProvider.GetUtcNow();
        foreach (var notification in await notificationRepository.ListUnseenForUpdateAsync(access.UserId, cancellationToken))
        {
            notification.MarkSeen(now);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
