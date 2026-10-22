using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassPacks;

public sealed class ClassPackPurchase : ITenantOwned, IAuditable, ISoftDeletable
{
    public const int NotesMaxLength = 200;

    private const string InactivePackMessage = "This pack is no longer sold.";
    private const string PurchasedOnInFutureMessage = "The sale date can't be in the future.";
    private const string MethodRequiredMessage = "Choose a payment method.";
    private const string NotesLengthMessage = "Notes must be at most 200 characters.";

    private ClassPackPurchase()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long ClientId { get; private set; }
    public long ClassPackId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int ClassCount { get; private set; }
    public decimal Price { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public DateOnly PurchasedOn { get; private set; }
    public DateOnly? ExpiresOn { get; private set; }
    public PaymentMethod? Method { get; private set; }
    public string? Notes { get; private set; }
    public int? ClassDurationMinutes { get; private set; }
    public long? TrialLessonId { get; private set; }
    public long? RecordedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Result<ClassPackPurchase> Sell(
        long clientId,
        ClassPack pack,
        decimal? price,
        string currencyCode,
        DateOnly purchasedOn,
        PaymentMethod? method,
        string? notes,
        DateOnly today,
        DateTimeOffset createdAt,
        long? recordedByUserId = null)
    {
        if (!pack.IsActive)
        {
            return Result.Validation<ClassPackPurchase>(InactivePackMessage, fieldName: nameof(ClassPackId));
        }

        var chargedPrice = price ?? pack.Price;
        var priceValidation = MonthlyFee.Validate(chargedPrice, nameof(Price));
        if (priceValidation.IsFailure)
        {
            return priceValidation.Error!;
        }

        if (purchasedOn > today)
        {
            return Result.Validation<ClassPackPurchase>(PurchasedOnInFutureMessage, fieldName: nameof(PurchasedOn));
        }

        if (method is null)
        {
            return Result.Validation<ClassPackPurchase>(MethodRequiredMessage, fieldName: nameof(Method));
        }

        return Create(clientId, pack, chargedPrice, currencyCode, purchasedOn, method, notes, createdAt, recordedByUserId);
    }

    public static Result<ClassPackPurchase> ForOrder(
        long clientId,
        ClassPack pack,
        decimal price,
        string currencyCode,
        DateOnly purchasedOn,
        PaymentMethod? method,
        string? notes,
        DateTimeOffset createdAt,
        long? recordedByUserId) =>
        Create(clientId, pack, price, currencyCode, purchasedOn, method, notes, createdAt, recordedByUserId);

    public void DeductTrial(long trialLessonId) => TrialLessonId = trialLessonId;

    public void KeepUsedClasses(int usedClasses, decimal refundedAmount)
    {
        ClassCount = usedClasses;
        Price -= refundedAmount;
    }

    public bool IsValidOn(DateOnly date) => ExpiresOn is null || ExpiresOn >= date;

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }

    private static Result<ClassPackPurchase> Create(
        long clientId,
        ClassPack pack,
        decimal price,
        string currencyCode,
        DateOnly purchasedOn,
        PaymentMethod? method,
        string? notes,
        DateTimeOffset createdAt,
        long? recordedByUserId)
    {
        var trimmedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (trimmedNotes?.Length > NotesMaxLength)
        {
            return Result.Validation<ClassPackPurchase>(NotesLengthMessage, fieldName: nameof(Notes));
        }

        return new ClassPackPurchase
        {
            ClientId = clientId,
            ClassPackId = pack.Id,
            Name = pack.Name,
            ClassCount = pack.ClassCount,
            Price = price,
            CurrencyCode = currencyCode,
            PurchasedOn = purchasedOn,
            ExpiresOn = pack.ValidityMonths is null ? null : purchasedOn.AddMonths(pack.ValidityMonths.Value).AddDays(-1),
            Method = method,
            Notes = trimmedNotes,
            ClassDurationMinutes = pack.ClassDurationMinutes,
            RecordedByUserId = recordedByUserId,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }
}
