using Microsoft.AspNetCore.Http;

namespace ClassManager.Api.I.Tests.Audit.When_an_access_token_is_rejected;

public sealed class Then_no_security_event_is_recorded
{
    [Fact]
    public async Task Then_no_security_event_is_recorded_Run()
    {
        var requests = new SecurityEventRequests();

        await requests.SendAsync(StatusCodes.Status401Unauthorized);

        requests.Events.Events.ShouldBeEmpty();
    }
}
