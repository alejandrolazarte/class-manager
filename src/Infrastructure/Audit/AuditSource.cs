namespace ClassManager.Infrastructure.Audit;

public sealed record AuditSource(string Name)
{
    public const string AppName = "App";
    public const string BackofficeName = "Backoffice";

    public static AuditSource App { get; } = new(AppName);

    public static AuditSource Backoffice { get; } = new(BackofficeName);
}
