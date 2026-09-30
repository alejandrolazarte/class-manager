using ClassManager.Core.Domain.Makeups;

namespace ClassManager.Core.U.Tests.Domain.Makeups.When_a_class_was_missed_with_notice;

public sealed class Then_one_credit_is_available_for_30_days
{
    [Fact]
    public void Then_one_credit_is_available_for_30_days_Run()
    {
        var missedOn = new DateOnly(2026, 9, 21);

        var balance = MakeupCredits.Calculate([new MakeupSource(missedOn, MakeupReason.Notice)], [], missedOn);

        balance.Available.ShouldBe([new MakeupCredit(missedOn, MakeupReason.Notice, missedOn.AddDays(MakeupCredits.ValidityDays))]);
        balance.UncoveredBookings.ShouldBe(0);
    }
}
