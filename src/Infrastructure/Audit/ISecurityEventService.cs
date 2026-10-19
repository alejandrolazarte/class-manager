namespace ClassManager.Infrastructure.Audit;

public interface ISecurityEventService
{
    void Record(SecurityEvent securityEvent);
}
