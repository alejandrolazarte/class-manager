using ClassManager.Core.Common;
using ClassManager.Core.Domain.Fees;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.ClassPacks;

public sealed class ClassPackPurchase : ITenantOwned, ISoftDeletable
{
    public const int NotesMaxLength = 200;

    private const string InactivePackMessage = "This pack is no longer sold.";
    private const string PurchasedOnInFutureMessage = "The sale date can't be in the future.";
    private const string MethodRequiredMessage = "Choose a payment method.";
    private const string NotesLengthMessage = "Notes must be at most 200 characters.";

    private ClassPackPurchase()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid ClassPackId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int ClassCount { get; private set; }
    public decimal Price { get; private set; }
    public DateOnly PurchasedOn { get; private set; }
    public DateOnly? ExpiresOn { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? Notes { get; private set; }
    public int? ClassDurationMinutes { get; private set; }
    public Guid? TrialLessonId { get; private set; }
    public Guid? RecordedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Result<ClassPackPurchase> Sell(
        Guid clientId,
        ClassPack pack,
        decimal? price,
        DateOnly purchasedOn,
        PaymentMethod? method,
        string? notes,
        DateOnly today,
        DateTimeOffset createdAt,
        Guid? recordedByUserId = null)
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

        var trimmedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (trimmedNotes?.Length > NotesMaxLength)
        {
            return Result.Validation<ClassPackPurchase>(NotesLengthMessage, fieldName: nameof(Notes));
        }

        return new ClassPackPurchase
        {
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            ClassPackId = pack.Id,
            Name = pack.Name,
            ClassCount = pack.ClassCount,
            Price = chargedPrice,
            PurchasedOn = purchasedOn,
            ExpiresOn = pack.ValidityMonths is null ? null : purchasedOn.AddMonths(pack.ValidityMonths.Value).AddDays(-1),
            Method = method.Value,
            Notes = trimmedNotes,
            ClassDurationMinutes = pack.ClassDurationMinutes,
            RecordedByUserId = recordedByUserId,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public void DeductTrial(Guid trialLessonId) => TrialLessonId = trialLessonId;

    public void KeepUsedClasses(int usedClasses, decimal refundedAmount)
    {
        ClassCount = usedClasses;
        Price -= refundedAmount;
    }

    public bool IsValidOn(DateOnly date) => ExpiresOn is null || ExpiresOn >= date;

    public void Delete(DateTimeOffset deletedOn)
    {
        DeletedOn = DeletedOnGuard.Delete(DeletedOn, deletedOn);
        IsDeleted = true;
    }
}
