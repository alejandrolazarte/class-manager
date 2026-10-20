namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_an_enabled_identity_has_no_platform_role;

public sealed class Then_no_session_is_issued
{
    [Fact]
    public async Task Then_no_session_is_issued_Run()
    {
        var scenario = new SignInScenario().WithEnabledLogin();

        var result = await scenario.SignInAsync();

        scenario.Tokens.IssuedTo.ShouldBeEmpty();
        result.Value!.AccessRequestCode.ShouldNotBeNull();
    }
}
