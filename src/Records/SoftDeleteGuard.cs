namespace ClassManager.Records;

public static class SoftDeleteGuard
{
    public const string AlreadyDeletedMessage = "The record was already deleted.";

    public static DateTimeOffset Delete(DateTimeOffset? currentDeletedAt, DateTimeOffset deletedAt) =>
        currentDeletedAt is null
            ? deletedAt.ToUniversalTime()
            : throw new InvalidOperationException(AlreadyDeletedMessage);
}
