namespace ClassManager.Infrastructure.Audit;

public interface ICommandAuditService
{
    void Record(CommandAuditEntry entry);
}
