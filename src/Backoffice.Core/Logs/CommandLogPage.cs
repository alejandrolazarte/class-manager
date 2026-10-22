namespace ClassManager.Backoffice.Core.Logs;

public sealed record CommandLogPage(IReadOnlyList<CommandLogEntry> Entries, bool IsTruncated);
