namespace ClassManager.ImportExport.Tabular;

public sealed record TabularData(IReadOnlyList<string> Headers, IReadOnlyList<TabularRow> Rows);
