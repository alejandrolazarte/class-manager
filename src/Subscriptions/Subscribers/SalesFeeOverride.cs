using ClassManager.Records;
using ClassManager.Subscriptions.Catalog;

namespace ClassManager.Subscriptions.Subscribers;

public sealed class SalesFeeOverride : ICreatedOn, ISoftDeletable
{
    private SalesFeeOverride()
    {
    }

    public long Id { get; private set; }
    public long SubscriberId { get; private set; }
    public decimal Percent { get; private set; }
    public string? Note { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }

    public static SalesFeeOverride Create(long subscriberId, decimal percent, string? note, DateTimeOffset createdOn)
    {
        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (trimmedNote is { Length: > CatalogRules.NoteMaxLength })
        {
            throw new ArgumentException($"Note must be at most {CatalogRules.NoteMaxLength} characters.", nameof(note));
        }

        return new SalesFeeOverride
        {
            SubscriberId = subscriberId,
            Percent = CatalogRules.RequirePercent(percent, nameof(percent)),
            Note = trimmedNote,
            CreatedOn = createdOn.ToUniversalTime(),
        };
    }

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }
}
