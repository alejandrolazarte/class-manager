using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_MonthlyFee_has_three_decimals;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var validation = MonthlyFee.Validate(12000.555m, "Amount");

        validation.Error!.FieldName.ShouldBe("Amount");
    }
}
