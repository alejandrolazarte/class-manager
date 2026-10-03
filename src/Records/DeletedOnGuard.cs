namespace ClassManager.Records;

public static class DeletedOnGuard
{
    public const string AlreadyReplacedMessage = "The record was already replaced.";

    public static DateTimeOffset Delete(DateTimeOffset? currentDeletedOn, DateTimeOffset deletedOn) =>
        currentDeletedOn is null
            ? deletedOn.ToUniversalTime()
            : throw new InvalidOperationException(AlreadyReplacedMessage);
}
