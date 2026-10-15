namespace ClassManager.Infrastructure.Notifications;

internal static class PushBodies
{
    private const int BodyMaxLength = 160;
    private const string Ellipsis = "…";

    public static string Shorten(string text) =>
        text.Length <= BodyMaxLength ? text : $"{text[..(BodyMaxLength - Ellipsis.Length)]}{Ellipsis}";
}
