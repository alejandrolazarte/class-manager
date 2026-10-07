using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.Domain.Accounts.When_a_person_is_13_in_Spain;

public sealed class Then_they_cannot_have_their_own_account
{
    [Fact]
    public void Then_they_cannot_have_their_own_account_Run()
    {
        var today = new DateOnly(2026, 10, 7);

        var canHaveOwnAccount = PersonAge.CanHaveOwnAccount(new DateOnly(2013, 1, 1), today, "34");

        canHaveOwnAccount.ShouldBeFalse();
    }
}
