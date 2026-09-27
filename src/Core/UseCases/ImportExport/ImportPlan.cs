namespace ClassManager.Core.UseCases.ImportExport;

public sealed record ImportPlan(IReadOnlyList<ImportRowResult> Rows, Action AddValidRows);
