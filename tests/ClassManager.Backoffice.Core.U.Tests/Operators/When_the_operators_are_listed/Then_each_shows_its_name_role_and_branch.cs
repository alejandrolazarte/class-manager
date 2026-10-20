using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_the_operators_are_listed;

public sealed class Then_each_shows_its_name_role_and_branch
{
    [Fact]
    public async Task Then_each_shows_its_name_role_and_branch_Run()
    {
        var scenario = new OperatorsScenario();
        scenario.WithTeamMember("Operadora", BusinessRole.PlatformReader, scenario.CurrentOperatorAccountId);
        var login = scenario.WithLogin(scenario.CurrentOperatorAccountId, "google-subject-5");

        var result = await scenario.ListAsync();

        result.Value!.Operators.ShouldHaveSingleItem().ShouldBe(new OperatorResponse(
            login.Id, login.Provider, login.Email, "Operadora", BusinessRole.PlatformReader, OperatorsScenario.BranchName, login.EnabledAt, IsCurrentOperator: true));
    }
}
