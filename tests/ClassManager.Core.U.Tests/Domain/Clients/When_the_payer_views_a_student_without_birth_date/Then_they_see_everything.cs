using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_the_payer_views_a_student_without_birth_date;

public sealed class Then_they_see_everything
{
    private const int ArgentinaOwnAccountMinimumAge = 13;

    [Fact]
    public void Then_they_see_everything_Run()
    {
        FamilyVisibility.ViewOf(isOwnStudent: false, viewerPaysForFamily: true, birthDate: null, new DateOnly(2026, 10, 28), ArgentinaOwnAccountMinimumAge)
            .ShouldBe(FamilyMemberView.Everything);
    }
}
