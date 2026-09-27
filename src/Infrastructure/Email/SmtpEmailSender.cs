using ClassManager.Core.Abstractions.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ClassManager.Infrastructure.Email;

internal sealed class SmtpEmailSender(IOptions<SmtpOptions> options) : IEmailSender
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        var smtp = options.Value;
        var mimeMessage = new MimeMessage
        {
            Subject = message.Subject,
            Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = message.TextBody },
        };
        mimeMessage.From.Add(new MailboxAddress(smtp.FromName, string.IsNullOrWhiteSpace(smtp.FromAddress) ? smtp.UserName : smtp.FromAddress));
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));

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
