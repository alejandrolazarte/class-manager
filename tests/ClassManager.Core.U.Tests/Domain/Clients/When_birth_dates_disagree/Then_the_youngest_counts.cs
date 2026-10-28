using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_birth_dates_disagree;

public sealed class Then_the_youngest_counts
{
    [Fact]
    public void Then_the_youngest_counts_Run()
    {
        FamilyVisibility.YoungestBirthDate([new DateOnly(2000, 1, 1), null, new DateOnly(2012, 3, 4)])
            .ShouldBe(new DateOnly(2012, 3, 4));
    }
}
