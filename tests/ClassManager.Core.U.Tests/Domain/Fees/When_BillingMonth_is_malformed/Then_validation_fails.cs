using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_BillingMonth_is_malformed;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var month = BillingMonth.Parse("09/2026");

        month.Error!.Kind.ShouldBe(ErrorKind.Validation);
    }
}
