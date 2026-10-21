using Azure.Storage.Blobs;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Backoffice.Api.I.Tests.Logs.When_a_security_event_was_recorded;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_shows_in_the_day_of_security_events(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_it_shows_in_the_day_of_security_events_Run()
    {
        var day = new DateOnly(2025, 5, 6);
        var occurredAt = new DateTimeOffset(day.ToDateTime(new TimeOnly(9, 30)), TimeSpan.Zero);
        var store = new AppendBlobSecurityEventStore(
            new BlobServiceClient(fixture.StorageConnectionString).GetBlobContainerClient(AppendBlobSecurityEventStore.ContainerName),
            createsContainer: true);
        await store.WriteAsync(
            [new SecurityEvent(occurredAt, SecurityEventType.SignInFailed, null, null, null, "/api/auth/signin", "POST", 401, "203.0.113.7", null, null, null)],
            CancellationToken.None);
        using var client = fixture.ApiFactory.CreateHttpsClient();
        var accessToken = await client.OpenSessionAsync(await fixture.SeedOperatorAsync());

        var events = await client.GetSecurityEventsAsync(accessToken, day);

        events.Events.ShouldHaveSingleItem().ShouldBe(new SecurityEventEntryResponse("SignInFailed", "/api/auth/signin", "203.0.113.7", null));
    }
}
