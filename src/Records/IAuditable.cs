namespace ClassManager.Records;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }

    Guid? CreatedByUserId { get; }

    DateTimeOffset UpdatedAt { get; }

    Guid? UpdatedByUserId { get; }
}
