using System.Text;
using ClassManager.Infrastructure.Email;
using ClassManager.Notifications.Email;

namespace ClassManager.EmailPreviews;

internal sealed record PreviewBrand(string Name, EmailBrand Brand);

internal static class PreviewBrands
{
    public const string BusinessName = "Escuela Brazada";

    private const string ThemeColor = "#5b3fa0";
    private const string AccentColor = "#efb062";
    private const string LogoContentType = "image/svg+xml";

    private const string LogoSvg =
        """
        <svg xmlns="http://www.w3.org/2000/svg" width="512" height="512" viewBox="0 0 100 100">
        <rect width="100" height="100" fill="#5b3fa0"/>
        <path d="M10 62 Q 25 50 40 62 T 70 62 T 100 62 V 100 H 0 V 62 Z" fill="#efb062"/>
        <circle cx="66" cy="32" r="12" fill="#efb062"/>
        <text x="12" y="46" font-family="Arial Black, sans-serif" font-size="30" font-weight="900" fill="white">B</text>
        </svg>
        """;

    private static readonly EmailInlineImage Logo = new(EmailBrand.LogoContentId, Encoding.UTF8.GetBytes(LogoSvg), LogoContentType);

    public static PreviewBrand App { get; } = new("app", BrandedEmailSender.AppBrand);

    public static IReadOnlyList<PreviewBrand> Business { get; } =
    [
        new("sin-marca", new EmailBrand(BusinessName, null, null, null)),
        new("color", new EmailBrand(BusinessName, ThemeColor, AccentColor, null)),
        new("logo", new EmailBrand(BusinessName, null, null, Logo)),
        new("color-y-logo", new EmailBrand(BusinessName, ThemeColor, AccentColor, Logo)),
    ];
}
