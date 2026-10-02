using Microsoft.Extensions.Logging;

namespace ClassManager.Infrastructure.Email;

internal sealed partial class LoggingEmailTransport(ILogger<LoggingEmailTransport> logger) : IEmailTransport
{
    public Task SendAsync(OutgoingEmail email, CancellationToken cancellationToken)
    {
        LogNotSent(logger, email.To, email.Subject);
        LogBody(logger, email.TextBody);
        return Task.CompletedTask;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Email:Smtp is not configured. Email to {To} not sent: {Subject}")]
    private static partial void LogNotSent(ILogger logger, string to, string subject);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Email body:\n{Body}")]
    private static partial void LogBody(ILogger logger, string body);
}
