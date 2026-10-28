using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_the_payer_buys;

public sealed class Then_it_is_allowed
{
    [Fact]
    public void Then_it_is_allowed_Run()
    {
        FamilyVisibility.CanBuy(viewerPaysForFamily: true, birthDate: null, new DateOnly(2026, 10, 28)).ShouldBeTrue();
    }
}
