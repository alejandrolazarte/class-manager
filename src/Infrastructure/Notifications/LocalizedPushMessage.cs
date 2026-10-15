using ClassManager.Notifications.WebPush;

namespace ClassManager.Infrastructure.Notifications;

internal sealed record LocalizedPushMessage(PushMessage Default, IReadOnlyDictionary<string, PushMessage> ByLanguage)
{
    public static LocalizedPushMessage Build(Func<INotificationTexts, PushMessage> messageIn) =>
        new(
            messageIn(NotificationTexts.Spanish),
            NotificationTexts.All.ToDictionary(texts => texts.Language, messageIn));

    public static LocalizedPushMessage SameInEveryLanguage(PushMessage message) => Build(_ => message);

    public PushMessage For(string? language) =>
        language is not null && ByLanguage.TryGetValue(language, out var message) ? message : Default;
}
