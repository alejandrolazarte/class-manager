using ClassManager.Core.Abstractions.Email;
using ClassManager.Infrastructure.BackgroundTasks;

namespace ClassManager.Infrastructure.Email;

internal sealed class SendEmailBackgroundTaskHandler(IEmailSender emailSender) : IBackgroundTaskHandler<SendEmailBackgroundTaskCommand>
{
    public Task HandleAsync(SendEmailBackgroundTaskCommand command, CancellationToken cancellationToken) =>
        emailSender.SendAsync(command.Message, cancellationToken);
}
