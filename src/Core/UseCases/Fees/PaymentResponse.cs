using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record PaymentResponse(
    Guid Id,
    Guid ClientId,
    decimal Amount,
    string Month,
    DateOnly PaidOn,
    PaymentMethod Method,
    string? Notes,
    Guid? RecordedByUserId = null,
    string? RecordedByFullName = null)
{
    public static PaymentResponse From(Payment payment, string? recordedByFullName = null) =>
        new(
            payment.Id,
            payment.ClientId,
            payment.Amount,
            payment.BillingMonth.ToString(),
            payment.PaidOn,
            payment.Method,
            payment.Notes,
            payment.RecordedByUserId,
            recordedByFullName);
}
