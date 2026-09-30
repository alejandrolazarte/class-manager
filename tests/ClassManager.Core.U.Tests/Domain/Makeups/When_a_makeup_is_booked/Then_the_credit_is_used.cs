using ClassManager.Core.Domain.Makeups;

namespace ClassManager.Core.U.Tests.Domain.Makeups.When_a_makeup_is_booked;

public sealed class Then_the_credit_is_used
{
    [Fact]
    public void Then_the_credit_is_used_Run()
    {
        var missedOn = new DateOnly(2026, 9, 21);

        var balance = MakeupCredits.Calculate([new MakeupSource(missedOn, MakeupReason.Cancelled)], [missedOn.AddDays(2)], missedOn);

        balance.Available.ShouldBeEmpty();
        balance.UncoveredBookings.ShouldBe(0);
    }
}
