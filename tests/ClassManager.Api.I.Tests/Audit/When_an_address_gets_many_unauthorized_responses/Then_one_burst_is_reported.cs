using ClassManager.Api.Audit;
using ClassManager.Infrastructure.Audit;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Api.I.Tests.Audit.When_an_address_gets_many_unauthorized_responses;

public sealed class Then_one_burst_is_reported
{
    [Fact]
    public async Task Then_one_burst_is_reported_Run()
    {
        var requests = new SecurityEventRequests();
        for (var request = 0; request < SecurityEventThrottle.UnauthorizedBurstThreshold; request++)
        {
            await requests.SendAsync(StatusCodes.Status401Unauthorized);
        }

        requests.BurstWorker.ReportBursts();
        requests.BurstWorker.ReportBursts();

        var burst = requests.Events.Events.ShouldHaveSingleItem();
        (burst.Event, burst.IpAddress, burst.Count).ShouldBe((SecurityEventType.UnauthorizedBurst, SecurityEventRequests.IpAddress, SecurityEventThrottle.UnauthorizedBurstThreshold));
    }
}
