using ClassManager.Core.Abstractions.Email;

namespace ClassManager.Infrastructure.Email;

internal sealed class BrandedEmailSender(EmailBrandReader brandReader, IEmailTransport transport) : IEmailSender
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        var brand = await brandReader.ReadAsync(message.BusinessId, cancellationToken);
        var email = new OutgoingEmail(
            message.To,
            message.Subject,
            brand.IsBusiness ? brand.DisplayName : null,
            message.TextBody,
            BrandedEmailHtml.Render(message.Content, brand),
            brand.Logo is null ? [] : [brand.Logo]);
        await transport.SendAsync(email, cancellationToken);
    }
}
