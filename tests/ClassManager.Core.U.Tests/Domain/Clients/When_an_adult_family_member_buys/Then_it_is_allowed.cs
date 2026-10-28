using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_an_adult_family_member_buys;

public sealed class Then_it_is_allowed
{
    [Fact]
    public void Then_it_is_allowed_Run()
    {
        FamilyVisibility.CanBuy(viewerPaysForFamily: false, new DateOnly(2008, 10, 28), new DateOnly(2026, 10, 28)).ShouldBeTrue();
    }
}
