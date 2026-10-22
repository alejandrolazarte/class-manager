using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_the_chosen_member_is_not_on_the_platform_team;

public sealed class Then_enabling_is_refused
{
    [Fact]
    public async Task Then_enabling_is_refused_Run()
    {
        var scenario = new OperatorsScenario();
        var accessRequest = scenario.WithAccessRequest();

        var result = await scenario.EnableAsync(accessRequest.Id, TestIds.Next(), BusinessRole.PlatformOperator);

        (result.Error!.Code, result.Error.FieldName).ShouldBe((OperatorErrorCodes.UnknownTeamMember, "MemberId"));
        scenario.Logins.Logins.ShouldBeEmpty();
    }
}
