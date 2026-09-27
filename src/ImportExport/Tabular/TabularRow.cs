namespace ClassManager.ImportExport.Tabular;

public sealed record TabularRow(int LineNumber, IReadOnlyList<string> Cells);
