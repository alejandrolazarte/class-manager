using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

internal static class FeeRules
{
    public static FeeStatus StatusOf(decimal? fee, decimal paid) => fee switch
    {
        null => FeeStatus.NoFee,
        _ when paid >= fee => FeeStatus.Paid,
        _ when paid > 0 => FeeStatus.Partial,
        _ => FeeStatus.Unpaid,
    };
}
