using ClassManager.Core.Abstractions.Email;
using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.Email;

internal sealed class SendEmailTaskHandler(IEmailSender emailSender) : IBackgroundTaskHandler<SendEmailTask>
{
    public Task HandleAsync(SendEmailTask command, CancellationToken cancellationToken) =>
        emailSender.SendAsync(command.Message, cancellationToken);
}
