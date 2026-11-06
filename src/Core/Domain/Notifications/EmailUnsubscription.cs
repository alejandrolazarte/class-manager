namespace ClassManager.Core.Domain.Notifications;

public sealed record EmailUnsubscription(long BusinessId, Guid AccountId, NotificationCategory Category);
