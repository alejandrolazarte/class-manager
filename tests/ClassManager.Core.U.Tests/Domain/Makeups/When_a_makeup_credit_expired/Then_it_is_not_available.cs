using ClassManager.Core.Domain.Makeups;

namespace ClassManager.Core.U.Tests.Domain.Makeups.When_a_makeup_credit_expired;

public sealed class Then_it_is_not_available
{
    [Fact]
    public void Then_it_is_not_available_Run()
    {
        var missedOn = new DateOnly(2026, 8, 1);

        var balance = MakeupCredits.Calculate([new MakeupSource(missedOn, MakeupReason.Notice)], [], missedOn.AddDays(MakeupCredits.ValidityDays + 1));

        balance.Available.ShouldBeEmpty();
    }
}
