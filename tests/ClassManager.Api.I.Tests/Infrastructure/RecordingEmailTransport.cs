using System.Collections.Concurrent;
using ClassManager.Infrastructure.Email;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed class RecordingEmailTransport : IEmailTransport
{
    private readonly ConcurrentQueue<OutgoingEmail> _sentEmails = new();

    public Task SendAsync(OutgoingEmail email, CancellationToken cancellationToken)
    {
        _sentEmails.Enqueue(email);
        return Task.CompletedTask;
    }

    public IReadOnlyList<OutgoingEmail> SentTo(string email) =>
        [.. _sentEmails.Where(sentEmail => string.Equals(sentEmail.To, email, StringComparison.OrdinalIgnoreCase))];
}
