using Microsoft.AspNetCore.Http;

namespace ClassManager.Api.I.Tests.Audit.When_a_forbidden_response_is_a_plan_limit;

public sealed class Then_no_security_event_is_recorded
{
    [Fact]
    public async Task Then_no_security_event_is_recorded_Run()
    {
        var requests = new SecurityEventRequests();

        await requests.SendAsync(StatusCodes.Status403Forbidden, planLimit: true);

        requests.Events.Events.ShouldBeEmpty();
    }
}
