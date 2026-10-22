namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_an_operator_is_disabled;

public sealed class Then_the_login_is_removed_and_their_sessions_revoked
{
    [Fact]
    public async Task Then_the_login_is_removed_and_their_sessions_revoked_Run()
    {
        var scenario = new OperatorsScenario();
        var accountId = Guid.CreateVersion7();
        var login = scenario.WithLogin(accountId, "google-subject-3");

        await scenario.DisableAsync(login.Id);

        scenario.Sessions.RevokedAccountIds.ShouldBe([accountId]);
        scenario.Logins.Logins.ShouldBeEmpty();
    }
}
