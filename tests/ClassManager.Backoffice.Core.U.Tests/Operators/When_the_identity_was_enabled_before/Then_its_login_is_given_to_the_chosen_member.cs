using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_the_identity_was_enabled_before;

public sealed class Then_its_login_is_given_to_the_chosen_member
{
    [Fact]
    public async Task Then_its_login_is_given_to_the_chosen_member_Run()
    {
        var scenario = new OperatorsScenario();
        var previousLogin = scenario.WithLogin(Guid.CreateVersion7(), OperatorsScenario.Identity.ProviderKey);
        var accessRequest = scenario.WithAccessRequest();
        var accountId = Guid.CreateVersion7();
        var member = scenario.WithTeamMember("Persona", BusinessRole.Viewer, accountId);

        await scenario.EnableAsync(accessRequest.Id, member.Id, BusinessRole.PlatformOperator);

        var login = scenario.Logins.Logins.ShouldHaveSingleItem();
        (login.Id, login.AccountId, login.EnabledAt).ShouldBe((previousLogin.Id, accountId, OperatorsScenario.Now));
    }
}
