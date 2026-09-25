namespace ClassManager.Tenancy;

public interface ITenantScope
{
    void Establish(Guid tenantId);
}
