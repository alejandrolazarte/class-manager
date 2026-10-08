using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Core.U.Tests.Domain.Accounts.When_a_birthday_has_not_come_yet;

public sealed class Then_the_year_is_not_counted
{
    [Fact]
    public void Then_the_year_is_not_counted_Run()
    {
        var years = PersonAge.YearsOn(new DateOnly(2008, 10, 8), new DateOnly(2026, 10, 7));

        years.ShouldBe(17);
    }
}
