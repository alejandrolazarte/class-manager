using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_an_enabled_operator_signs_in;

public sealed class Then_a_backoffice_session_is_issued
{
    [Fact]
    public async Task Then_a_backoffice_session_is_issued_Run()
    {
        var scenario = new SignInScenario().WithEnabledLogin().WithPlatformRole(BusinessRole.PlatformOperator);

        var result = await scenario.SignInAsync();

        result.Value!.Tokens.ShouldNotBeNull();
        scenario.Tokens.IssuedTo.ShouldHaveSingleItem().ShouldBe(new SessionUser(
            scenario.AccountId,
            SignInScenario.Identity.Email,
            scenario.BusinessId,
            nameof(BusinessRole.PlatformOperator),
            BackofficeSession.Kind));
    }
}
