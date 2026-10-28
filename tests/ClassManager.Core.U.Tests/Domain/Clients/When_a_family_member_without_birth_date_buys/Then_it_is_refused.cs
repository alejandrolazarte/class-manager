using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_a_family_member_without_birth_date_buys;

public sealed class Then_it_is_refused
{
    [Fact]
    public void Then_it_is_refused_Run()
    {
        FamilyVisibility.CanBuy(viewerPaysForFamily: false, birthDate: null, new DateOnly(2026, 10, 28)).ShouldBeFalse();
    }
}
