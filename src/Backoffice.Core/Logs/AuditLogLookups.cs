namespace ClassManager.Backoffice.Core.Logs;

public static class AuditLogLookups
{
    public static IReadOnlyCollection<Guid> DistinctIds(IEnumerable<Guid?> ids) =>
        [.. ids.OfType<Guid>().Distinct()];

    public static LoggedBranch? BranchOf(IReadOnlyDictionary<Guid, LoggedBranch> branches, Guid? tenantId) =>
        tenantId is { } id ? branches.GetValueOrDefault(id) : null;

    public static string? NameOf(IReadOnlyDictionary<Guid, string> userNames, Guid? userId) =>
        userId is { } id ? userNames.GetValueOrDefault(id) : null;
}
