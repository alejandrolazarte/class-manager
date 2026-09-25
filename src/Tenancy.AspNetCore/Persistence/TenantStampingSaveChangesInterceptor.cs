using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ClassManager.Tenancy.AspNetCore.Persistence;

public sealed class TenantStampingSaveChangesInterceptor(ITenantContext tenantContext) : SaveChangesInterceptor
{
    private const string CrossTenantWriteMessage = "An entity that belongs to another tenant cannot be written by the current tenant.";

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        StampTenant(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        StampTenant(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void StampTenant(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<ITenantOwned>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    StampAddedEntity(entry);
                    break;
                case EntityState.Modified or EntityState.Deleted:
                    EnsureBelongsToCurrentTenant(entry.Entity.TenantId);
                    break;
                default:
                    break;
            }
        }
    }

    private void StampAddedEntity(EntityEntry<ITenantOwned> entry)
    {
        if (entry.Entity.TenantId == Guid.Empty)
        {
            entry.Property(nameof(ITenantOwned.TenantId)).CurrentValue = tenantContext.TenantId;
            return;
        }

        EnsureBelongsToCurrentTenant(entry.Entity.TenantId);
    }

    private void EnsureBelongsToCurrentTenant(Guid entityTenantId)
    {
        if (entityTenantId != tenantContext.TenantId)
        {
            throw new InvalidOperationException(CrossTenantWriteMessage);
        }
    }
}
