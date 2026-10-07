using ClassManager.Core.Abstractions.Email;
using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.Email;

public sealed record SendEmailBackgroundTaskCommand(EmailMessage Message) : BackgroundTaskCommand(Message.BusinessId);
