using ClassManager.Core.Common;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Fees;

public sealed class Payment : ITenantOwned
{
    public const int NotesMaxLength = 200;
    public const int MonthsAheadAllowed = 12;

    public static readonly DateOnly EarliestMonth = new(2000, 1, 1);

    private const string PaidOnInFutureMessage = "The payment date can't be in the future.";
    private const string MonthRangeMessage = "The month must be between 2000-01 and 12 months from now.";
    private const string NotesLengthMessage = "Notes must be at most 200 characters.";
    private const string MethodRequiredMessage = "Choose a payment method.";

    private Payment()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly Month { get; private set; }
    public DateOnly PaidOn { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public BillingMonth BillingMonth => BillingMonth.From(Month);

    public static Result<Payment> Create(
        Guid clientId,
        decimal? amount,
        BillingMonth month,
        DateOnly paidOn,
        PaymentMethod? method,
        string? notes,
        DateOnly today,
        DateTimeOffset createdAt)
    {
        var amountValidation = MonthlyFee.Validate(amount ?? 0, nameof(Amount));
        if (amountValidation.IsFailure)
        {
            return amountValidation.Error!;
        }

        if (month.FirstDay < EarliestMonth || month.FirstDay > BillingMonth.From(today).AddMonths(MonthsAheadAllowed).FirstDay)
        {
            return Result.Validation<Payment>(MonthRangeMessage, fieldName: nameof(Month));
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
            Id = Guid.CreateVersion7(),
            ClientId = clientId,
            Amount = amount!.Value,
            Month = month.FirstDay,
            PaidOn = paidOn,
            Method = method.Value,
            Notes = trimmedNotes,
            CreatedAt = createdAt.ToUniversalTime(),
        };
    }
}
