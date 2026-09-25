namespace ClassManager.Infrastructure.Persistence.Repositories;

internal static class LikePatterns
{
    public const string EscapeCharacter = "\\";

    private const char Wildcard = '%';

    public static string Contains(string value) => Wildcard + Escape(value) + Wildcard;

    public static string? StartsWith(string? value) => value is null ? null : Escape(value) + Wildcard;

    private static string Escape(string value) =>
        value
            .Replace(EscapeCharacter, EscapeCharacter + EscapeCharacter, StringComparison.Ordinal)
            .Replace("%", EscapeCharacter + "%", StringComparison.Ordinal)
            .Replace("_", EscapeCharacter + "_", StringComparison.Ordinal)
            .Replace("[", EscapeCharacter + "[", StringComparison.Ordinal);
}
