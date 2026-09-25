namespace ClassManager.Tenancy;

public interface ITenantContext
{
    Guid TenantId { get; }
}
