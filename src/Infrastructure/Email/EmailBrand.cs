namespace ClassManager.Infrastructure.Email;

internal sealed record EmailBrand(string DisplayName, string? ThemeColor, string? AccentColor, EmailInlineImage? Logo, bool IsBusiness)
{
    public const string AppDisplayName = "Class Manager";
    public const string LogoContentId = "brand-logo@class-manager";

    public static readonly EmailBrand App = new(AppDisplayName, null, null, null, IsBusiness: false);
}
