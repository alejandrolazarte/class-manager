namespace ClassManager.Notifications.U.Tests.Email.When_money_is_formatted_for_an_email;

public sealed class Then_us_dollars_stay_apart_from_local_pesos
{
    [Fact]
    public void Then_us_dollars_stay_apart_from_local_pesos_Run()
    {
        EmailMoney.Format(40m, "USD").ShouldBe("US$\u00A040");
    }
}
