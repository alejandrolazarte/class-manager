using ClassManager.Core.Domain.Makeups;

namespace ClassManager.Core.U.Tests.Domain.Makeups.When_a_makeup_is_booked_with_two_credits;

public sealed class Then_the_one_expiring_first_is_used
{
    [Fact]
    public void Then_the_one_expiring_first_is_used_Run()
    {
        var older = new DateOnly(2026, 9, 1);
        var newer = new DateOnly(2026, 9, 20);

        var balance = MakeupCredits.Calculate(
            [new MakeupSource(newer, MakeupReason.Notice), new MakeupSource(older, MakeupReason.Cancelled)], [newer.AddDays(1)], newer);

        balance.Available.ShouldBe([new MakeupCredit(newer, MakeupReason.Notice, newer.AddDays(MakeupCredits.ValidityDays))]);
    }
}
