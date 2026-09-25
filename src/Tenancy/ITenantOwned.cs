namespace ClassManager.Tenancy;

public interface ITenantOwned
{
    Guid TenantId { get; }
}
