using ClassManager.Core.Abstractions.Email;
using Microsoft.Extensions.Configuration;

namespace ClassManager.Infrastructure.Email;

internal sealed class AppFeedbackInbox(IConfiguration configuration) : IAppFeedbackInbox
{
    public const string AppFeedbackRecipientSetting = "AppFeedback:Recipient";

    private const string MissingRecipientMessage = "Set " + AppFeedbackRecipientSetting + " to the address that receives app feedback.";

    public string Address =>
        configuration[AppFeedbackRecipientSetting] is { Length: > 0 } recipient
            ? recipient
            : throw new InvalidOperationException(MissingRecipientMessage);
}
