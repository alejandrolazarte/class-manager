namespace ClassManager.Backoffice.Core.Logs;

public static class AuditLogLookups
{
    public static IReadOnlyCollection<long> DistinctIds(IEnumerable<long?> ids) =>
        [.. ids.OfType<long>().Distinct()];

    public static LoggedBranch? BranchOf(IReadOnlyDictionary<long, LoggedBranch> branches, long? tenantId) =>
        tenantId is { } id ? branches.GetValueOrDefault(id) : null;

    public static string? NameOf(IReadOnlyDictionary<long, string> userNames, long? userId) =>
        userId is { } id ? userNames.GetValueOrDefault(id) : null;
}
