using Azure.Data.Tables;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Backoffice.Api.I.Tests.Logs.When_commands_were_logged_around_a_day;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_that_day_is_listed(BackofficeApiFixture fixture)
{
    [Fact]
    public async Task Then_only_that_day_is_listed_Run()
    {
        var day = new DateOnly(2025, 3, 14);
        var dayStart = new DateTimeOffset(day.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var tenantId = Guid.CreateVersion7();
        await new TableCommandAuditStore(new TableServiceClient(fixture.StorageConnectionString)).WriteAsync(
            [
                LoggedCommand("BeforeTheDayCommand", dayStart.AddTicks(-1), tenantId),
                LoggedCommand("FirstOfTheDayCommand", dayStart, tenantId),
                LoggedCommand("LastOfTheDayCommand", dayStart.AddDays(1).AddTicks(-1), tenantId),
                LoggedCommand("AfterTheDayCommand", dayStart.AddDays(1), tenantId),
            ],
            CancellationToken.None);
        using var client = fixture.ApiFactory.CreateHttpsClient();
        var accessToken = await client.OpenSessionAsync(await fixture.SeedOperatorAsync());

        var log = await client.GetCommandLogAsync(accessToken, day);

        log.Entries.Select(entry => entry.Command).ShouldBe(["LastOfTheDayCommand", "FirstOfTheDayCommand"]);
    }

    private static CommandAuditEntry LoggedCommand(string command, DateTimeOffset occurredAt, Guid tenantId) =>
        new(occurredAt, tenantId, command, null, null, CommandOutcome.Succeeded, null, null, 5, null, "{}", false);
}
