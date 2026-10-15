using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Infrastructure.Notifications;

internal static class NotificationTexts
{
    public static INotificationTexts Spanish { get; } = new SpanishNotificationTexts();

    public static INotificationTexts English { get; } = new EnglishNotificationTexts();

    public static IReadOnlyList<INotificationTexts> All { get; } = [Spanish, English];

    public static INotificationTexts For(string? language) =>
        All.FirstOrDefault(texts => texts.Language == language) ?? All.First(texts => texts.Language == AccountLanguage.Default);
}
