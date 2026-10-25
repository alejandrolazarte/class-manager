namespace ClassManager.Notifications.U.Tests.Email.When_money_is_formatted_for_an_email;

public sealed class Then_thousands_are_separated_with_a_dot
{
    [Fact]
    public void Then_thousands_are_separated_with_a_dot_Run()
    {
        EmailMoney.Format(12000m, "ARS").ShouldBe("$\u00A012.000");
    }
}
