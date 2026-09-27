namespace ClassManager.Core.U.Tests.Domain.ClassPacks.When_ClassPackPurchase_pack_lasts_two_months;

public sealed class Then_it_expires_the_day_before_two_months_later
{
    [Fact]
    public void Then_it_expires_the_day_before_two_months_later_Run()
    {
        var purchase = ClassPackTestData.Purchase(new DateOnly(2026, 9, 5), classCount: 8, validityMonths: 2);

        purchase.ExpiresOn.ShouldBe(new DateOnly(2026, 11, 4));
    }
}
