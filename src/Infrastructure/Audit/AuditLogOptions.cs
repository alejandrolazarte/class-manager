namespace ClassManager.Infrastructure.Audit;

public sealed class AuditLogOptions
{
    public const string SectionName = "AuditLog";
    public const string ConnectionStringName = "AuditLog";

    public Uri? TableServiceUri { get; init; }

    public Uri? BlobServiceUri { get; init; }

    public int MaximumSecurityEventsPerAddressPerMinute { get; init; } = 10;
}
