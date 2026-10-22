namespace ClassManager.Records;

public interface ISoftDeletable
{
    bool IsDeleted { get; }

    DateTimeOffset? DeletedAt { get; }

    long? DeletedByUserId { get; }

    void Delete(DateTimeOffset deletedAt);
}
