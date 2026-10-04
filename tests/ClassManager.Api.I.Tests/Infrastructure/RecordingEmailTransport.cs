using System.Collections.Concurrent;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed class RecordingEmailTransport : IEmailTransport
{
    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);

    private readonly ConcurrentQueue<OutgoingEmail> _sentEmails = new();
    private TaskCompletionSource _deliveryGate = CompletedGate();

    public async Task SendAsync(OutgoingEmail email, CancellationToken cancellationToken)
    {
        await _deliveryGate.Task.WaitAsync(cancellationToken);
        _sentEmails.Enqueue(email);
    }

    public IReadOnlyList<OutgoingEmail> SentTo(string email) =>
        [.. _sentEmails.Where(sentEmail => string.Equals(sentEmail.To, email, StringComparison.OrdinalIgnoreCase))];

    public async Task<OutgoingEmail> WaitForEmailToAsync(string email, Func<OutgoingEmail, bool> matches)
    {
        var deadline = DateTimeOffset.UtcNow + WaitTimeout;
        while (DateTimeOffset.UtcNow < deadline)
        {
            if (SentTo(email).LastOrDefault(matches) is { } sentEmail)
            {
                return sentEmail;
            }

            await Task.Delay(PollInterval);
        }

        throw new TimeoutException($"No matching email reached {email}.");
    }

    public IDisposable HoldDeliveries()
    {
        _deliveryGate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        return new DeliveryRelease(this);
    }

    private static TaskCompletionSource CompletedGate()
    {
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        gate.SetResult();
        return gate;
    }

    private sealed class DeliveryRelease(RecordingEmailTransport transport) : IDisposable
    {
        public void Dispose() => transport._deliveryGate.TrySetResult();
    }
}
