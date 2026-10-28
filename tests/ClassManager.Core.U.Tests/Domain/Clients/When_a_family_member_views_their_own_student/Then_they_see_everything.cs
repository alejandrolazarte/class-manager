using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_a_family_member_views_their_own_student;

public sealed class Then_they_see_everything
{
    private const int ArgentinaOwnAccountMinimumAge = 13;

    [Fact]
    public void Then_they_see_everything_Run()
    {
        FamilyVisibility.ViewOf(isOwnStudent: true, viewerPaysForFamily: false, new DateOnly(2015, 1, 1), new DateOnly(2026, 10, 28), ArgentinaOwnAccountMinimumAge)
            .ShouldBe(FamilyMemberView.Everything);
    }
}
