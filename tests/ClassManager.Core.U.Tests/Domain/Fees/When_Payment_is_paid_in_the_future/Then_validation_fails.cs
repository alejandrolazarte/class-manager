using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_Payment_is_paid_in_the_future;

public sealed class Then_validation_fails
{
    [Fact]
    public void Then_validation_fails_Run()
    {
        var payment = Payment.Create(
            Guid.CreateVersion7(), 12000m, BillingMonth.From(TestData.Today), TestData.Today.AddDays(1), PaymentMethod.Cash, null, TestData.Today, TestData.Now);

        payment.Error!.FieldName.ShouldBe(nameof(Payment.PaidOn));
    }
}
