using ClassManager.Infrastructure.Audit;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Api.I.Tests.Audit.When_a_request_is_forbidden;

public sealed class Then_a_forbidden_event_is_recorded
{
    [Fact]
    public async Task Then_a_forbidden_event_is_recorded_Run()
    {
        var requests = new SecurityEventRequests();

        await requests.SendAsync(StatusCodes.Status403Forbidden);

        var recorded = requests.Events.Events.ShouldHaveSingleItem();
        (recorded.Event, recorded.Route, recorded.Method, recorded.StatusCode, recorded.IpAddress, recorded.TenantId, recorded.UserId).ShouldBe((
            SecurityEventType.Forbidden,
            SecurityEventRequests.ClientsRoute,
            HttpMethods.Post,
            StatusCodes.Status403Forbidden,
            SecurityEventRequests.IpAddress,
            SecurityEventRequests.TenantId,
            SecurityEventRequests.UserId));
    }
}
