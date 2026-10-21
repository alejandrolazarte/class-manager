using ClassManager.Api.Audit;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Api.I.Tests.Audit.When_an_address_gets_many_unauthorized_responses;

public sealed class Then_the_burst_keeps_the_user_agent
{
    [Fact]
    public async Task Then_the_burst_keeps_the_user_agent_Run()
    {
        var requests = new SecurityEventRequests();
        for (var request = 0; request < SecurityEventThrottle.UnauthorizedBurstThreshold; request++)
        {
            await requests.SendAsync(StatusCodes.Status401Unauthorized);
        }

        requests.BurstWorker.ReportBursts();

        requests.Events.Events.ShouldHaveSingleItem().UserAgent.ShouldBe(SecurityEventRequests.UserAgent);
    }
}
