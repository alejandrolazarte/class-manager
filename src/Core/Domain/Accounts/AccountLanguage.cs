namespace ClassManager.Core.Domain.Accounts;

public static class AccountLanguage
{
    public const string Spanish = "es";
    public const string English = "en";
    public const string Default = Spanish;

    public static IReadOnlyList<string> Supported { get; } = [Spanish, English];

    public static bool IsSupported(string? language) => language is not null && Supported.Contains(language);
}
