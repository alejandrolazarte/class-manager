namespace ClassManager.Notifications.U.Tests.Email.When_money_is_formatted_for_an_email;

public sealed class Then_euros_show_the_euro_sign
{
    [Fact]
    public void Then_euros_show_the_euro_sign_Run()
    {
        EmailMoney.Format(40m, "EUR").ShouldBe("€\u00A040");
    }
}
