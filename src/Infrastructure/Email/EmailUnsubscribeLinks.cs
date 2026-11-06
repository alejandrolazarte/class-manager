using ClassManager.Core.Abstractions.Email;
using ClassManager.Core.Abstractions.Notifications;
using ClassManager.Core.Emails;
using ClassManager.Notifications.Email;
using Microsoft.Extensions.Configuration;

namespace ClassManager.Infrastructure.Email;

internal sealed class EmailUnsubscribeLinks(IEmailUnsubscribeTokenService tokens, IWebAppLinks webAppLinks, IConfiguration configuration)
{
    public const string ApiPublicUrlSetting = "Api:PublicUrl";
    public const string OneClickPath = "/api/email-unsubscribe?token=";

    public EmailMessage Add(EmailMessage message, long businessId, Guid? accountId, NotificationCategory category, IEmailTexts texts)
    {
        if (accountId is not { } recipientAccountId)
        {
            return message;
        }

        var token = tokens.Create(new EmailUnsubscription(businessId, recipientAccountId, category));
        var apiPublicUrl = configuration[ApiPublicUrlSetting];
        return message with
        {
            Content = message.Content with
            {
                Unsubscribe = new EmailAction(texts.StopTheseEmails, webAppLinks.Unsubscribe(token)),
            },
            OneClickUnsubscribeUrl = string.IsNullOrWhiteSpace(apiPublicUrl) ? null : apiPublicUrl.TrimEnd('/') + OneClickPath + Uri.EscapeDataString(token),
        };
    }
}
