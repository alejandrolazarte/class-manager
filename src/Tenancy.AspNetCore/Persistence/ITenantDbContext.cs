namespace ClassManager.Tenancy.AspNetCore.Persistence;

public interface ITenantDbContext
{
    Guid CurrentTenantId { get; }
}
