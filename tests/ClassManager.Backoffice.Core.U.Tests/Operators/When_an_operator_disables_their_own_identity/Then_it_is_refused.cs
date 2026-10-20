using ClassManager.Core.Common;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_an_operator_disables_their_own_identity;

public sealed class Then_it_is_refused
{
    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        var scenario = new OperatorsScenario();
        var login = scenario.WithLogin(scenario.CurrentOperatorAccountId, "google-subject-4");

        var result = await scenario.DisableAsync(login.Id);

        (result.Error!.Kind, result.Error.Code).ShouldBe((ErrorKind.Conflict, OperatorErrorCodes.CannotDisableYourself));
        scenario.Logins.Logins.ShouldHaveSingleItem();
        scenario.Sessions.RevokedAccountIds.ShouldBeEmpty();
    }
}
