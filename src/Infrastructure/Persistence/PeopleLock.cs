using System.Globalization;
namespace ClassManager.Infrastructure.Persistence;

internal sealed class PeopleLock(AppDbContext context, ITenantContext tenantContext) : IPeopleLock
{
    private const string ResourcePrefix = "people:";
    private const int LockTimeoutMilliseconds = 10_000;

    public Task LockAsync(CancellationToken cancellationToken)
    {
        var resource = ResourcePrefix + tenantContext.TenantId.ToString(CultureInfo.InvariantCulture);
        return context.Database.ExecuteSqlInterpolatedAsync(
            $"""
            DECLARE @lockResult int;
            EXEC @lockResult = sp_getapplock @Resource = {resource}, @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = {LockTimeoutMilliseconds};
            IF @lockResult < 0 THROW 51000, 'The people of the business are busy. Try again.', 1;
            """,
            cancellationToken);
    }
}
