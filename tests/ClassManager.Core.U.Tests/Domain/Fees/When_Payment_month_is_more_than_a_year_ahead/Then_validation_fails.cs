using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_Payment_month_is_more_than_a_year_ahead;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var farMonth = BillingMonth.From(TestData.Today.AddMonths(13));

        var payment = Payment.Create(
            Guid.CreateVersion7(), 12000m, farMonth, TestData.Today, PaymentMethod.Cash, null, TestData.Today, TestData.Now);

        payment.Error!.FieldName.ShouldBe(nameof(Payment.Month));
    }
}
