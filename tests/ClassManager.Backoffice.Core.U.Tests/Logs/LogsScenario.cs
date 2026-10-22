using ClassManager.Core.Common;
using Microsoft.Extensions.Time.Testing;

namespace ClassManager.Backoffice.Core.U.Tests.Logs;

internal sealed class LogsScenario
{
    public static readonly DateTimeOffset Now = new(2026, 10, 21, 15, 0, 0, TimeSpan.Zero);

    public static readonly DateOnly Today = DateOnly.FromDateTime(Now.UtcDateTime);

    private readonly FakeTimeProvider _timeProvider = new(Now);

    public InMemoryAuditLogReader Reader { get; } = new();

    public InMemoryAuditLogDirectory Directory { get; } = new();

    public LoggedBranch WithBranch(string brandName, string branchName)
    {
        var branch = new LoggedBranch(TestIds.Next(), branchName, TestIds.Next(), brandName);
        Directory.Branches.Add(branch);
        return branch;
    }

    public CommandLogEntry WithCommand(string command, DateTimeOffset occurredAt, long? tenantId = null, long? userId = null)
    {
        var entry = new CommandLogEntry(occurredAt, tenantId, command, null, userId, "Succeeded", null, null, 12, null, "{}", false, null, "App");
        Reader.Commands.Add(entry);
        return entry;
    }

    public SecurityLogEntry WithSecurityEvent(string eventType, DateTimeOffset occurredAt, long? tenantId = null)
    {
        var entry = new SecurityLogEntry(occurredAt, eventType, tenantId, null, null, "/api/students", "GET", 403, "203.0.113.7", null, null, null);
        Reader.SecurityEvents.Add(entry);
        return entry;
    }

    public Task<Result<CommandLogResponse>> ListCommandsAsync(DateOnly? day = null, long? brandId = null) =>
        new ListCommandLogUseCase(Reader, Directory, _timeProvider)
            .ExecuteAsync(new ListCommandLogQuery(day, brandId), CancellationToken.None);

    public Task<Result<SecurityEventListResponse>> ListSecurityEventsAsync(DateOnly? day = null) =>
        new ListSecurityEventsUseCase(Reader, Directory, _timeProvider)
            .ExecuteAsync(new ListSecurityEventsQuery(day), CancellationToken.None);
}

internal sealed class InMemoryAuditLogReader : IAuditLogReader
{
    public bool IsConnected { get; set; } = true;

    public List<CommandLogEntry> Commands { get; } = [];

    public List<SecurityLogEntry> SecurityEvents { get; } = [];

    public List<DateOnly> ReadDays { get; } = [];

    public IReadOnlyCollection<long>? ReadTenantIds { get; private set; }

    public bool IsTruncated { get; set; }

    public Task<CommandLogPage> ListCommandsAsync(DateOnly day, IReadOnlyCollection<long>? tenantIds, CancellationToken cancellationToken)
    {
        ReadDays.Add(day);
        ReadTenantIds = tenantIds;
        return Task.FromResult(new CommandLogPage(
            [.. Commands.Where(entry => tenantIds is null || (entry.TenantId is { } tenantId && tenantIds.Contains(tenantId)))],
            IsTruncated));
    }

    public Task<IReadOnlyList<SecurityLogEntry>> ListSecurityEventsAsync(DateOnly day, CancellationToken cancellationToken)
    {
        ReadDays.Add(day);
        return Task.FromResult<IReadOnlyList<SecurityLogEntry>>(SecurityEvents);
    }
}

internal sealed class InMemoryAuditLogDirectory : IAuditLogDirectory
{
    public List<LoggedBranch> Branches { get; } = [];

    public Dictionary<long, string> UserNames { get; } = [];

    public Task<IReadOnlyList<long>> ListBranchIdsAsync(long brandId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<long>>([.. Branches.Where(branch => branch.BrandId == brandId).Select(branch => branch.BranchId)]);

    public Task<IReadOnlyDictionary<long, LoggedBranch>> FindBranchesAsync(IReadOnlyCollection<long> branchIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<long, LoggedBranch>>(Branches.Where(branch => branchIds.Contains(branch.BranchId)).ToDictionary(branch => branch.BranchId));

    public Task<IReadOnlyDictionary<long, string>> FindUserNamesAsync(IReadOnlyCollection<long> userIds, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<long, string>>(UserNames.Where(pair => userIds.Contains(pair.Key)).ToDictionary());
}
