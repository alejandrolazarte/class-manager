using ClassManager.Tenancy.AspNetCore.Persistence;
namespace ClassManager.Infrastructure.Persistence;

internal sealed class ExpiredOrderDirectory(AppDbContext context) : IExpiredOrderDirectory
{
    public async Task<IReadOnlyList<Guid>> ListBusinessesWithRequestsCreatedBeforeAsync(
        DateTimeOffset createdBefore, CancellationToken cancellationToken) =>
        await context.Orders.AsNoTracking()
            .IgnoreTenantFilter()
            .Where(order => order.Status == OrderStatus.Requested && order.CreatedAt <= createdBefore)
            .Select(order => order.TenantId)
            .Distinct()
            .ToListAsync(cancellationToken);
}
