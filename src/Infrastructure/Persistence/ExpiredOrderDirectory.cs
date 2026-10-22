using ClassManager.Tenancy.AspNetCore.Persistence;
namespace ClassManager.Infrastructure.Persistence;

internal sealed class ExpiredOrderDirectory(AppDbContext context) : IExpiredOrderDirectory
{
    public async Task<IReadOnlyList<long>> ListBusinessesWithUnpaidRequestsCreatedBeforeAsync(
        DateTimeOffset createdBefore, CancellationToken cancellationToken) =>
        await context.Orders.AsNoTracking()
            .IgnoreTenantFilter()
            .Where(OrderQueries.IsUnpaidRequestCreatedBefore(createdBefore))
            .Select(order => order.TenantId)
            .Distinct()
            .ToListAsync(cancellationToken);
}
