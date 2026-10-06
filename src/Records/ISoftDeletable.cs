namespace ClassManager.Records;

public interface ISoftDeletable
{
    DateTimeOffset? DeletedOn { get; }

    bool IsDeleted { get; }

    void Delete(DateTimeOffset deletedOn);
}
