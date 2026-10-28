using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_the_payer_views_a_teenager;

public sealed class Then_they_see_only_classes
{
    private const int ArgentinaOwnAccountMinimumAge = 13;

    [Fact]
    public void Then_they_see_only_classes_Run()
    {
        FamilyVisibility.ViewOf(isOwnStudent: false, viewerPaysForFamily: true, new DateOnly(2011, 5, 1), new DateOnly(2026, 10, 28), ArgentinaOwnAccountMinimumAge)
            .ShouldBe(FamilyMemberView.Classes);
    }
}
