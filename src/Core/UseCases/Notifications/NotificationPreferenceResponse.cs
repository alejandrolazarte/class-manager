using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.UseCases.Notifications;

public sealed record NotificationChannelPreferenceResponse(NotificationChannel Channel, bool IsEnabled);

public sealed record NotificationPreferenceResponse(
    NotificationCategory Category,
    NotificationAudience Audience,
    IReadOnlyList<NotificationChannelPreferenceResponse> Channels)
{
    public static NotificationPreferenceResponse From(NotificationCategoryDefinition definition, IEnumerable<NotificationOptOut> optOuts)
    {
        var turnedOff = optOuts
            .Where(optOut => optOut.Category == definition.Category)
            .Select(optOut => optOut.Channel)
            .ToHashSet();
        return new NotificationPreferenceResponse(
            definition.Category,
            definition.Audience,
            [.. definition.Channels.Select(channel => new NotificationChannelPreferenceResponse(channel, !turnedOff.Contains(channel)))]);
    }
}
