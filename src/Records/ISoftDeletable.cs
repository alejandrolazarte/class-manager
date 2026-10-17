namespace ClassManager.Records;

public interface ISoftDeletable
{
    bool IsDeleted { get; }

    DateTimeOffset? DeletedAt { get; }

    Guid? DeletedByUserId { get; }

    void Delete(DateTimeOffset deletedAt);
}
