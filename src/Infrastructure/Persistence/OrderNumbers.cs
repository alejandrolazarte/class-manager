namespace ClassManager.Infrastructure.Persistence;

internal sealed class OrderNumbers(AppDbContext context) : IOrderNumbers
{
    private const string MissingBusinessMessage = "The current business does not exist.";

    public async Task<int> TakeNextAsync(CancellationToken cancellationToken)
    {
        var businessId = context.CurrentTenantId;
        var takenNumbers = await context.Database
            .SqlQuery<int>(
                $"""
                UPDATE [Businesses] SET [NextOrderNumber] = [NextOrderNumber] + 1
                OUTPUT deleted.[NextOrderNumber] AS [Value]
                WHERE [Id] = {businessId}
                """)
            .ToListAsync(cancellationToken);

        return takenNumbers is [var takenNumber] ? takenNumber : throw new InvalidOperationException(MissingBusinessMessage);
    }
}
