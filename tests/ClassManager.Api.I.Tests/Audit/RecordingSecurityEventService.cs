using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit;

internal sealed class RecordingSecurityEventService : ISecurityEventService
{
    public List<SecurityEvent> Events { get; } = [];

    public void Record(SecurityEvent securityEvent) => Events.Add(securityEvent);
}
