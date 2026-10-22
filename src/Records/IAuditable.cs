namespace ClassManager.Records;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }

    long? CreatedByUserId { get; }

    DateTimeOffset UpdatedAt { get; }

    long? UpdatedByUserId { get; }
}
