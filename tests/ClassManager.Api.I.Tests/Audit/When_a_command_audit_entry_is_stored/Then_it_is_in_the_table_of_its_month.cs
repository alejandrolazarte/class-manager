using Azure.Data.Tables;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_a_command_audit_entry_is_stored;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_in_the_table_of_its_month(ApiFixture fixture)
{
    private const string TableOfTheMonth = "Commands202610";

    [Fact]
    public async Task Then_it_is_in_the_table_of_its_month_Run()
    {
        var serviceClient = new TableServiceClient(fixture.StorageConnectionString);
        var tenantId = Guid.CreateVersion7();
        var entry = CommandAuditEntries.Succeeded(tenantId);

        await new TableCommandAuditStore(serviceClient).WriteAsync([entry], CancellationToken.None);

        var row = await serviceClient.GetTableClient(TableOfTheMonth)
            .QueryAsync<TableEntity>(stored => stored.PartitionKey == tenantId.ToString())
            .SingleAsync();
        (row.GetString(nameof(CommandAuditEntry.Command)),
            row.GetGuid(nameof(CommandAuditEntry.AccountId)),
            row.GetGuid(nameof(CommandAuditEntry.UserId)),
            row.GetString(nameof(CommandAuditEntry.Outcome)),
            row.GetInt64(nameof(CommandAuditEntry.DurationMilliseconds)),
            row.GetString(nameof(CommandAuditEntry.Payload)))
            .ShouldBe((entry.Command, entry.AccountId, entry.UserId, nameof(CommandOutcome.Succeeded), entry.DurationMilliseconds, entry.Payload));
    }
}
