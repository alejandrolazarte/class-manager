using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ClassManager.Infrastructure.Persistence;

internal sealed class SqlConnectionRetryInterceptor : DbConnectionInterceptor
{
    private const int NumberOfTries = 6;

    private static readonly TimeSpan DeltaTime = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan MaxTimeInterval = TimeSpan.FromSeconds(20);

    private static readonly SqlRetryLogicBaseProvider OpenRetryProvider = SqlConfigurableRetryFactory.CreateExponentialRetryProvider(
        new SqlRetryLogicOption
        {
            NumberOfTries = NumberOfTries,
            DeltaTime = DeltaTime,
            MaxTimeInterval = MaxTimeInterval,
        });

    public override DbConnection ConnectionCreated(ConnectionCreatedEventData eventData, DbConnection result)
    {
        if (result is SqlConnection sqlConnection)
        {
            sqlConnection.RetryLogicProvider = OpenRetryProvider;
        }

        return result;
    }
}
