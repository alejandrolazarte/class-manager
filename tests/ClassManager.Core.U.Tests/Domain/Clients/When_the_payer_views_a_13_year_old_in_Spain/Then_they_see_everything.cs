using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_the_payer_views_a_13_year_old_in_Spain;

public sealed class Then_they_see_everything
{
    private const int SpainOwnAccountMinimumAge = 14;

    [Fact]
    public void Then_they_see_everything_Run()
    {
        FamilyVisibility.ViewOf(isOwnStudent: false, viewerPaysForFamily: true, new DateOnly(2013, 5, 1), new DateOnly(2026, 10, 28), SpainOwnAccountMinimumAge)
            .ShouldBe(FamilyMemberView.Everything);
    }
}
