using System.Collections.Concurrent;
using ClassManager.Core.Abstractions.Email;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed class RecordingEmailSender : IEmailSender
{
    private readonly ConcurrentQueue<EmailMessage> _sentMessages = new();

    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        _sentMessages.Enqueue(message);
        return Task.CompletedTask;
    }

    public IReadOnlyList<EmailMessage> SentTo(string email) =>
        [.. _sentMessages.Where(message => string.Equals(message.To, email, StringComparison.OrdinalIgnoreCase))];
}
