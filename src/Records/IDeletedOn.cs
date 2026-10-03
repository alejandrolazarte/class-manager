namespace ClassManager.Records;

public interface IDeletedOn
{
    DateTimeOffset? DeletedOn { get; }

    void Delete(DateTimeOffset deletedOn);
}
