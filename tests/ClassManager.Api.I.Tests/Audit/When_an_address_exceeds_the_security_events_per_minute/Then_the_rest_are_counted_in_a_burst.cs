using ClassManager.Infrastructure.Audit;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Api.I.Tests.Audit.When_an_address_exceeds_the_security_events_per_minute;

public sealed class Then_the_rest_are_counted_in_a_burst
{
    private const int ForbiddenRequests = 15;

    [Fact]
    public async Task Then_the_rest_are_counted_in_a_burst_Run()
    {
        var requests = new SecurityEventRequests();
        for (var request = 0; request < ForbiddenRequests; request++)
        {
            await requests.SendAsync(StatusCodes.Status403Forbidden);
        }

        requests.BurstWorker.ReportBursts();

        var maximum = new AuditLogOptions().MaximumSecurityEventsPerAddressPerMinute;
        (requests.Events.Events.Count(recorded => recorded.Event == SecurityEventType.Forbidden),
            requests.Events.Events.Single(recorded => recorded.Event == SecurityEventType.UnauthorizedBurst).Count)
            .ShouldBe((maximum, ForbiddenRequests - maximum));
    }
}
