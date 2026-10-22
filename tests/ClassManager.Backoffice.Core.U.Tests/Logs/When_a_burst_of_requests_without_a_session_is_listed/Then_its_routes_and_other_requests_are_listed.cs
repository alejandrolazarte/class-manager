namespace ClassManager.Backoffice.Core.U.Tests.Logs.When_a_burst_of_requests_without_a_session_is_listed;

public sealed class Then_its_routes_and_other_requests_are_listed
{
    private const int OtherRouteRequests = 7;

    private static readonly SecurityLogRoute StudentsRoute = new("GET", "/api/students", 30);

    [Fact]
    public async Task Then_its_routes_and_other_requests_are_listed_Run()
    {
        var scenario = new LogsScenario();
        scenario.Reader.SecurityEvents.Add(new SecurityLogEntry(
            LogsScenario.Now, "UnauthorizedBurst", null, null, null, null, null, null, "203.0.113.7", "test-agent", 37, null, [StudentsRoute], OtherRouteRequests));

        var result = await scenario.ListSecurityEventsAsync(LogsScenario.Today);

        var burst = result.Value!.Events.ShouldHaveSingleItem();
        (burst.Routes.ShouldNotBeNull().ShouldHaveSingleItem(), burst.OtherRouteRequests).ShouldBe((StudentsRoute, OtherRouteRequests));
    }
}
