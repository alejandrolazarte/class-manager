using ClassManager.Core.Common;
using ClassManager.Records;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Fees;

public sealed class Payment : ITenantOwned, IAuditable, ISoftDeletable
{
    public const int NotesMaxLength = 200;

    private const string PaidOnInFutureMessage = "The payment date can't be in the future.";
    private const string NotesLengthMessage = "Notes must be at most 200 characters.";
    private const string MethodRequiredMessage = "Choose a payment method.";

    private Payment()
    {
    }

    public long Id { get; private set; }
    public long TenantId { get; private set; }
    public long ClientId { get; private set; }
    public decimal Amount { get; private set; }
    public string CurrencyCode { get; private set; } = string.Empty;
    public DateOnly Month { get; private set; }
    public DateOnly PaidOn { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? Notes { get; private set; }
    public long? RecordedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public long? DeletedByUserId { get; private set; }
    public bool IsDeleted { get; private set; }

    public BillingMonth BillingMonth => BillingMonth.From(Month);

    public static Result<Payment> Create(
        long clientId,
        decimal? amount,
        string currencyCode,
        BillingMonth month,
        DateOnly paidOn,
        PaymentMethod? method,
        string? notes,
        DateOnly today,
        DateTimeOffset createdAt,
        long? recordedByUserId = null)
    {
        var amountValidation = MonthlyFee.Validate(amount ?? 0, nameof(Amount));
        if (amountValidation.IsFailure)
        {
            return amountValidation.Error!;
        }

        if (!month.IsWithinAllowedRange(today))
        {
            return Result.Validation<Payment>(BillingMonth.AllowedRangeMessage, fieldName: nameof(Month));
        }

        if (paidOn > today)
        {
            return Result.Validation<Payment>(PaidOnInFutureMessage, fieldName: nameof(PaidOn));
        }

        if (method is null)
        {
            return Result.Validation<Payment>(MethodRequiredMessage, fieldName: nameof(Method));
        }

        var trimmedNotes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        if (trimmedNotes?.Length > NotesMaxLength)
        {
            return Result.Validation<Payment>(NotesLengthMessage, fieldName: nameof(Notes));
        }

        return new Payment
        {
            ClientId = clientId,
            Amount = amount!.Value,
            CurrencyCode = currencyCode,
            Month = month.FirstDay,
            PaidOn = paidOn,
            Method = method.Value,
            Notes = trimmedNotes,
            RecordedByUserId = recordedByUserId,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }

    public void Delete(DateTimeOffset deletedAt)
    {
        DeletedAt = SoftDeleteGuard.Delete(DeletedAt, deletedAt);
        IsDeleted = true;
    }

    public void Restore()
    {
        DeletedAt = null;
        DeletedByUserId = null;
        IsDeleted = false;
    }
}
