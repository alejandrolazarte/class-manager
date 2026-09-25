using ClassManager.Core.Common;

namespace ClassManager.Core.Domain.Fees;

public static class MonthlyFee
{
    public const decimal MaximumAmount = 10_000_000m;
    public const int AmountDecimals = 2;
    public const int AmountPrecision = 12;

    private const string AmountRangeMessage = "The amount must be greater than 0 and at most 10,000,000, with at most 2 decimals.";

    public static bool IsValidAmount(decimal amount) =>
        amount > 0 && amount <= MaximumAmount && decimal.Round(amount, AmountDecimals) == amount;

    public static Result Validate(decimal? amount, string fieldName) =>
        amount is null || IsValidAmount(amount.Value)
            ? Result.Success()
            : Result.Validation(AmountRangeMessage, fieldName: fieldName);
}
