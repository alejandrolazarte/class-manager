using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record PaymentResponse(
    long Id,
    long ClientId,
    decimal Amount,
    string CurrencyCode,
    string Month,
    DateOnly PaidOn,
    PaymentMethod Method,
    string? Notes,
    long? RecordedByUserId = null,
    string? RecordedByFullName = null)
{
    public static PaymentResponse From(Payment payment, string? recordedByFullName = null) =>
        new(
            payment.Id,
            payment.ClientId,
            payment.Amount,
            payment.CurrencyCode,
            payment.BillingMonth.ToString(),
            payment.PaidOn,
            payment.Method,
            payment.Notes,
            payment.RecordedByUserId,
            recordedByFullName);
}
