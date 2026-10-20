using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_the_chosen_role_is_not_a_platform_role;

public sealed class Then_enabling_is_refused
{
    [Fact]
    public async Task Then_enabling_is_refused_Run()
    {
        var scenario = new OperatorsScenario();
        var accessRequest = scenario.WithAccessRequest();
        var member = scenario.WithTeamMember("Persona", BusinessRole.Viewer, Guid.CreateVersion7());

        var result = await scenario.EnableAsync(accessRequest.Id, member.Id, BusinessRole.BranchOwner);

        result.Error!.Code.ShouldBe(OperatorErrorCodes.NotAPlatformRole);
        scenario.Logins.Logins.ShouldBeEmpty();
        member.Role.ShouldBe(BusinessRole.Viewer);
    }
}
