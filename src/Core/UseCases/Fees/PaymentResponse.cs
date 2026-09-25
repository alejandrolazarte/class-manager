using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record PaymentResponse(
    Guid Id,
    Guid ClientId,
    decimal Amount,
    string Month,
    DateOnly PaidOn,
    PaymentMethod Method,
    string? Notes)
{
    public static PaymentResponse From(Payment payment) =>
        new(payment.Id, payment.ClientId, payment.Amount, payment.BillingMonth.ToString(), payment.PaidOn, payment.Method, payment.Notes);
}
