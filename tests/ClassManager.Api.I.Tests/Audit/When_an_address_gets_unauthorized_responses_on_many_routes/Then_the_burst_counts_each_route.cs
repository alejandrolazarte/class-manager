using ClassManager.Api.Audit;
using ClassManager.Infrastructure.Audit;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Api.I.Tests.Audit.When_an_address_gets_unauthorized_responses_on_many_routes;

public sealed class Then_the_burst_counts_each_route
{
    private const string StudentRoute = "/api/students/{id}";
    private const int StudentRequests = 3;

    [Fact]
    public async Task Then_the_burst_counts_each_route_Run()
    {
        var requests = new SecurityEventRequests();
        for (var request = 0; request < SecurityEventThrottle.UnauthorizedBurstThreshold; request++)
        {
            await requests.SendAsync(StatusCodes.Status401Unauthorized);
        }

        for (var request = 0; request < StudentRequests; request++)
        {
            await requests.SendAsync(StatusCodes.Status401Unauthorized, route: StudentRoute);
        }

        requests.BurstWorker.ReportBursts();

        requests.Events.Events.ShouldHaveSingleItem().Routes.ShouldBe([
            new SecurityEventRoute(HttpMethods.Post, SecurityEventRequests.ClientsRoute, SecurityEventThrottle.UnauthorizedBurstThreshold),
            new SecurityEventRoute(HttpMethods.Post, StudentRoute, StudentRequests),
        ]);
    }
}
