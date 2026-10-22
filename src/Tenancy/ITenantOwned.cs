namespace ClassManager.Tenancy;

public interface ITenantOwned
{
    long TenantId { get; }
}
