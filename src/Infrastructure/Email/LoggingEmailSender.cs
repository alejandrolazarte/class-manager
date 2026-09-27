using ClassManager.Core.Abstractions.Email;
using Microsoft.Extensions.Logging;

namespace ClassManager.Infrastructure.Email;

internal sealed partial class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        LogNotSent(logger, message.To, message.Subject);
        LogBody(logger, message.TextBody);
        return Task.CompletedTask;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Email:Smtp is not configured. Email to {To} not sent: {Subject}")]
    private static partial void LogNotSent(ILogger logger, string to, string subject);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Email body:\n{Body}")]
    private static partial void LogBody(ILogger logger, string body);
}
