using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_the_chosen_member_has_no_account;

public sealed class Then_enabling_is_refused
{
    [Fact]
    public async Task Then_enabling_is_refused_Run()
    {
        var scenario = new OperatorsScenario();
        var accessRequest = scenario.WithAccessRequest();
        var member = scenario.WithTeamMember("Persona", BusinessRole.Viewer, accountId: null);

        var result = await scenario.EnableAsync(accessRequest.Id, member.Id, BusinessRole.PlatformOperator);

        result.Error!.Code.ShouldBe(OperatorErrorCodes.UnknownTeamMember);
        member.Role.ShouldBe(BusinessRole.Viewer);
    }
}
