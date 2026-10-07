using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.Domain.Accounts.When_the_country_is_unknown;

public sealed class Then_16_is_required
{
    [Fact]
    public void Then_16_is_required_Run()
    {
        var minimumAge = PersonAge.OwnAccountMinimumAge("1");

        minimumAge.ShouldBe(16);
    }
}
