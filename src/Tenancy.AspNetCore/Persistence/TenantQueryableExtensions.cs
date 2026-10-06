using Microsoft.EntityFrameworkCore;

namespace ClassManager.Tenancy.AspNetCore.Persistence;

public static class TenantQueryableExtensions
{
    public static IQueryable<TEntity> IgnoreTenantFilter<TEntity>(this IQueryable<TEntity> query)
        where TEntity : class =>
        query.IgnoreQueryFilters([TenantModelBuilderExtensions.TenantQueryFilter]);
}
