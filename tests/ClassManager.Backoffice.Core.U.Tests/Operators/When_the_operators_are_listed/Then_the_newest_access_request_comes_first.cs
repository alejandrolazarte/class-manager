namespace ClassManager.Backoffice.Core.U.Tests.Operators.When_the_operators_are_listed;

public sealed class Then_the_newest_access_request_comes_first
{
    [Fact]
    public async Task Then_the_newest_access_request_comes_first_Run()
    {
        var scenario = new OperatorsScenario();
        var older = scenario.WithAccessRequest();
        var newer = BackofficeAccessRequest.Create(OperatorsScenario.Identity with { ProviderKey = "google-subject-6" }, "AB-CDE", OperatorsScenario.Now);
        scenario.Logins.AccessRequests.Add(newer);

        var result = await scenario.ListAsync();

        result.Value!.AccessRequests.Select(request => request.Id).ShouldBe([newer.Id, older.Id]);
    }
}
