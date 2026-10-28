using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_the_payer_views_a_child_below_the_own_account_age;

public sealed class Then_they_see_everything
{
    private const int ArgentinaOwnAccountMinimumAge = 13;

    [Fact]
    public void Then_they_see_everything_Run()
    {
        FamilyVisibility.ViewOf(isOwnStudent: false, viewerPaysForFamily: true, new DateOnly(2016, 5, 1), new DateOnly(2026, 10, 28), ArgentinaOwnAccountMinimumAge)
            .ShouldBe(FamilyMemberView.Everything);
    }
}
