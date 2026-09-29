namespace ClassManager.Infrastructure.Persistence;

internal sealed class StockLock(AppDbContext context) : IStockLock
{
    private const string ResourcePrefix = "stock:";
    private const int LockTimeoutMilliseconds = 10_000;

    public async Task LockAsync(IReadOnlyCollection<Guid> variantIds, CancellationToken cancellationToken)
    {
        foreach (var variantId in variantIds.Distinct().Order())
        {
            var resource = ResourcePrefix + variantId.ToString("N");
            await context.Database.ExecuteSqlInterpolatedAsync(
                $"""
                DECLARE @lockResult int;
                EXEC @lockResult = sp_getapplock @Resource = {resource}, @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = {LockTimeoutMilliseconds};
                IF @lockResult < 0 THROW 51000, 'The stock of a product is busy. Try again.', 1;
                """,
                cancellationToken);
        }
    }
}
