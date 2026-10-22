namespace ClassManager.Tenancy.AspNetCore.Persistence;

public interface ITenantDbContext
{
    long CurrentTenantId { get; }
}
