using System.Globalization;
using Azure.Data.Tables;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_command_audit_entries_of_one_business_are_stored;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_newest_comes_first(ApiFixture fixture)
{
    private const string TableOfTheMonth = "Commands202610";

    [Fact]
    public async Task Then_the_newest_comes_first_Run()
    {
        var serviceClient = new TableServiceClient(fixture.StorageConnectionString);
        var tenantId = TestIds.Next();
        var older = CommandAuditEntries.Succeeded(tenantId, CommandAuditEntries.OccurredAt);
        var newer = CommandAuditEntries.Succeeded(tenantId, CommandAuditEntries.OccurredAt.AddMinutes(1));

        await new TableCommandAuditStore(serviceClient).WriteAsync([older, newer], CancellationToken.None);

        var rows = await serviceClient.GetTableClient(TableOfTheMonth)
            .QueryAsync<TableEntity>(stored => stored.PartitionKey == tenantId.ToString(CultureInfo.InvariantCulture))
            .ToListAsync();
        rows.Select(row => row.GetDateTimeOffset(nameof(CommandAuditEntry.OccurredAt))).ShouldBe([newer.OccurredAt, older.OccurredAt]);
    }
}
