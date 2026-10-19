namespace ClassManager.Infrastructure.Audit;

public sealed class QueuedSecurityEventService(AuditLogChannel<SecurityEvent> channel) : ISecurityEventService
{
    public void Record(SecurityEvent securityEvent) => channel.TryWrite(securityEvent);
}
