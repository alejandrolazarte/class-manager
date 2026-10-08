using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.Domain.Accounts.When_a_person_is_13_in_Argentina;

public sealed class Then_they_can_have_their_own_account
{
    [Fact]
    public void Then_they_can_have_their_own_account_Run()
    {
        var today = new DateOnly(2026, 10, 7);

        var canHaveOwnAccount = PersonAge.CanHaveOwnAccount(new DateOnly(2013, 10, 7), today, "54");

        canHaveOwnAccount.ShouldBeTrue();
    }
}
