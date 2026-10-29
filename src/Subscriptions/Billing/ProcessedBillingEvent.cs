namespace ClassManager.Subscriptions.Billing;

public sealed class ProcessedBillingEvent
{
    public const int EventIdMaxLength = 255;

    private ProcessedBillingEvent()
    {
    }

    public string EventId { get; private set; } = string.Empty;
    public DateTimeOffset ProcessedAt { get; private set; }

    public static ProcessedBillingEvent Create(string eventId, DateTimeOffset processedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventId);
        if (eventId.Length > EventIdMaxLength)
        {
            throw new ArgumentException($"Event id must be at most {EventIdMaxLength} characters.", nameof(eventId));
        }

        return new ProcessedBillingEvent
        {
            EventId = eventId,
            ProcessedAt = processedAt.ToUniversalTime(),
        };
    }
}
