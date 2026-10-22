using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_the_operators_are_listed;

public sealed class Then_only_team_members_with_an_account_can_be_chosen
{
    [Fact]
    public async Task Then_only_team_members_with_an_account_can_be_chosen_Run()
    {
        var scenario = new OperatorsScenario();
        var withAccount = scenario.WithTeamMember("Con cuenta", BusinessRole.Viewer, Guid.CreateVersion7());
        scenario.WithTeamMember("Sin cuenta", BusinessRole.Viewer, accountId: null);

        var result = await scenario.ListAsync();

        result.Value!.TeamMembers.ShouldHaveSingleItem().MemberId.ShouldBe(withAccount.Id);
    }
}
