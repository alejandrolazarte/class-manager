namespace ClassManager.Core.Domain.Notifications;

public static class NotificationErrorCodes
{
    public const string ChannelNotInCategory = "notifications.channel_not_in_category";
    public const string ChannelNotInCategoryMessage = "This kind of notification is not sent through that channel.";
    public const string InvalidUnsubscribeLink = "notifications.invalid_unsubscribe_link";
    public const string InvalidUnsubscribeLinkMessage = "The link to stop these emails is not valid.";
}
