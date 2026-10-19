using ClassManager.Api.Audit;
using ClassManager.Infrastructure.Audit;
using Microsoft.AspNetCore.Http;

namespace ClassManager.Api.I.Tests.Audit.When_a_sign_in_is_rejected;

public sealed class Then_a_sign_in_failed_event_is_recorded
{
    [Fact]
    public async Task Then_a_sign_in_failed_event_is_recorded_Run()
    {
        var requests = new SecurityEventRequests();

        await requests.SendAsync(StatusCodes.Status401Unauthorized, new SecurityEventOnRejection(SecurityEventType.SignInFailed));

        requests.Events.Events.ShouldHaveSingleItem().Event.ShouldBe(SecurityEventType.SignInFailed);
    }
}
