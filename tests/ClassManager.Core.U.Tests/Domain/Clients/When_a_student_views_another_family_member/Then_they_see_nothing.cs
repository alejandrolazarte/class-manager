using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_a_student_views_another_family_member;

public sealed class Then_they_see_nothing
{
    private const int ArgentinaOwnAccountMinimumAge = 13;

    [Fact]
    public void Then_they_see_nothing_Run()
    {
        FamilyVisibility.ViewOf(isOwnStudent: false, viewerPaysForFamily: false, new DateOnly(2016, 5, 1), new DateOnly(2026, 10, 28), ArgentinaOwnAccountMinimumAge)
            .ShouldBe(FamilyMemberView.None);
    }
}
