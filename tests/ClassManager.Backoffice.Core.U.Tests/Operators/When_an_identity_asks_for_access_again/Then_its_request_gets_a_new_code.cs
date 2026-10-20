namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_an_identity_asks_for_access_again;

public sealed class Then_its_request_gets_a_new_code
{
    [Fact]
    public async Task Then_its_request_gets_a_new_code_Run()
    {
        var scenario = new SignInScenario();
        await scenario.SignInAsync();

        var secondAttempt = await scenario.SignInAsync();

        scenario.Logins.AccessRequests.ShouldHaveSingleItem().Code.ShouldBe(secondAttempt.Value!.AccessRequestCode);
    }
}
