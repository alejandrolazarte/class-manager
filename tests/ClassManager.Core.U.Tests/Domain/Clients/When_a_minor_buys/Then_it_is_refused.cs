using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_a_minor_buys;

public sealed class Then_it_is_refused
{
    [Fact]
    public void Then_it_is_refused_Run()
    {
        FamilyVisibility.CanBuy(viewerPaysForFamily: false, new DateOnly(2008, 10, 29), new DateOnly(2026, 10, 28)).ShouldBeFalse();
    }
}
