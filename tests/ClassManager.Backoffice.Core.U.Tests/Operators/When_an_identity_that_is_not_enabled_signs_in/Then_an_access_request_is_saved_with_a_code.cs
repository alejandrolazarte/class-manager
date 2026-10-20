using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_an_identity_that_is_not_enabled_signs_in;

public sealed class Then_an_access_request_is_saved_with_a_code
{
    [Fact]
    public async Task Then_an_access_request_is_saved_with_a_code_Run()
    {
        var scenario = new SignInScenario().WithPlatformRole(BusinessRole.PlatformOperator);

        var result = await scenario.SignInAsync();

        var accessRequest = scenario.Logins.AccessRequests.ShouldHaveSingleItem();
        (accessRequest.Email, accessRequest.Code, accessRequest.RequestedAt, result.Value!.Tokens)
            .ShouldBe((SignInScenario.Identity.Email, result.Value.AccessRequestCode!, SignInScenario.Now, null));
        scenario.Tokens.IssuedTo.ShouldBeEmpty();
    }
}
