using Azure.Storage.Blobs;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Backoffice.Api.I.Tests.Logs.When_a_burst_of_requests_without_a_session_was_recorded;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_routes_show_in_the_day_of_security_events(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_its_routes_show_in_the_day_of_security_events_Run()
    {
        var day = new DateOnly(2025, 5, 7);
        var occurredAt = new DateTimeOffset(day.ToDateTime(new TimeOnly(15, 53)), TimeSpan.Zero);
        var store = new AppendBlobSecurityEventStore(
            new BlobServiceClient(fixture.StorageConnectionString).GetBlobContainerClient(AppendBlobSecurityEventStore.ContainerName),
            createsContainer: true);
        await store.WriteAsync(
            [new SecurityEvent(
                occurredAt,
                SecurityEventType.UnauthorizedBurst,
                null,
                null,
                null,
                null,
                null,
                null,
                "203.0.113.7",
                "test-agent",
                42,
                null,
                [new SecurityEventRoute("GET", "/api/students", 40)],
                OtherRouteRequests: 2)],
            CancellationToken.None);
        using var client = fixture.ApiFactory.CreateHttpsClient();
        var accessToken = await client.OpenSessionAsync(await fixture.SeedOperatorAsync());

        var events = await client.GetSecurityEventsAsync(accessToken, day);

        var burst = events.Events.ShouldHaveSingleItem();
        (burst.Routes.ShouldNotBeNull().ShouldHaveSingleItem(), burst.OtherRouteRequests).ShouldBe((new SecurityEventRouteResponse("GET", "/api/students", 40), 2));
    }
}
