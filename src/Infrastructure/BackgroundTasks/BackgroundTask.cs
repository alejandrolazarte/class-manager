namespace ClassManager.Infrastructure.BackgroundTasks;

public sealed class BackgroundTask
{
    public const int TypeMaxLength = 100;
    public const int LastErrorMaxLength = 2000;

    public static readonly TimeSpan FirstRetryDelay = TimeSpan.FromMinutes(1);

    private const int RetryDelayGrowth = 4;

    private BackgroundTask()
    {
    }

    public Guid Id { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public Guid? TenantId { get; private set; }

    public int Attempts { get; private set; }

    public DateTimeOffset NextAttemptOn { get; private set; }

    public DateTimeOffset EnqueuedOn { get; private set; }

    public string? LastError { get; private set; }

    public DateTimeOffset? FailedOn { get; private set; }

    public static BackgroundTask Create(string type, string payload, Guid? tenantId, DateTimeOffset now) =>
        new()
        {
            Id = Guid.NewGuid(),
            Type = type,
            Payload = payload,
            TenantId = tenantId,
            NextAttemptOn = now,
            EnqueuedOn = now,
        };

    public void RecordFailure(string error, DateTimeOffset now, int maxAttempts)
    {
        if (Attempts >= maxAttempts)
        {
            Fail(error, now);
            return;
        }

        LastError = Shorten(error);
        NextAttemptOn = now + RetryDelayAfter(Attempts);
    }

    public void Fail(string error, DateTimeOffset now)
    {
        LastError = Shorten(error);
        FailedOn = now;
    }

    private static string Shorten(string error) =>
        error.Length > LastErrorMaxLength ? error[..LastErrorMaxLength] : error;

    private static TimeSpan RetryDelayAfter(int attempts) =>
        FirstRetryDelay * Math.Pow(RetryDelayGrowth, attempts - 1);
}
