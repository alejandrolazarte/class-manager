using ClassManager.Infrastructure.Persistence;
using ClassManager.Security.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Persistence.When_the_database_connection_is_opened;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_transient_failures_are_retried(ApiFixture fixture)
{
    private const int SingleAttempt = 1;

    [Fact]
    public async Task Then_transient_failures_are_retried_Run()
    {
        await using var scope = fixture.ApiFactory.Services.CreateAsyncScope();
        var appConnection = (SqlConnection)scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.GetDbConnection();
        var securityConnection = scope.ServiceProvider.GetRequiredService<SecurityDbContext>().Database.GetDbConnection();

        appConnection.RetryLogicProvider.RetryLogic.NumberOfTries.ShouldBeGreaterThan(SingleAttempt);
        securityConnection.ShouldBeSameAs(appConnection);
    }
}
