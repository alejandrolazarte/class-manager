using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

public sealed record SetMonthlyFeeRequest(
    decimal? Amount,
    string? EffectiveFrom = null,
    BusinessBillingMode BillingMode = BusinessBillingMode.MonthlyFee);
