using ClassManager.Core.Common;
using ClassManager.Infrastructure.Audit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Audit.When_an_audited_command_throws_a_database_error;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_entry_has_the_sql_error_number(ApiFixture fixture)
{
    private const string DivideByZeroQuery = "SELECT 1 / 0";
    private const string ExpectedErrorCode = "sql.8134";

    [Fact]
    public async Task Then_its_entry_has_the_sql_error_number_Run()
    {
        var databaseError = new DbUpdateException("Saving failed.", await SqlErrorAsync());
        var auditLog = new RecordingCommandAuditService();
        var behavior = CommandAuditBehaviors.Create(auditLog);

        await Should.ThrowAsync<DbUpdateException>(() => behavior.HandleAsync(
            CommandAuditBehaviors.Command,
            () => Task.FromException<Result<string>>(databaseError),
            CancellationToken.None));

        var entry = auditLog.Entries.ShouldHaveSingleItem();
        (entry.Outcome, entry.ErrorKind, entry.ErrorCode).ShouldBe((CommandOutcome.Exception, nameof(DbUpdateException), ExpectedErrorCode));
    }

    private async Task<SqlException> SqlErrorAsync()
    {
        await using var connection = new SqlConnection(fixture.ConnectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(DivideByZeroQuery, connection);

        return await Should.ThrowAsync<SqlException>(() => command.ExecuteScalarAsync());
    }
}
