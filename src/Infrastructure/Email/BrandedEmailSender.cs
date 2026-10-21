using ClassManager.Core.Abstractions.Email;
using ClassManager.Notifications.Email;

namespace ClassManager.Infrastructure.Email;

internal sealed class BrandedEmailSender(EmailBrandReader brandReader, WebAppLinks webAppLinks, IEmailTransport transport) : IEmailSender
{
    public const string AppDisplayName = "Laztty";

    public static readonly EmailBrand AppBrand = new(AppDisplayName, null, null, null);

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        var businessBrand = await brandReader.ReadAsync(message.BusinessId, cancellationToken);
        var brand = businessBrand ?? AppBrand;
        var email = new OutgoingEmail(
            message.To,
            message.Subject,
            businessBrand?.DisplayName,
            message.TextBody,
            BrandedEmailHtml.Render(message.Content, brand, webAppLinks.WebFonts()),
            brand.Logo is null ? [] : [brand.Logo],
            message.ReplyTo);
        await transport.SendAsync(email, cancellationToken);
    }
}
