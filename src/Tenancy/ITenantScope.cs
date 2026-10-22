namespace ClassManager.Tenancy;

public interface ITenantScope
{
    void Establish(long tenantId);
}
