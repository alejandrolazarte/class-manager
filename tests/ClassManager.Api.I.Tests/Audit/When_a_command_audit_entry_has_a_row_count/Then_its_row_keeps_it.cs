using System.Globalization;
using Azure.Data.Tables;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_a_command_audit_entry_has_a_row_count;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_row_keeps_it(ApiFixture fixture)
{
    private const string TableOfTheMonth = "Commands202610";
    private const int ExportedRows = 342;

    [Fact]
    public async Task Then_its_row_keeps_it_Run()
    {
        var serviceClient = new TableServiceClient(fixture.StorageConnectionString);
        var tenantId = TestIds.Next();
        var entry = CommandAuditEntries.Succeeded(tenantId) with { RowCount = ExportedRows };

        await new TableCommandAuditStore(serviceClient).WriteAsync([entry], CancellationToken.None);

        var row = await serviceClient.GetTableClient(TableOfTheMonth)
            .QueryAsync<TableEntity>(stored => stored.PartitionKey == tenantId.ToString(CultureInfo.InvariantCulture))
            .SingleAsync();
        row.GetInt32(nameof(CommandAuditEntry.RowCount)).ShouldBe(ExportedRows);
    }
}
