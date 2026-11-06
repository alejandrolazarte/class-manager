using ClassManager.Core.Domain.Notifications;

namespace ClassManager.Core.Abstractions.Notifications;

public interface IEmailUnsubscribeTokenService
{
    string Create(EmailUnsubscription unsubscription);

    EmailUnsubscription? Read(string? token);
}
