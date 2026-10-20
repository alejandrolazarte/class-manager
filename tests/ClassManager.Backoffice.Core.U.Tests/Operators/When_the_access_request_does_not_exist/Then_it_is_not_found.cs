using ClassManager.Core.Common;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_the_access_request_does_not_exist;

public sealed class Then_it_is_not_found
{
    [Fact]
    public async Task Then_it_is_not_found_Run()
    {
        var scenario = new OperatorsScenario();
        var member = scenario.WithTeamMember("Persona", BusinessRole.Viewer, Guid.CreateVersion7());

        var result = await scenario.EnableAsync(Guid.CreateVersion7(), member.Id, BusinessRole.PlatformOperator);

        (result.Error!.Kind, result.Error.Code).ShouldBe((ErrorKind.NotFound, OperatorErrorCodes.AccessRequestNotFound));
    }
}
