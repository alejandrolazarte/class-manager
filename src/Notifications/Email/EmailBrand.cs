namespace ClassManager.Notifications.Email;

public sealed record EmailBrand(string DisplayName, string? ThemeColor, string? AccentColor, EmailInlineImage? Logo)
{
    public const string LogoContentId = "brand-logo@email";
}
