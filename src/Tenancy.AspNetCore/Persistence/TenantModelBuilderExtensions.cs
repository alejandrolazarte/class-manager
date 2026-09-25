using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace ClassManager.Tenancy.AspNetCore.Persistence;

public static class TenantModelBuilderExtensions
{
    public static ModelBuilder ApplyTenantQueryFilters<TContext>(this ModelBuilder modelBuilder, TContext context)
        where TContext : DbContext, ITenantDbContext
    {
        var tenantOwnedEntityTypes = modelBuilder.Model.GetEntityTypes()
            .Where(entityType => typeof(ITenantOwned).IsAssignableFrom(entityType.ClrType));

        foreach (var entityType in tenantOwnedEntityTypes)
        {
            var entity = Expression.Parameter(entityType.ClrType);
            var entityTenantId = Expression.Property(entity, nameof(ITenantOwned.TenantId));
            var currentTenantId = Expression.Property(Expression.Constant(context), nameof(ITenantDbContext.CurrentTenantId));
            var filter = Expression.Lambda(Expression.Equal(entityTenantId, currentTenantId), entity);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }

        return modelBuilder;
    }
}
