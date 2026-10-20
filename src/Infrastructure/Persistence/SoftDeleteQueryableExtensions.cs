namespace ClassManager.Infrastructure.Persistence;

public static class SoftDeleteQueryableExtensions
{
    public static IQueryable<TEntity> IncludeDeleted<TEntity>(this IQueryable<TEntity> query)
        where TEntity : class =>
        query.IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter]);
}
