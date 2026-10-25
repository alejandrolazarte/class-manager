namespace ClassManager.Notifications.U.Tests.Email.When_money_is_formatted_for_an_email;

public sealed class Then_cents_are_shown_only_when_there_are_some
{
    [Fact]
    public void Then_cents_are_shown_only_when_there_are_some_Run()
    {
        EmailMoney.Format(9.5m, "GBP").ShouldBe("£\u00A09,50");
    }
}
