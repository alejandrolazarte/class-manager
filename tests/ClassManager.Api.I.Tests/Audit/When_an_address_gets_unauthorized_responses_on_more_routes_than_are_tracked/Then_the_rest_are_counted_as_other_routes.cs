using ClassManager.Api.Audit;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Api.I.Tests.Audit.When_an_address_gets_unauthorized_responses_on_more_routes_than_are_tracked;

public sealed class Then_the_rest_are_counted_as_other_routes
{
    private const int UntrackedRoutes = 5;

    [Fact]
    public async Task Then_the_rest_are_counted_as_other_routes_Run()
    {
        var requests = new SecurityEventRequests();
        for (var route = 0; route < SecurityEventThrottle.MaximumTrackedRoutesPerAddress + UntrackedRoutes; route++)
        {
            await requests.SendAsync(StatusCodes.Status401Unauthorized, route: $"/api/route-{route}");
        }

        requests.BurstWorker.ReportBursts();

        var burst = requests.Events.Events.ShouldHaveSingleItem();
        (burst.Routes!.Count, burst.OtherRouteRequests).ShouldBe((SecurityEventThrottle.MaximumTrackedRoutesPerAddress, UntrackedRoutes));
    }
}
