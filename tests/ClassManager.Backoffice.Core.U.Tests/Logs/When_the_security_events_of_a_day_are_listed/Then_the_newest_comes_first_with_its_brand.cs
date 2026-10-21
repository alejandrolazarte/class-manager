namespace ClassManager.Backoffice.Core.U.Tests.Logs.When_the_security_events_of_a_day_are_listed;

public sealed class Then_the_newest_comes_first_with_its_brand
{
    [Fact]
    public async Task Then_the_newest_comes_first_with_its_brand_Run()
    {
        var scenario = new LogsScenario();
        var branch = scenario.WithBranch("Estudio Centro", "Sede Norte");
        scenario.WithSecurityEvent("SignInFailed", LogsScenario.Now.AddHours(-2));
        scenario.WithSecurityEvent("Forbidden", LogsScenario.Now, branch.BranchId);

        var result = await scenario.ListSecurityEventsAsync(LogsScenario.Today);

        result.Value!.Events.Select(entry => (entry.Event, entry.BrandName))
            .ShouldBe([("Forbidden", "Estudio Centro"), ("SignInFailed", (string?)null)]);
    }
}
