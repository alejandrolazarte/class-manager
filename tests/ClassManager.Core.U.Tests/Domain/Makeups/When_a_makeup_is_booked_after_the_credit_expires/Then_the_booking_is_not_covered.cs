using ClassManager.Core.Domain.Makeups;

namespace ClassManager.Core.U.Tests.Domain.Makeups.When_a_makeup_is_booked_after_the_credit_expires;

public sealed class Then_the_booking_is_not_covered
{
    [Fact]
    public void Then_the_booking_is_not_covered_Run()
    {
        var missedOn = new DateOnly(2026, 9, 1);

        var balance = MakeupCredits.Calculate(
            [new MakeupSource(missedOn, MakeupReason.Notice)], [missedOn.AddDays(MakeupCredits.ValidityDays + 1)], missedOn);

        balance.UncoveredBookings.ShouldBe(1);
    }
}
