using static ClassManager.Subscriptions.U.Tests.BillingTestData;

namespace ClassManager.Subscriptions.U.Tests.Billing.When_the_customer_resumes_before_the_end;

public sealed class Then_the_expiry_is_cleared
{
    [Fact]
    public void Then_the_expiry_is_cleared_Run()
    {
        var paying = Paying();
        paying.Expire(Today.AddDays(20));

        Sync(paying, External(cancelOn: null));

        paying.ExpiredOn.ShouldBeNull();
    }
}
