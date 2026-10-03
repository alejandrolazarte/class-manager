namespace ClassManager.Records;

public static class RecordExtensions
{
    public static DateOnly CreatedDate(this ICreatedOn record)
    {
        ArgumentNullException.ThrowIfNull(record);
        return DateOnly.FromDateTime(record.CreatedOn.UtcDateTime);
    }

    public static bool IsCurrent(this IDeletedOn record)
    {
        ArgumentNullException.ThrowIfNull(record);
        return record.DeletedOn is null;
    }

    public static bool HasExpiredOn(this IExpiredOn record, DateOnly date)
    {
        ArgumentNullException.ThrowIfNull(record);
        return record.ExpiredOn is { } expiredOn && date > expiredOn;
    }

    public static bool IsActiveOn<TRecord>(this TRecord record, DateOnly date)
        where TRecord : ICreatedOn, IDeletedOn, IExpiredOn =>
        record.IsCurrent() && record.CreatedDate() <= date && !record.HasExpiredOn(date);

    public static IQueryable<TRecord> WhereCurrent<TRecord>(this IQueryable<TRecord> records)
        where TRecord : class, IDeletedOn =>
        records.Where(record => record.DeletedOn == null);
}
