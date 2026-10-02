using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ClassManager.Infrastructure.Email;

internal sealed class SmtpEmailTransport(IOptions<SmtpOptions> options) : IEmailTransport
{
    public async Task SendAsync(OutgoingEmail email, CancellationToken cancellationToken)
    {
        var smtp = options.Value;
        var body = new BodyBuilder { TextBody = email.TextBody, HtmlBody = email.HtmlBody };
        foreach (var image in email.InlineImages)
        {
            var linkedImage = body.LinkedResources.Add(image.ContentId, image.Content, ContentType.Parse(image.ContentType));
            linkedImage.ContentId = image.ContentId;
        }

        var mimeMessage = new MimeMessage { Subject = email.Subject, Body = body.ToMessageBody() };
        mimeMessage.From.Add(new MailboxAddress(
            email.FromName ?? smtp.FromName,
            string.IsNullOrWhiteSpace(smtp.FromAddress) ? smtp.UserName : smtp.FromAddress));
        mimeMessage.To.Add(MailboxAddress.Parse(email.To));

        using var client = new SmtpClient();
        await client.ConnectAsync(smtp.Host, smtp.Port, SecureSocketOptions.Auto, cancellationToken);
        if (!string.IsNullOrWhiteSpace(smtp.UserName))
        {
            await client.AuthenticateAsync(smtp.UserName, smtp.Password, cancellationToken);
        }

        await client.SendAsync(mimeMessage, cancellationToken);
        await client.DisconnectAsync(quit: true, cancellationToken);
    }
}
