using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_an_access_request_is_enabled;

public sealed class Then_its_identity_is_enabled_for_the_chosen_member
{
    [Fact]
    public async Task Then_its_identity_is_enabled_for_the_chosen_member_Run()
    {
        var scenario = new OperatorsScenario();
        var accessRequest = scenario.WithAccessRequest();
        var accountId = Guid.CreateVersion7();
        var member = scenario.WithTeamMember("Persona", BusinessRole.Viewer, accountId);

        var result = await scenario.EnableAsync(accessRequest.Id, member.Id, BusinessRole.PlatformReader);

        var login = scenario.Logins.Logins.ShouldHaveSingleItem();
        (login.Id, login.Provider, login.ProviderKey, login.AccountId, login.EnabledAt, login.EnabledByUserId)
            .ShouldBe((result.Value!.LoginId, OperatorsScenario.Identity.Provider, OperatorsScenario.Identity.ProviderKey, accountId, OperatorsScenario.Now, scenario.CurrentOperatorUserId));
        scenario.Logins.AccessRequests.ShouldBeEmpty();
    }
}
