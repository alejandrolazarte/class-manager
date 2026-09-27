namespace ClassManager.Core.UseCases.ImportExport;

public sealed record ImportSummary(int Total, int Valid, int Errors, int Skipped)
{
    public static ImportSummary From(IReadOnlyList<ImportRowResult> rows) => new(
        rows.Count,
        rows.Count(row => row.Status == ImportRowStatus.Valid),
        rows.Count(row => row.Status == ImportRowStatus.Error),
        rows.Count(row => row.Status == ImportRowStatus.Skipped));
}
