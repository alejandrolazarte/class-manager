using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_an_access_request_is_enabled;

public sealed class Then_the_member_gets_the_platform_role_in_its_branch
{
    [Fact]
    public async Task Then_the_member_gets_the_platform_role_in_its_branch_Run()
    {
        var scenario = new OperatorsScenario();
        var accessRequest = scenario.WithAccessRequest();
        var member = scenario.WithTeamMember("Persona", BusinessRole.Viewer, Guid.CreateVersion7());

        await scenario.EnableAsync(accessRequest.Id, member.Id, BusinessRole.PlatformOperator);

        (member.Role, scenario.TenantScope.EstablishedTenantId, scenario.UnitOfWork.SaveCount)
            .ShouldBe((BusinessRole.PlatformOperator, scenario.BranchId, 1));
    }
}
