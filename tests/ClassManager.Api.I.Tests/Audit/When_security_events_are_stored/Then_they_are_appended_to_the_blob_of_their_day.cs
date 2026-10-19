using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_security_events_are_stored;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_are_appended_to_the_blob_of_their_day(ApiFixture fixture)
{
    private static readonly DateTimeOffset Day = new(2026, 2, 3, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Then_they_are_appended_to_the_blob_of_their_day_Run()
    {
        var container = SecurityEventBlobs.Container(fixture);
        var store = new AppendBlobSecurityEventStore(container, createsContainer: true);

        await store.WriteAsync([SignInFailedAt(Day)], CancellationToken.None);
        await store.WriteAsync([SignInFailedAt(Day.AddHours(5))], CancellationToken.None);

        var lines = await SecurityEventBlobs.ReadDayAsync(container, Day);
        lines.Select(line => (SecurityEventBlobs.Text(line, "event"), line.GetProperty("occurredAt").GetDateTimeOffset()))
            .ShouldBe([(nameof(SecurityEventType.SignInFailed), Day), (nameof(SecurityEventType.SignInFailed), Day.AddHours(5))]);
    }

    private static SecurityEvent SignInFailedAt(DateTimeOffset occurredAt) => new(
        occurredAt,
        SecurityEventType.SignInFailed,
        TenantId: null,
        AccountId: null,
        UserId: null,
        Route: "/api/auth/sign-in",
        Method: "POST",
        StatusCode: 401,
        IpAddress: "203.0.113.7",
        UserAgent: "test",
        Count: null,
        TraceId: null);
}
