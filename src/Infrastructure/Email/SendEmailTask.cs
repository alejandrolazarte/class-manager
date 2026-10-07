using ClassManager.Core.Abstractions.Email;
using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.Email;

public sealed record SendEmailTask(EmailMessage Message) : BackgroundTaskCommand(Message.BusinessId)
{
    public const string TypeName = "email.send";
}
