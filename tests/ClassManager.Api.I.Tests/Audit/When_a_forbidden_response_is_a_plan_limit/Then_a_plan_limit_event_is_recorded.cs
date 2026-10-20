using ClassManager.Infrastructure.Audit;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Api.I.Tests.Audit.When_a_forbidden_response_is_a_plan_limit;

public sealed class Then_a_plan_limit_event_is_recorded
{
    [Fact]
    public async Task Then_a_plan_limit_event_is_recorded_Run()
    {
        var requests = new SecurityEventRequests();

        await requests.SendAsync(StatusCodes.Status403Forbidden, planLimit: true);

        var recorded = requests.Events.Events.ShouldHaveSingleItem();
        (recorded.Event, recorded.Route, recorded.StatusCode, recorded.TenantId).ShouldBe((
            SecurityEventType.PlanLimit,
            SecurityEventRequests.ClientsRoute,
            StatusCodes.Status403Forbidden,
            SecurityEventRequests.TenantId));
    }
}
