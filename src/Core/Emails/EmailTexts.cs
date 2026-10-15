using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.Emails;

public static class EmailTexts
{
    public static IEmailTexts Spanish { get; } = new SpanishEmailTexts();

    public static IEmailTexts English { get; } = new EnglishEmailTexts();

    public static IReadOnlyList<IEmailTexts> All { get; } = [Spanish, English];

    public static IEmailTexts For(string? language) =>
        All.FirstOrDefault(texts => texts.Language == language) ?? All.First(texts => texts.Language == AccountLanguage.Default);
}
